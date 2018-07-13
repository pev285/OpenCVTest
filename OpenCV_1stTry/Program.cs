using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Cvb;
using Emgu.CV.UI;

using System.Drawing;
using System.Windows.Forms;

namespace OpenCV_1stTry
{
    class Program
    {
        static void Main(string[] args)
        {
            //HelloWorld();
            SimpleCameraCapture();



        } // Main() //

        //https://ru.stackoverflow.com/questions/723412/c-emgu-%d0%bd%d0%b5-%d0%b7%d0%bd%d0%b0%d0%b5%d1%82-%d0%ba%d0%bb%d0%b0%d1%81%d1%81%d1%8b-capture-%d0%b8-haarcascade/723586#723586

        static void Habr()
        {

            // https://habr.com/post/260741/

            VideoCapture capture = new VideoCapture();


            //for face
            CascadeClassifier faceCascade = new CascadeClassifier("haarcascade_frontalface_alt.xml");
            //for eye
            CascadeClassifier eyeCascade = new CascadeClassifier("haarcascade_eye.xml");

            Mat image = capture.QueryFrame();

            var Face = faceCascade.DetectMultiScale(image);


            foreach (var face in Face)
            {
                //Если таковы найдены, то рисуем вокруг них круг, с заданным цветом и толщиной линии.
                //image.Draw(face.rect, new Bgr(255, 255, 255), 10);
            }

            //image.ConvertTo(, DepthType.Cv8U);

            //var Face = image.Detect

        }



        static void SimpleCameraCapture()
        {
            ImageViewer viewer = new ImageViewer(); //create an image viewer
            VideoCapture capture = new VideoCapture(); //create a camera captue
            Application.Idle += new EventHandler(delegate (object sender, EventArgs e)
            {  //run this until application closed (close button click on image viewer)
                viewer.Image = capture.QueryFrame(); //draw the image obtained from camera
            });
            viewer.ShowDialog(); //show the image viewer
        }


        static void HelloWorld()
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



    } // end of class ///
} // namespace ////
