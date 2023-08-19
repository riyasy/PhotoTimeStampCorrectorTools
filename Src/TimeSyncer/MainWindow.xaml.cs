using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;


namespace TimeSyncer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ObservableCollection<PhotoViewModel> Images = new ObservableCollection<PhotoViewModel>();

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();            
        }

        private void SelectFolderButtonClicked(object sender, RoutedEventArgs e)
        {
            var tb = (sender as System.Windows.Controls.Button).Tag as TextBlock;
            FolderBrowserDialog dlg1 = new FolderBrowserDialog();
            if (Directory.Exists(tb.Text))
            {
                dlg1.SelectedPath = tb.Text;
            }
            if (dlg1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tb.Text = dlg1.SelectedPath;
            }
        }
        
        private void buttonRefreshClicked(object sender, RoutedEventArgs e)
        {
            listBox.ItemsSource = Images;
            Images.Clear();

            List<PhotoViewModel> imgs = new List<PhotoViewModel>();

            int value1 = String.IsNullOrEmpty(tb1.Text) ? 0 : Convert.ToInt32(tb1.Text);
            int value2 = String.IsNullOrEmpty(tb2.Text) ? 0 : Convert.ToInt32(tb2.Text);
            int value3 = String.IsNullOrEmpty(tb3.Text) ? 0 : Convert.ToInt32(tb3.Text);
            int value4 = String.IsNullOrEmpty(tb4.Text) ? 0 : Convert.ToInt32(tb4.Text);
            int value5 = String.IsNullOrEmpty(tb5.Text) ? 0 : Convert.ToInt32(tb5.Text);

            AddImages(imgs, textBlock1.Text, Brushes.Red, value1);
            AddImages(imgs, textBlock2.Text, Brushes.Green, value2);
            AddImages(imgs, textBlock3.Text, Brushes.Blue, value3);
            AddImages(imgs, textBlock4.Text, Brushes.Brown, value4);
            AddImages(imgs, textBlock5.Text, Brushes.Orange, value5);

            var sorted = imgs.OrderBy(x => x.DateTaken).ToList();
            foreach (var photo in sorted)
            {
                Images.Add(photo);
            }
        }

        private static void AddImages(List<PhotoViewModel> imgs, string folderName, Brush borderBrush, int offSet)
        {
            if (Directory.Exists(folderName))
            {
                foreach (string s in Directory.GetFiles(folderName))
                {
                    var dateTime = GetDateTimeFromFileName(s);
                    if (dateTime != null)
                    {         
                        if (s.ToLower().EndsWith(".jpg") || s.ToLower().EndsWith(".jpeg"))
                        {
                            imgs.Add(new PhotoViewModel(s, borderBrush, dateTime.Value.AddSeconds(offSet), true));
                        }    
                        else
                        {
                            imgs.Add(new PhotoViewModel(s, borderBrush, dateTime.Value.AddSeconds(offSet), false));
                        }                        
                    }
                }
            }
        }

        public static DateTime? GetDateTimeFromFileName(string path)
        {
            try
            {
                var fileName = System.IO.Path.GetFileName(path);
                if (fileName.Length >= 15)
                {
                    string formatString = "yyyyMMddHHmmss";
                    DateTime dateTime = DateTime.ParseExact(fileName.Substring(0, 15).Replace("_", ""), formatString, null);
                    return dateTime;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                if (listBox.SelectedItem != null)
                {
                    var photo = listBox.SelectedItem as PhotoViewModel;
                    System.Diagnostics.Process.Start(photo.MediaUrl);
                }
            }
        }

        private void buttonModifyClicked(object sender, RoutedEventArgs e)
        {
            string sMessageBoxText = "This will modifiy Modified date of all files. Do you want to continue?";
            string sCaption = "Careful";
            MessageBoxResult rsltMessageBox = System.Windows.MessageBox.Show(sMessageBoxText, sCaption, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (rsltMessageBox == MessageBoxResult.Yes)
            {
                var imagesCopy = Images.ToList();
                Images.Clear();

                foreach (var img in imagesCopy)
                {
                    img.Dispose();
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                foreach (var img in imagesCopy)
                {
                    File.SetLastWriteTime(img.MediaUrl, img.DateTaken);
                }

                System.Windows.MessageBox.Show("Completed");
            }
        }

        private void PathTextBlockMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                var path = System.Windows.Clipboard.GetText();
                if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                {
                    var tb = sender as TextBlock;
                    tb.Text = path;
                }
            }
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StringBuilder bb = new StringBuilder();
                AddStringFromTB(textBlock1, bb);
                AddStringFromTB(textBlock2, bb);
                AddStringFromTB(textBlock3, bb);
                AddStringFromTB(textBlock4, bb);
                AddStringFromTB(textBlock5, bb);
                AddStringFromTB(tb1, bb);
                AddStringFromTB(tb2, bb);
                AddStringFromTB(tb3, bb);
                AddStringFromTB(tb4, bb);
                AddStringFromTB(tb5, bb);
                File.WriteAllText(saveFileDialog.FileName, bb.ToString());
            }                
        }

        private void AddStringFromTB(TextBlock tb, StringBuilder bb)
        {
            if (string.IsNullOrEmpty(tb.Text))
            {
                bb.Append("NULL" + ">");
            }
            else
            {
                bb.Append(tb.Text + ">");
            }
        }

        private void AddStringFromTB(System.Windows.Controls.TextBox tb, StringBuilder bb)
        {
            if (string.IsNullOrEmpty(tb.Text))
            {
                bb.Append("NULL" + ">");
            }
            else
            {
                bb.Append(tb.Text + ">");
            }
        }

        private void buttonLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var fileText = File.ReadAllText(openFileDialog.FileName);
                var splits = fileText.Split('>');
                if (splits.Length >= 10)
                {
                    SetText(textBlock1, splits, 0);
                    SetText(textBlock2, splits, 1);
                    SetText(textBlock3, splits, 2);
                    SetText(textBlock4, splits, 3);
                    SetText(textBlock5, splits, 4);

                    SetText(tb1, splits, 5);
                    SetText(tb2, splits, 6);
                    SetText(tb3, splits, 7);
                    SetText(tb4, splits, 8);
                    SetText(tb5, splits, 9);

                }
            }           
                
        }

        private void SetText(TextBlock tb, string[] splits, int index)
        {
            if (splits[index] == "NULL")
            {
                tb.Text = string.Empty;
            }
            else
            {
                tb.Text = splits[index];
            }
        }

        private void SetText(System.Windows.Controls.TextBox tb, string[] splits, int index)
        {
            if (splits[index] == "NULL")
            {
                tb.Text = string.Empty;
            }
            else
            {
                tb.Text = splits[index];
            }
        }


    }
}
