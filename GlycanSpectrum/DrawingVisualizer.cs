using GlycanQuant.Engine.Search;
using GlycanQuant.Spectrum.Process.PeakPicking;
using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace GlycanSpectrum
{
    public class DrawingVisualizer
    {
        private int BinarySearchPoints(List<double> peaks, double mz)
        {
            int start = 0;
            int end = peaks.Count - 1;

            while (start + 1 < end)
            {
                int mid = end + (start - end) / 2;
                if (Math.Abs(peaks[mid] - mz) < 0.001)
                {
                    return mid;
                }
                else if (peaks[mid] > mz)
                {
                    end = mid - 1;
                }
                else
                {
                    start = mid;
                }
            }

            return start;
        }


        public DrawingVisual CreateDrawingVisual(ISpectrum spectrum, List<IResult> results,
            double x1 = 40, double y1 = 40, double x2 = 660, int y2 = 300)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            // Create a drawing and draw it in the DrawingContext.
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                double delta = 10;
                Pen pen = new Pen(Brushes.Black, 1);
                Rect rect =
                    new Rect(new Point(x1 - delta, y1 - delta),
                    new Point(x2 + delta, y2 + delta)); // range of (600, 260)
                drawingContext.DrawRectangle(Brushes.Transparent, pen, rect);

                // process peaks
                List<IPeak> peaks = spectrum.GetPeaks();

                double deltaX = x2 - x1;
                double deltaY = y2 - y1;
                double minX = peaks.Min(p => p.GetMZ());
                double maxX = peaks.Max(p => p.GetMZ());
                List<double> x = peaks.Select(p => (p.GetMZ() - minX) / (maxX - minX) * deltaX + x1).ToList();
                double maxY = peaks.Max(p => p.GetIntensity());
                List<double> y = peaks.Select(p => y2 - p.GetIntensity() / maxY * deltaY).ToList();

                // draw curve
                List<Point> points = new List<Point>();
                for (int i = 0; i < x.Count; i++)
                {
                    points.Add(new Point(x[i], y[i]));
                }


                Pen curve = new Pen(Brushes.Blue, 1);
                for (int i = 0; i < points.Count - 1; i++)
                {
                    drawingContext.DrawLine(curve, points[i], points[i + 1]);
                }

                // draw points
                Pen p = new Pen(Brushes.Red, 3);
                List<double> mzList = results.Select(r => r.GetMZ()).ToList();

                for (int i = 0; i < mzList.Count; i++)
                {
                    double xi = (mzList[i] - minX) / (maxX - minX) * deltaX + x1;
                    double yi = y[BinarySearchPoints(mzList, xi)];
                    Point point = new Point(xi, yi);
                    drawingContext.DrawEllipse(Brushes.Red, p, point, 0.5, 0.5);
                }


            }
            return drawingVisual;
        }
    }
}
