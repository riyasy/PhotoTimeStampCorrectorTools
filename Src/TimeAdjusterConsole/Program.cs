using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAdjusterConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter seconds to adjust photos :");
            var photoTimeAdjustStr = Console.ReadLine();
            int photoTimeAdjust = int.Parse(photoTimeAdjustStr);

            Console.WriteLine("Enter seconds to adjust videos :");
            var videoTimeAdjustStr = Console.ReadLine();
            int videoTimeAdjust = int.Parse(videoTimeAdjustStr);

            Dictionary<string, DateTime> imgs = new Dictionary<string, DateTime>();
            Dictionary<string, DateTime> vids = new Dictionary<string, DateTime>();

            if (Directory.Exists(args[0]))
            {
                foreach (var fil in Directory.GetFiles(args[0]))
                {
                    if (fil.ToLower().EndsWith(".jpg"))
                    {
                        var dateTime = GetDateTimeFromFileName(fil);
                        if (dateTime != null)
                        {
                            imgs.Add(fil, dateTime.Value);
                        }
                    }
                    else
                    {
                        var dateTime = GetDateTimeFromFileName(fil);
                        if (dateTime != null)
                        {
                            vids.Add(fil, dateTime.Value);
                        }
                    }
                }
            }
            else
            {
                foreach (var fil in args)
                {
                    if (fil.ToLower().EndsWith(".jpg"))
                    {
                        var dateTime = GetDateTimeFromFileName(fil);
                        if (dateTime != null)
                        {
                            imgs.Add(fil, dateTime.Value);
                        }
                    }
                    else
                    {
                        var dateTime = GetDateTimeFromFileName(fil);
                        if (dateTime != null)
                        {
                            vids.Add(fil, dateTime.Value);
                        }
                    }
                }
            }



            //if (videoTimeAdjust != 0)
            {
                foreach (var vid in vids)
                {
                    Console.WriteLine(vid.Key + " | " + vid.Value.AddSeconds(videoTimeAdjust));
                    File.SetLastWriteTime(vid.Key, vid.Value.AddSeconds(videoTimeAdjust));
                }
            }

            //if (photoTimeAdjust != 0)
            {
                foreach (var img in imgs)
                {
                    Console.WriteLine(img.Key + " | " + img.Value.AddSeconds(photoTimeAdjust));
                    File.SetLastWriteTime(img.Key, img.Value.AddSeconds(photoTimeAdjust));
                }
            }
            Console.WriteLine("Finished");
            Console.Read();
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
    }
}
