using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using SearchNumbers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCV_1stTry
{
    class DoesNotWork
    {



        #region CarNumberDetecting

        private static NumberDetector _numberDetector;
        //
        static void CarNumberDetecting()
        {
            //Перебираем все *.jpg в папке img
            string dir_name = Directory.GetCurrentDirectory() + "\\img\\";
            Console.WriteLine("curdir=" + dir_name);
            Console.ReadLine();
            DirectoryInfo dir = new DirectoryInfo(dir_name);

            foreach (FileInfo file in dir.GetFiles("*.jpg"))
            {
                SaveInFile(dir_name + file.Name);

                _numberDetector = new NumberDetector("");


                //Способ конвертировать из обычного Image - медленнее но может пригодиться.
                Image img_ext = Image.FromFile(dir_name + file.Name);
                Mat img = GetMatFromSDImage(img_ext);

                //Способ взять сразу картинку с диска как Mat.
                //Mat img;
                //img = CvInvoke.Imread("c:\\IMG\\960.jpg");



                UMat uImg = img.GetUMat(AccessType.ReadWrite);
                string res = ProcessImage(uImg);

                SaveInFile(res);
                SaveInFile("");
            }
            Console.WriteLine("Нажмите любую клавишу чтобы закрыть приложение.");
            //Закончили и ждем когда нажмут клавишу чтобы выйти.
            Console.ReadKey();

        }

        /// <summary>
        /// Преобразует изображение Image в Mat
        /// </summary>
        private static Mat GetMatFromSDImage(System.Drawing.Image image)
        {
            int stride = 0;
            Bitmap bmp = new Bitmap(image);

            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);

            System.Drawing.Imaging.PixelFormat pf = bmp.PixelFormat;
            if (pf == System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                stride = bmp.Width * 4;
            }
            else
            {
                stride = bmp.Width * 3;
            }

            Image<Bgra, byte> cvImage = new Image<Bgra, byte>(bmp.Width, bmp.Height, stride, (IntPtr)bmpData.Scan0);

            bmp.UnlockBits(bmpData);

            return cvImage.Mat;
        }

        /// <summary>
        /// Обработка изображения
        /// </summary>
        private static string ProcessImage(IInputOutputArray image)
        {

            Stopwatch watch = Stopwatch.StartNew(); //Засекаем время, чтобы понять сколько ушло на обработку

            List<IInputOutputArray> licensePlateImagesList = new List<IInputOutputArray>();
            List<IInputOutputArray> filteredLicensePlateImagesList = new List<IInputOutputArray>();
            List<RotatedRect> licenseBoxList = new List<RotatedRect>();
            List<string> words = _numberDetector.DetectLicensePlate(
               image,
               licensePlateImagesList,
               filteredLicensePlateImagesList,
               licenseBoxList);

            watch.Stop(); //Останавливаем таймер - узнали время выполнения


            Point startPoint = new Point(10, 10);
            string res = "";
            for (int i = 0; i < words.Count; i++)
            {
                Mat dest = new Mat();
                CvInvoke.VConcat(licensePlateImagesList[i], filteredLicensePlateImagesList[i], dest);

                //Показываем то, что получилось
                SaveInFile(String.Format("Номер: {0}", words[i]));

                res = words[i];
                PointF[] verticesF = licenseBoxList[i].GetVertices();
                Point[] vertices = Array.ConvertAll(verticesF, Point.Round);
                using (VectorOfPoint pts = new VectorOfPoint(vertices))
                    CvInvoke.Polylines(image, pts, true, new Bgr(Color.Red).MCvScalar, 2);

            }
            return String.Format("Время распознавания номера : {0} в миллисекундах", watch.Elapsed.TotalMilliseconds); ;

        }

        /// <summary>
        /// Ведет лог в файл, вместе с выводом в консоль.
        /// </summary>
        private static void SaveInFile(string mess)
        {
            try
            {
                string file_name_p = "Car_Numbers_" + DateTime.Now.ToString("yyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                using (StreamWriter writer = new StreamWriter(file_name_p + ".log", true))
                {
                    Console.WriteLine(mess);
                    writer.WriteLine(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss.ffffff") + " :>> " + mess);
                }
            }
            catch
            {
            }
        }

        #endregion



        //https://ru.stackoverflow.com/questions/723412/c-emgu-%d0%bd%d0%b5-%d0%b7%d0%bd%d0%b0%d0%b5%d1%82-%d0%ba%d0%bb%d0%b0%d1%81%d1%81%d1%8b-capture-%d0%b8-haarcascade/723586#723586
        //
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

                //CvInvoke.DrawContours(image,, 1, new Bgr(255, 255, 255));


            }

            //image.ConvertTo(, DepthType.Cv8U);

            //var Face = image.Detect

        }



    }
}
