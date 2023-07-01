using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Rectangle = System.Drawing.Rectangle;
//System.Drawing version 5 only

namespace libScreenshotBase64XlibTest
{
    internal class Program
    {
        //libScreenshotBase64Xlib.so should be next to the executable
        [DllImport("libScreenshotBase64Xlib.so", EntryPoint = "GetScreenshotBase64")]
         public static extern IntPtr GetScreenshotBase64();
         
         
         
        public static byte[] Base64ToByteArray(string base64String)
        {
            byte[] byteArray = Convert.FromBase64String(base64String);
            return byteArray;
        }
        public static string CaptureScreen()
        {
            IntPtr resultPtr = GetScreenshotBase64();
            
            string result = Marshal.PtrToStringAnsi(resultPtr);

            Marshal.FreeCoTaskMem(resultPtr);
            
            return result;
        }
        
        public static Bitmap ZPixmapToBitmap(byte[] imageData, int width, int height, int stride)
        {
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            IntPtr bmpPtr = bmpData.Scan0;


            Marshal.Copy(imageData, 0, bmpPtr, imageData.Length);

            bitmap.UnlockBits(bmpData);

            return bitmap;
       
        }
       
        public static Bitmap CaptureScreenBitmap(int screenWidth,int screenHeight)
        {
            string base64Image = CaptureScreen(); 

            return ZPixmapToBitmap(Base64ToByteArray(base64Image), screenWidth, screenHeight,0); // Преобразование Base64 в Bitmap
       
        }
        
        static void Main(string[] args)
        {
            CaptureScreenBitmap(1920,1080).Save("test.png");
        }
    }
}