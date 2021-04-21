using GlycanGUI.Algorithm.CurveFitting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GlycoGUIApp
{
    /// <summary>
    /// Interaction logic for CurveWindow.xaml
    /// </summary>
    public partial class CurveWindow : Window
    {
        public CurveWindow(NormalizerEngine engine)
        {
            InitializeComponent();

            // set title
            coefficient.Text = Math.Round(engine.Coefficient(), 4).ToString();
            CreateEquation(engine.Fitter as PolynomialFitting);
            //// draw curve
            canvas.Children.Add(new VisualHost
            { Visual = CreateCurveDrawingVisual(engine) });

        }

        private Run SuperscriptRun(string text)
        {
            Run run = new Run(text);
            run.BaselineAlignment = BaselineAlignment.Superscript;
            return run;
        }

        private void CreateEquation(PolynomialFitting fitting = null)
        {
            if (fitting == null)
                return;
            double[] param = fitting.Parameter;
            equation.Inlines.Add(" ");
            equation.Inlines.Add(Math.Round(param[3], 2).ToString());
            equation.Inlines.Add("x");
            equation.Inlines.Add(SuperscriptRun("3"));
            if (param[2] > 0)
                equation.Inlines.Add("+");
            equation.Inlines.Add(Math.Round(param[2], 2).ToString());
            equation.Inlines.Add("x");
            equation.Inlines.Add(SuperscriptRun("2"));
            if (param[1] > 0)
                equation.Inlines.Add("+");
            equation.Inlines.Add(Math.Round(param[1], 2).ToString());
            equation.Inlines.Add("x");
            if (param[0] > 0)
                equation.Inlines.Add("+");
            equation.Inlines.Add(Math.Round(param[0], 2).ToString());
        }


        private DrawingVisual CreateCurveDrawingVisual(
           NormalizerEngine engine)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            // Create a drawing and draw it in the DrawingContext.
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                Pen pen = new Pen(Brushes.Black, 1);
                Rect rect =
                    new Rect(new Point(40, 40), new Point(660, 300)); // range of (600, 260)
                drawingContext.DrawRectangle(Brushes.Transparent, pen, rect);
                List<double> x = engine.Retention;
                List<double> y = engine.Guis;

                // draw points
                Pen p = new Pen(Brushes.Blue, 3);
                double max_x = Math.Ceiling(x.Max() / 10) * 10;
                for (int i = 0; i < x.Count; i++)
                {
                    double x1 = x[i] * 600.0 / max_x + 40;
                    double y1 = 300.0 - y[i] * 20.0;
                    Point point = new Point(x1, y1);
                    drawingContext.DrawEllipse(Brushes.Black, p, point, 1, 1);
                }

                // draw xtics per 5 min
                Pen line = new Pen(Brushes.Black, 1);
                double max_min = Math.Ceiling(max_x / 5) * 5;
                for (int i = 5; i <= max_min; i += 5)
                {
                    double x1 = i * 600.0 / max_x + 40;
                    double y1 = 300;
                    double y2 = 290;
                    double y3 = 310;
                    Point p1 = new Point(x1, y1);
                    Point p2 = new Point(x1, y2);
                    Point p3 = new Point(x1 - 5, y3);
                    drawingContext.DrawLine(line, p1, p2);
                    if (i % 2 == 0)
                    {
                        FormattedText formattedText = new FormattedText(
                            i.ToString(),
                            CultureInfo.GetCultureInfo("en-us"),
                            FlowDirection.LeftToRight,
                            new Typeface("Times New Roman"),
                            11,
                            Brushes.Black);
                        drawingContext.DrawText(formattedText, p3);
                    }
                }
                FormattedText x_title = new FormattedText(
                            "Time (min)",
                            CultureInfo.GetCultureInfo("en-us"),
                            FlowDirection.LeftToRight,
                            new Typeface("Times New Roman"),
                            11,
                            Brushes.Black);
                drawingContext.DrawText(x_title, new Point(320, 330));

                // draw ytics per 1 unit
                for (int i = 1; i <= 12; i++)
                {
                    double x1 = 40;
                    double x2 = 50;
                    double x3 = 26;
                    double y1 = 300.0 - i * 20.0;
                    Point p1 = new Point(x1, y1);
                    Point p2 = new Point(x2, y1);
                    Point p3 = new Point(x3, y1 - 5);
                    drawingContext.DrawLine(line, p1, p2);
                    if (i % 2 == 0)
                    {
                        FormattedText formattedText = new FormattedText(
                            i.ToString(),
                            CultureInfo.GetCultureInfo("en-us"),
                            FlowDirection.LeftToRight,
                            new Typeface("Times New Roman"),
                            11,
                            Brushes.Black);
                        drawingContext.DrawText(formattedText, p3);
                    }
                }

                FormattedText y_title = new FormattedText(
                            "Units",
                            CultureInfo.GetCultureInfo("en-us"),
                            FlowDirection.LeftToRight,
                            new Typeface("Times New Roman"),
                            11,
                            Brushes.Black);
                drawingContext.DrawText(y_title, new Point(10, 40));


                // draw curve
                List<Point> points = new List<Point>();
                Pen curve = new Pen(Brushes.Red, 1);
                double min_x = Math.Floor(x.Min());
                for (double i = min_x; i <= max_min; i += 0.01)
                {
                    double x1 = i * 600 / max_x + 40;
                    double y1 = 300.0 - engine.Normalize(i) * 20.0;
                    points.Add(new Point(x1, y1));
                }
                for (int i = 0; i < points.Count - 1; i++)
                {
                    drawingContext.DrawLine(curve, points[i], points[i + 1]);
                }

            }
            return drawingVisual;
        }
    }

    public class VisualHost : UIElement
    {
        public Visual Visual { get; set; }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0)
                throw new ArgumentOutOfRangeException("index");
            return Visual;
        }
    }
}
