using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows;

namespace TimeSyncer
{
    public enum ExifOrientations
    {
        None = 0,
        Normal = 1,
        HorizontalFlip = 2,
        Rotate180 = 3,
        VerticalFlip = 4,
        Transpose = 5,
        Rotate270 = 6,
        Transverse = 7,
        Rotate90 = 8
    }




    public class PhotoViewModel : INotifyPropertyChanged
    {
        #region Membres
        private BitmapSource _thumbnail = null;
        /// <summary>
        /// INotofyPropertyChanged event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private ExifOrientations orientation = ExifOrientations.Normal;
        #endregion

        #region Ctors
        /// <summary>
        /// Crée une instance du view model permettant d'obtenir un thumbnail d'une image
        /// </summary>
        /// <param name="mediaUrl">Url de l'image à manipuler</param>
        public PhotoViewModel(String mediaUrl)
        {
            MediaUrl = mediaUrl;
        }
        public PhotoViewModel(String mediaUrl, Brush borderBrushh, DateTime dateTaken, bool isPicture)
        {
            MediaUrl = mediaUrl;
            BorderBrushh = borderBrushh;
            DateTaken = dateTaken;
            IsPicture = isPicture;
        }
        #endregion

        #region Propriétées

        /// <summary>
        /// Détermine l'url de l'image à manipuler
        /// </summary>
        public String MediaUrl
        {
            get;
            private set;
        }

        /// <summary>
        /// Détermine si la génération de la miniature à échoué
        /// </summary>
        public Boolean IsFailed
        {
            get;
            private set;
        }

        /// <summary>
        /// Obtient le thumbnail de l'image source
        /// </summary>
        public BitmapSource Thumbnail
        {
            get
            {
                if ((_thumbnail == null) && !IsFailed)
                {
                    _thumbnail = GetThumbnail();
                }                    
                return _thumbnail;
            }
        }
        public Brush BorderBrushh { get; set; }
        public DateTime DateTaken { get; internal set; }
        public bool IsPicture { get; private set; }
        #endregion


        #region Méthodes
        /// <summary>
        /// Libère les ressources utilisées
        /// </summary>
        public void Dispose()
        {
            _thumbnail = null;
        }

        /// <summary>
        /// Obtient le thumnail de l'image source (incluant la rotation)
        /// </summary>
        /// <returns></returns>
        private BitmapSource GetThumbnail()
        {
            if (!IsPicture)
            {
                return null;
            }

            BitmapSource ret = null;
            BitmapMetadata meta = null;
            double angle = 0;

            try
            {
                //tentative de creation du thumbnail via Bitmap frame : très rapide et très peu couteux en mémoire !
                BitmapFrame frame = BitmapFrame.Create(
                    new Uri(MediaUrl),
                    BitmapCreateOptions.DelayCreation,
                    BitmapCacheOption.None);

                if (frame.Thumbnail == null) //echec, on tente avec BitmapImage (plus lent et couteux en mémoire)
                {
                    BitmapImage image = new BitmapImage();
                    image.DecodePixelHeight = 90; //on limite à 90px de hauteur
                    image.BeginInit();
                    image.UriSource = new Uri(MediaUrl);
                    image.CacheOption = BitmapCacheOption.None;
                    image.CreateOptions = BitmapCreateOptions.DelayCreation; //important pour les performances
                    image.EndInit();

                    if (image.CanFreeze) //pour éviter les memory leak
                        image.Freeze();

                    ret = image;
                }
                else
                {
                    //récupération des métas de l'image
                    meta = frame.Metadata as BitmapMetadata;
                    ret = frame.Thumbnail;
                }

                if ((meta != null) && (ret != null)) //si on a des meta, tentative de récupération de l'orientation
                {
                    if (meta.GetQuery("/app1/ifd/{ushort=274}") != null)
                    {
                        orientation = (ExifOrientations)Enum.Parse(typeof(ExifOrientations), meta.GetQuery("/app1/ifd/{ushort=274}").ToString());
                    }

                    switch (orientation)
                    {
                        case ExifOrientations.Rotate90:
                            angle = -90;
                            break;
                        case ExifOrientations.Rotate180:
                            angle = 180;
                            break;
                        case ExifOrientations.Rotate270:
                            angle = 90;
                            break;
                    }

                    if (angle != 0) //on doit effectuer une rotation de l'image
                    {
                        ret = new TransformedBitmap(ret.Clone(), new RotateTransform(angle));
                        ret.Freeze();
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return ret;
        }
        #endregion

        #region Handlers
        public void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}