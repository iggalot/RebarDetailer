using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VMDiagrammer.Helpers
{

    /// <summary>
    /// Enum for types of arrows (both straight and circular)
    /// </summary>
    public enum ArrowDirections
    {
        ARROW_UP = 0,
        ARROW_RIGHT = 1,
        ARROW_DOWN = 2,
        ARROW_LEFT = 3,
        ARROW_CLOCKWISE = 4,            // circular
        ARROW_COUNTERCLOCKWISE = 5      // circular
    }

    /// <summary>
    /// Enum for positioning of text relative to an object.
    /// </summary>
    public enum TextPositions
    {
        TEXT_ABOVE = 0,
        TEXT_BELOW = 1,
        TEXT_LEFT = 2,
        TEXT_RIGHT = 3
    }

    
    public enum Linetypes
    {
        LINETYPE_SOLID = 0,
        LINETYPE_DASHED = 1,
        LINETYPE_DASHED_X2 = 2,
        LINETYPE_CENTER = 3,
        LINETYPE_CENTER_X2 = 4,
        LINETYPE_PHANTOM = 5,
        LINETYPE_PHANTOM_X2 = 6
            
    }

    /// <summary>
    /// A class for drawing shapes onto a WPF canvas
    /// </summary>
    public class DrawingHelpers
    {
        // Constants used by the drawing helpers -- unless overridden in the functon call
        public const double DEFAULT_ARROW_SHAFTLENGTH = 20;    // arrow shaft length
        public const double DEFAULT_ARROW_HEADLENGTH = 8;      // arrow head length
        public const double DEFAULT_ARROW_THICKNESS = 3;       // line thickness of arrow components
        public const double DEFAULT_TEXT_HEIGHT = 12.0;        // text height (in pixels)

        private static DoubleCollection GetStrokeDashArray(Linetypes ltype)
        {
            switch (ltype)
            {
                case Linetypes.LINETYPE_CENTER:
                    return new DoubleCollection() { 4, 2 };
                case Linetypes.LINETYPE_CENTER_X2:
                    return new DoubleCollection() { 8, 4 };
                case Linetypes.LINETYPE_SOLID:
                    return new DoubleCollection() { };
                case Linetypes.LINETYPE_DASHED:
                    return new DoubleCollection() { 4 };
                case Linetypes.LINETYPE_DASHED_X2:
                    return new DoubleCollection() { 8 };
                case Linetypes.LINETYPE_PHANTOM:
                    return new DoubleCollection() { 10, 2, 4, 2, 4, 2 };
                case Linetypes.LINETYPE_PHANTOM_X2:
                    return new DoubleCollection() { 20, 4, 8, 4, 8, 4 };
                default:
                    return new DoubleCollection() { };
            }
        }

        /// <summary>
        /// Draws a basic circle (ellipse) on a WPF canvas
        /// </summary>
        /// <param name="c">the WPF canvas object</param>
        /// <param name="x">the upper left x-coordinate for a bounding box around the node</param>
        /// <param name="y">the upper left y-coordinate for a bounding box around the node</param>
        /// <returns></returns>
        public static Shape DrawCircle(Canvas c, double x, double y, Brush fill, Brush stroke, double radius, double thickness, Linetypes ltype = Linetypes.LINETYPE_SOLID)
        {
            // Draw circle node
            Ellipse myEllipse = new Ellipse();
            myEllipse.Fill = fill;
            myEllipse.Stroke = stroke;
            myEllipse.StrokeThickness = 2.0;
            myEllipse.StrokeDashArray = GetStrokeDashArray(ltype);

            myEllipse.Width = radius;
            myEllipse.Height = radius;

            Canvas.SetLeft(myEllipse, x - myEllipse.Width / 2.0);
            Canvas.SetTop(myEllipse, y - myEllipse.Height / 2.0);

            c.Children.Add(myEllipse);

            return myEllipse;
        }

        public static Shape DrawCircleHollow(Canvas c, double x, double y, Brush stroke, double radius, double thickness=1.0, Linetypes ltype = Linetypes.LINETYPE_SOLID)
        {
            return DrawCircle(c, x, y, Brushes.Transparent, stroke, radius, thickness, ltype);
        }

        /// <summary>
        /// Draws a basic line object on a WPF canvas
        /// </summary>
        /// <param name="c">the WPF canvas object</param>
        /// <param name="ex">end point x-coord</param>
        /// <param name="ey">end point y-coord</param>
        /// <param name="sx">start point x_coord</param>
        /// <param name="sy">start point y-coord</param>
        /// <param name="stroke">color of the line object as a <see cref="Brush"/></param>
        /// <returns>the Shape object</returns>
        public static Shape DrawLine(Canvas c, double sx, double sy, double ex, double ey, Brush stroke, double thickness=1.0, Linetypes ltype=Linetypes.LINETYPE_SOLID)
        {
            Line myLine = new Line();
            myLine.Stroke = stroke;
            myLine.StrokeThickness = thickness;
            myLine.StrokeDashArray = GetStrokeDashArray(ltype);
            myLine.X1 = sx;
            myLine.Y1 = sy;
            myLine.X2 = ex;
            myLine.Y2 = ey;

            c.Children.Add(myLine);

            return myLine;
        }

        /// <summary>
        /// Draws a circular arc on a WPF canvas object
        /// Angles are measured as positive - clockwise.
        /// </summary>
        /// <param name="c">canvas to draw on</param>
        /// <param name="x">x-coord for center of circlular arc</param>
        /// <param name="y">y-coord for center of circular arc</param>
        /// <param name="fill">the fill color for the arc -- usually transparent</param>
        /// <param name="stroke">the stroke line color of the arc</param>
        /// <param name="radius">radius of the ciruclar arrow</param>
        /// <param name="thickness">stroke thickness of the arrow</param>
        /// <param name="end_angle">angle from center to the end of the arc (clockwise positive)/param>
        /// <param name="start_angle">angle from center to the start of the arc (clockwise positive)/param>
        /// <param name="head_len">length of the arrow head in pixels</param>
        public static void DrawCircularArc(Canvas c, double x, double y, Brush fill, Brush stroke, double thickness, double radius, double start_angle, double end_angle)
        {
            double sa, ea;

            Path path = new Path();
            path.Fill = fill; ;
            path.Stroke = stroke;
            path.StrokeThickness = thickness;
            Canvas.SetLeft(path, 0);
            Canvas.SetTop(path, 0);

            sa = ((start_angle % (Math.PI * 2)) + Math.PI * 2) % (Math.PI * 2);
            ea = ((end_angle % (Math.PI * 2)) + Math.PI * 2) % (Math.PI * 2);

            if(ea < sa)
            {
                double temp_angle = ea;
                ea = sa;
                sa = ea;
            }

            double angle_diff = ea - sa;

            PathGeometry pg = new PathGeometry();
            PathFigure pf = new PathFigure();
            
            ArcSegment arcSegment = new ArcSegment();
            arcSegment.IsLargeArc = angle_diff >= Math.PI;

            // Set the start of arc
            pf.StartPoint = new System.Windows.Point(x - radius * Math.Cos(sa), y - radius * Math.Sin(sa));

            // Set the end point of the arc
            arcSegment.Point = new System.Windows.Point(x - radius * Math.Cos(ea), y - radius * Math.Sin(ea));

            arcSegment.Size = new System.Windows.Size(0.8*radius, radius);
            arcSegment.SweepDirection = SweepDirection.Counterclockwise;

            pf.Segments.Add(arcSegment);
            pg.Figures.Add(pf);
            path.Data = pg;

            c.Children.Add(path);

            return;
        }

        /// <summary>
        /// Helper function to draw text on a WPF canvas
        /// </summary>
        /// <param name="c">canvas to draw on</param>
        /// <param name="x">x-coord</param>
        /// <param name="y">y-coord</param>
        /// <param name="z">z-coord (0 for 2D)</param>
        /// <param name="str">text string</param>
        /// <param name="brush">color of the text</param>
        /// <param name="size">size of the text</param>
        public static void DrawText(Canvas c, double x, double y, double z, string str, Brush brush, double size)
        {
            double xpos = x;
            double ypos = y;
            double zpos = z;

            if (string.IsNullOrEmpty(str))
                return;
            // Draw text
            TextBlock textBlock = new TextBlock();
            textBlock.Text = str;
            textBlock.FontSize = size;
            textBlock.Foreground = brush;

            Canvas.SetLeft(textBlock, xpos);
            Canvas.SetTop(textBlock, ypos);

            c.Children.Add(textBlock);
        }


        /// <summary>
        /// A helper function to draw an arrow in a specified direction on a WPF canvas
        /// <param name="c">canvas to draw on</param>
        /// <param name="x">x-coord</param>
        /// <param name="y">y-coord</param>
        /// <param name="fill">fill color --- usually transparent</param>
        /// <param name="stroke">color of the arrow</param>
        /// <param name="dir">enum for the direction of the arroe <see cref="ArrowDirections"/></param>
        /// <param name="thickness">stroke thickness of the arrow</param>
        /// <param name="shaft_len">length of the arrow shaft in pixels</param>
        /// <param name="head_len">length of the arrow head in pixels</param>
        public static void DrawArrow(Canvas c, double x, double y, Brush fill, Brush stroke, ArrowDirections dir, double thickness, double shaft_len=DEFAULT_ARROW_SHAFTLENGTH, double head_len = DEFAULT_ARROW_HEADLENGTH)
        {
            switch (dir)    
            {
                case ArrowDirections.ARROW_DOWN:
                    DrawArrowDown(c, x, y, fill, stroke, thickness, shaft_len, head_len);
                    break;
                case ArrowDirections.ARROW_UP:
                    DrawArrowUp(c, x, y, fill, stroke, thickness, shaft_len, head_len);
                    break;
                case ArrowDirections.ARROW_RIGHT:
                case ArrowDirections.ARROW_LEFT:
                default:
                    throw new NotImplementedException("draw function not defined for arrow of direction = " + dir);
            }
        }

        /// <summary>
        /// A helper function to draw downward arrow
        /// <param name="c">canvas to draw on</param>
        /// <param name="x">x-coord</param>
        /// <param name="y">y-coord</param>
        /// <param name="fill">fill color --- usually transparent</param>
        /// <param name="stroke">color of the arrow</param>
        /// <param name="dir">enum for the direction of the arroe <see cref="ArrowDirections"/></param>
        /// <param name="thickness">stroke thickness of the arrow</param>
        /// <param name="shaft_len">length of the arrow shaft in pixels</param>
        /// <param name="head_len">length of the arrow head in pixels</param>
        public static void DrawArrowDown(Canvas c, double x, double y, Brush fill, Brush stroke, double thickness, double shaft_len, double head_len)
        {
            DrawLine(c, x, y, x, y - shaft_len, stroke, thickness);
            DrawLine(c, x, y, x - head_len, y - head_len, stroke, thickness);
            DrawLine(c, x, y, x + head_len, y - head_len, stroke, thickness);
        }


        /// <summary>
        /// A helper function to draw an upward arrow in a specified direction
        /// <param name="c">canvas to draw on</param>
        /// <param name="x">x-coord</param>
        /// <param name="y">y-coord</param>
        /// <param name="fill">fill color --- usually transparent</param>
        /// <param name="stroke">color of the arrow</param>
        /// <param name="dir">enum for the direction of the arroe <see cref="ArrowDirections"/></param>
        /// <param name="thickness">stroke thickness of the arrow</param>
        /// <param name="shaft_len">length of the arrow shaft in pixels</param>
        /// <param name="head_len">length of the arrow head in pixels</param>
        public static void DrawArrowUp(Canvas c, double x, double y, Brush fill, Brush stroke, double thickness, double shaft_len, double head_len)
        {
            DrawLine(c, x, y, x, y + shaft_len, stroke, thickness);
            DrawLine(c, x, y, x - head_len, y + head_len, stroke, thickness);
            DrawLine(c, x, y, x + head_len, y + head_len, stroke, thickness);
        }

        /// <summary>
        /// Draws a circular arrow in a particular direction
        /// Angles are measured as positive - clockwise.
        /// </summary>
        /// <param name="c">canvas to draw on</param>
        /// <param name="x">x-coord for center of circlular arc</param>
        /// <param name="y">y-coord for center of circular arc</param>
        /// <param name="dir">enum for the direction of the circular arrow <see cref="ArrowDirections"/></param>
        /// <param name="radius">radius of the ciruclar arrow</param>
        /// <param name="thickness">stroke thickness of the arrow</param>
        /// <param name="end_angle">angle from center to the end of the arc (clockwise positive)/param>
        /// <param name="start_angle"/>angle from center to the start of the arc (clockwise positive)/param>
        /// <param name="head_len">length of the arrow head in pixels</param>
        public static void DrawCircularArrow(Canvas c, double x, double y, Brush fill, Brush stroke, 
            ArrowDirections dir, double thickness = DEFAULT_ARROW_THICKNESS, 
            double radius = 32.0, double start_angle = Math.PI / 2.0, double end_angle = (-1) * Math.PI / 2.0, 
            double head_len = DEFAULT_ARROW_HEADLENGTH)
        {
            double s_x, s_y;
            double e_x, e_y;

            // Ensure that the angles are between zero and 2 x pi
            double sa = ((start_angle % (Math.PI * 2)) + Math.PI * 2) % (Math.PI * 2);
            double ea = ((end_angle % (Math.PI * 2)) + Math.PI * 2) % (Math.PI * 2);

            // switch the end and start angle if they are outsize the zero to 2x pi range.
            if (ea < sa)
            {
                double temp_angle = ea;
                ea = sa;
                sa = ea;
            }

            // Draw the circular arc
            DrawingHelpers.DrawCircularArc(c, x, y, fill, stroke, thickness,radius,sa, ea);

            // Draw the arrow head
            s_x = x - radius * Math.Cos(sa);
            s_y = y - radius * Math.Sin(sa);
            e_x = x - radius * Math.Cos(ea);
            e_y = y - radius * Math.Sin(ea);

            if(dir == ArrowDirections.ARROW_COUNTERCLOCKWISE)
            {
                // use the endpoint to draw the head
                DrawLine(c, e_x, e_y, e_x - head_len, e_y - head_len, stroke, thickness);
                DrawLine(c, e_x, e_y, e_x - head_len, e_y + head_len, stroke, thickness);
            }
            else
            {
                // use the startpoint to draw the head
                DrawLine(c, s_x, s_y, s_x - head_len, s_y - head_len, stroke, thickness);
                DrawLine(c, s_x, s_y, s_x - head_len, s_y + head_len, stroke, thickness);
            }
        }
    }
}
