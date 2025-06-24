using System.Windows.Controls;
using System.Windows.Shapes;

namespace Client.Services.ShapeHandlers
{
    public class EllipseHandler : ShapeHandler
    {
        public System.Windows.Shapes.Shape CreateWpfShape(SharedModels.Shapes.Shape data)
        {
            var ellipseData = (SharedModels.Shapes.Ellipse)data;
            var wpfEllipse = new Ellipse {Width = ellipseData.Width, Height = ellipseData.Height};

            Canvas.SetLeft(wpfEllipse, ellipseData.X);
            Canvas.SetTop(wpfEllipse, ellipseData.Y);

            return wpfEllipse;
        }

        public SharedModels.Shapes.Shape CreateDataShape(System.Windows.Shapes.Shape wpfShape)
        {
            return new SharedModels.Shapes.Ellipse
            {
                X = Canvas.GetLeft(wpfShape),
                Y = Canvas.GetTop(wpfShape),
                Width = wpfShape.Width,
                Height = wpfShape.Height
            };
        }
    }
}
