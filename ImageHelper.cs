using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace DynamicFaceDemo
{
    public static class ImageHelper
    {
        /// <summary>
        /// Compressed picture
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Bitmap CustomSizeImage(Image img)
        {
            int gd = 720;
            int width = img.Width;
            int heigth = img.Height;
            double scale = 0.0;
            int h = 0;
            int w = 0;
            double whX = 0.0;
            if (width <= heigth)
            {
                scale = ((double)width) / ((double)heigth);
                h = (int)(gd / scale);
                whX = (double)h / (double)gd;
                w = gd;

                if (width < gd)
                {
                    h = heigth;
                    w = width;
                    whX = (double)heigth / (double)width;
                }
            }
            else
            {
                scale = ((double)heigth) / ((double)width);
                w = (int)(gd / scale);
                whX = (double)w / (double)gd;
                h = gd;

                if (heigth < gd)
                {
                    h = heigth;
                    w = width;
                    whX = (double)width / (double)heigth;
                }
            }

            Bitmap bmp = new Bitmap(w, h);
            int srcX = 0;
            int srcY = 0;
            int srcW = img.Width;
            int srcH = img.Height;
            double zoom = ((double)w) / ((double)srcW);
            Graphics g = Graphics.FromImage(bmp);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            Color c = Color.FromArgb(new Bitmap(img).GetPixel(1, 1).ToArgb());
            g.Clear(c);
            if (srcW * whX != srcH)
            {
                if (srcW * whX >= srcH)
                {
                    srcH = Convert.ToInt32(srcW * whX);
                    srcY = -(srcH - img.Height) / 2;
                }
                else
                {
                    srcW = Convert.ToInt32(srcH / whX);
                    srcX = -(srcW - img.Width) / 2;
                }
            }
            g.DrawImage(img, new Rectangle(0, 0, w, h), new Rectangle(srcX, srcY, srcW, srcH), GraphicsUnit.Pixel);
            img.Dispose();
            g.Dispose();
            return bmp;
        }

        /// <summary>
        /// Convert Image to Byte[]
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ImageToBytes(Image image)
        {
            ImageFormat format = image.RawFormat;
            using (MemoryStream ms = new MemoryStream())
            {
                if (format.Equals(ImageFormat.Jpeg))
                {
                    image.Save(ms, ImageFormat.Jpeg);
                }
                else if (format.Equals(ImageFormat.Png))
                {
                    image.Save(ms, ImageFormat.Png);
                }
                else if (format.Equals(ImageFormat.Bmp))
                {
                    image.Save(ms, ImageFormat.Bmp);
                }
                else if (format.Equals(ImageFormat.Gif))
                {
                    image.Save(ms, ImageFormat.Gif);
                }
                else if (format.Equals(ImageFormat.Icon))
                {
                    image.Save(ms, ImageFormat.Icon);
                }
                byte[] buffer = new byte[ms.Length];
                
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        /// <summary>
        /// Convert Byte[] to Image
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Image BytesToImage(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            Image image = System.Drawing.Image.FromStream(ms);
            return image;
        }

        /// <summary>
        /// Convert Byte[] to a picture and Store it in file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string CreateImageFromBytes(string fileName, byte[] buffer)
        {
            string file = fileName;
            Image image = BytesToImage(buffer);
            ImageFormat format = image.RawFormat;
            if (format.Equals(ImageFormat.Jpeg))
            {
                file += ".jpeg";
            }
            else if (format.Equals(ImageFormat.Png))
            {
                file += ".png";
            }
            else if (format.Equals(ImageFormat.Bmp))
            {
                file += ".bmp";
            }
            else if (format.Equals(ImageFormat.Gif))
            {
                file += ".gif";
            }
            else if (format.Equals(ImageFormat.Icon))
            {
                file += ".icon";
            }
            System.IO.FileInfo info = new System.IO.FileInfo(file);
            System.IO.Directory.CreateDirectory(info.Directory.FullName);
            File.WriteAllBytes(file, buffer);
            return file;
        }
    }
}
