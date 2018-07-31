using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenCV_1stTry
{
    class SimpleCameraAndHelloWorld
    {

        public static void SimpleCameraCapture()
        {
            ImageViewer viewer = new ImageViewer(); //create an image viewer
            VideoCapture capture = new VideoCapture(); //create a camera captue
            Application.Idle += new EventHandler(delegate (object sender, EventArgs e)
            {  //run this until application closed (close button click on image viewer)
                viewer.Image = capture.QueryFrame(); //draw the image obtained from camera
            });
            viewer.ShowDialog(); //show the image viewer
        }


        public static void ProcessCameraCapture()
        {

            ImageViewer viewer = new ImageViewer(); //create an image viewer
            VideoCapture capture = new VideoCapture(); //create a camera captue
            Application.Idle += new EventHandler(delegate (object sender, EventArgs e)
            {  //run this until application closed (close button click on image viewer)
                var frame = capture.QueryFrame(); //draw the image obtained from camera

                // =======================================
                // Let's process this frame in some way
                // =======================================

                // Basic processing image data: 
                // http://www.emgu.com/wiki/index.php/Setting_up_EMGU_C_Sharp
                // **************************************************************

                // *********************** //
                // Process grayscale frame //
                // *********************** //
                Image<Gray, byte> img_gray = frame.ToImage<Gray, byte>();

                //img.Data[0, 0, 0] = 255;
                //for (int i = 0; i < img.Height; i++)
                //{
                //    for (int j = 0; j < img.Width; j++)
                //    {
                //        img.Data[i, j, 0] = (byte)(255 - img.Data[i, j, 0]);
                //    }
                //}



                // *********************************** //
                // Process colored frame               //
                // *********************************** //
                Image<Bgr, byte> img = frame.ToImage<Bgr, byte>();

                for (int i = 0; i < img.Height; i++)
                {
                    for (int j = 0; j < img.Width; j++)
                    {
                        if (img_gray.Data[i, j, 0] < 50)
                        {
                            img.Data[i, j, 0] = (byte)((img.Data[i, j, 0] * 2) % 255);
                            img.Data[i, j, 1] = (byte)((img.Data[i, j, 1] * 2) % 255);
                            img.Data[i, j, 2] = (byte)((img.Data[i, j, 2] * 2) % 255);
                        }

                        //if ((i < img.Height / 2 && j < img.Width / 2) || (i>=img.Height / 2 && j >img.Width / 2))
                        //{
                        //    img.Data[i, j, 0] = (byte)(255 - img.Data[i, j, 0]);
                        //    img.Data[i, j, 1] = (byte)(255 - img.Data[i, j, 1]);
                        //    img.Data[i, j, 2] = (byte)(255 - img.Data[i, j, 2]);
                        //}
                    }
                }




                // ************************************** //
                // Get data in standard Bitmap class form //
                // ************************************** //
                Bitmap fb = frame.Bitmap;

                //for (int ix = 0; ix < fb.Width; ix++)
                //{
                //    for (int iy = 0; iy < fb.Height; iy++)
                //    {
                //        int tmp = (ix + iy) % 3;
                //        if (tmp == 1 || tmp == 2)
                //        //if (fb.GetPixel(ix, iy) == targetColor)
                //        {
                //            fb.SetPixel(ix, iy, Color.Black);
                //        }
                //    }
                //}


                // Grayscale Checkboard //
                int cellSize = 100;
                for (int i = 0; i < img_gray.Height; i++)
                {
                    for (int j = 0; j < img_gray.Width; j++)
                    {
                        int ibit = (i / cellSize) % 2;
                        int jbit = (j / cellSize) % 2;
                        if (ibit == jbit)
                        {
                            img_gray.Data[i, j, 0] = (byte)(255 - img_gray.Data[i, j, 0]);
                        }
                    }
                }



                // =======================================
                // ***************************************
                // =======================================


                //viewer.Image = img;
                viewer.Image = img_gray;
                //viewer.Image = frame;
            });
            viewer.ShowDialog(); //show the image viewer
        }


        public static void HelloWorld()
        {
            String win1 = "Test Window"; //The name of the window
            CvInvoke.NamedWindow(win1); //Create the window using the specific name

            Mat img = new Mat(350, 350, DepthType.Cv8U, 3); //Create a 3 channel image of 400x200
            img.SetTo(new Bgr(150, 150, 0).MCvScalar); // set it to Blue color


            //Draw "Hello, world." on the image using the specific font
            CvInvoke.PutText(
               img,
               "Hello, CV!",
               new System.Drawing.Point(100, 175),
               FontFace.HersheyComplex,
               1.0,
               new Bgr(255, 255, 255).MCvScalar);

            CvInvoke.Imshow(win1, img); //Show the image
            CvInvoke.WaitKey(0);  //Wait for the key pressing event
            CvInvoke.DestroyWindow(win1); //Destroy the window if key is pressed

        }


    }
}
