using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenCV_1stTry
{
    class SimpleShapesDetection
    {


        public static void ShapeDetection()
        {
            ImageViewer viewer = new ImageViewer(); //create an image viewer
            VideoCapture capture = new VideoCapture(); //create a camera captue
            Application.Idle += new EventHandler(delegate (object sender, EventArgs e)
            {  //run this until application closed (close button click on image viewer)
                var frame = capture.QueryFrame(); //draw the image obtained from camera

                Image<Bgr, byte> img = frame.ToImage<Bgr, byte>();

                ////////////////////////////////////
                // Let's find forms in this frame //
                // ****************************** //

                //* StringBuilder msgBuilder = new StringBuilder("Performance: ");



                //Convert the image to grayscale and filter out the noise
                UMat uimage = new UMat();
                CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);

                //use image pyr to remove noise
                UMat pyrDown = new UMat();
                CvInvoke.PyrDown(uimage, pyrDown);
                CvInvoke.PyrUp(pyrDown, uimage);

                //Image<Gray, Byte> gray = img.Convert<Gray, Byte>().PyrDown().PyrUp();

                // ***************** //
                //> circle detection //
                // ***************** //


                //* Stopwatch watch = Stopwatch.StartNew();
                double cannyThreshold = 280.0; // 180.0
                double circleAccumulatorThreshold = 150; //120;
                CircleF[] circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 2.0, 20.0, cannyThreshold, circleAccumulatorThreshold, 5);



                //* watch.Stop();
                //* msgBuilder.Append(String.Format("Hough circles - {0} ms; ", watch.ElapsedMilliseconds));

                // ************************** //
                // > Canny and edge detection //
                // ************************** //

                //* watch.Reset(); watch.Start();
                double cannyThresholdLinking = 120.0;
                UMat cannyEdges = new UMat();
                CvInvoke.Canny(uimage, cannyEdges, cannyThreshold, cannyThresholdLinking);

                LineSegment2D[] lines = CvInvoke.HoughLinesP(
                   cannyEdges,
                   1, //Distance resolution in pixel-related units
                   Math.PI / 45.0, //Angle resolution measured in radians.
                   20, //threshold
                   30, //min Line width
                   10); //gap between lines

                //* watch.Stop();
                //* msgBuilder.Append(String.Format("Canny & Hough lines - {0} ms; ", watch.ElapsedMilliseconds));




                // ****************************** //
                //> Find triangles and rectangles //
                // ****************************** //

                //* watch.Reset(); watch.Start();
                List<Triangle2DF> triangleList = new List<Triangle2DF>();
                List<RotatedRect> boxList = new List<RotatedRect>(); //a box is a rotated rectangle

                FindTrianglesAndRectangles(cannyEdges, triangleList, boxList);

                //* watch.Stop();
                //* msgBuilder.Append(String.Format("Triangles & Rectangles - {0} ms; ", watch.ElapsedMilliseconds));


                // ***************** //
                // * Draw them all * //
                // ***************** //


                //> draw triangles and rectangles

                Image<Bgr, Byte> triangleRectangleImage = img;//.CopyBlank();
                foreach (Triangle2DF triangle in triangleList)
                    triangleRectangleImage.Draw(triangle, new Bgr(Color.DarkBlue), 2);
                foreach (RotatedRect box in boxList)
                    triangleRectangleImage.Draw(box, new Bgr(Color.DarkOrange), 2);

                //> draw circles
                Image<Bgr, Byte> circleImage = img;//.CopyBlank();
                foreach (CircleF circle in circles)
                    circleImage.Draw(circle, new Bgr(Color.Brown), 2);

                //> draw lines
                Image<Bgr, Byte> lineImage = img;//.CopyBlank();
                foreach (LineSegment2D line in lines)
                    lineImage.Draw(line, new Bgr(Color.Green), 2);


                // ********************** //

                //* Console.WriteLine(msgBuilder);
                viewer.Image = img;

            });


            viewer.ShowDialog(); //show the image viewer




        } // ShapeDetection() ////

        private static void FindTrianglesAndRectangles(UMat cannyEdges, List<Triangle2DF> triangleList, List<RotatedRect> boxList)
        {
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(cannyEdges, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
                int count = contours.Size;
                for (int i = 0; i < count; i++)
                {
                    using (VectorOfPoint contour = contours[i])
                    using (VectorOfPoint approxContour = new VectorOfPoint())
                    {
                        CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05, true);
                        if (CvInvoke.ContourArea(approxContour, false) > 250) //only consider contours with area greater than 250
                        {
                            if (approxContour.Size == 3) //The contour has 3 vertices, it is a triangle
                            {
                                Point[] pts = approxContour.ToArray();
                                triangleList.Add(new Triangle2DF(
                                   pts[0],
                                   pts[1],
                                   pts[2]
                                   ));
                            }
                            else if (approxContour.Size == 4) //The contour has 4 vertices.
                            {
                                #region determine if all the angles in the contour are within [80, 100] degree
                                bool isRectangle = true;
                                Point[] pts = approxContour.ToArray();
                                LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                                for (int j = 0; j < edges.Length; j++)
                                {
                                    double angle = Math.Abs(
                                       edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
                                    if (angle < 80 || angle > 100)
                                    {
                                        isRectangle = false;
                                        break;
                                    }
                                }
                                #endregion

                                if (isRectangle) boxList.Add(CvInvoke.MinAreaRect(approxContour));
                            }
                        }
                    }
                }
            }
        } // FindTrianglesAndRectangles() ///



    } // End of class ///
} // namespace ///
