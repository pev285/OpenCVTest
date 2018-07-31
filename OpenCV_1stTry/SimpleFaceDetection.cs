using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenCV_1stTry
{
    static class SimpleFaceDetection
    {



        public static void DetectFaces()
        {

            ImageViewer viewer = new ImageViewer(); //create an image viewer
            VideoCapture capture = new VideoCapture(); //create a camera captue

            CascadeClassifier faceClassifier = new CascadeClassifier(@"C:\Emgu\emgucv-windesktop 3.4.1.2976\etc\haarcascades\haarcascade_frontalface_alt2.xml");
            CascadeClassifier eyesClassifier = new CascadeClassifier(@"C:\Emgu\emgucv-windesktop 3.4.1.2976\etc\haarcascades\haarcascade_eye.xml");

            int mills = DateTime.Now.Millisecond;

            Stopwatch watch = Stopwatch.StartNew();

            Application.Idle += new EventHandler(delegate (object sender, EventArgs e)
            {  //run this until application closed (close button click on image viewer)
                var frame = capture.QueryFrame(); //draw the image obtained from camera


                Image<Gray, byte> img_gray = frame.ToImage<Gray, byte>();
                Image<Bgr, byte> img = frame.ToImage<Bgr, byte>();
                //Image<Gray, byte> img = img_gray;


                var faces = faceClassifier.DetectMultiScale(img_gray, 2 /*1.1*/);
                var eyes = eyesClassifier.DetectMultiScale(img_gray);

                foreach (var face in faces)
                {
                    img.Draw(face, new Bgr(0, 255, 0), 2);
                    //img.Draw(face, new Gray(255), 2);
                }
                foreach (var eye in eyes)
                {
                    img.Draw(eye, new Bgr(255, 255, 0), 1);
                    //img.Draw(eye, new Gray(200), 1);
                }

                watch.Stop();
                long deltaMills = watch.ElapsedMilliseconds;
                watch.Reset();
                watch.Start();

                CvInvoke.PutText(
                       img,
                       "FPS: " + 1000 / deltaMills,
                       new System.Drawing.Point(10, 30),
                       FontFace.HersheyComplex,
                       0.7,// TextSize
                       new Bgr(0, 0, 255).MCvScalar);

                CvInvoke.PutText(
                       img,
                       "frame width = " + img.Width + ", frame height = " + img.Height + ", deltaMills=" + deltaMills,
                       new System.Drawing.Point(10, 50),
                       FontFace.HersheyComplex,
                       0.5,// TextSize
                       new Bgr(255, 0, 0).MCvScalar);




                viewer.Image = img;
            });
            viewer.ShowDialog(); //show the image viewer

            faceClassifier.Dispose();
            eyesClassifier.Dispose();

        } // DetectFaces() ///


    }
}
