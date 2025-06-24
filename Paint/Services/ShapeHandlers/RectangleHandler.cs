using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Client.Services.ShapeHandlers
{
    public class RectangleHandler : ShapeHandler
    {
        public System.Windows.Shapes.Shape CreateWpfShape(SharedModels.Shapes.Shape data)
        {
            var rectData = (SharedModels.Shapes.Rectangle)data;
            var wpfRectangle = new Rectangle {Width = rectData.Width, Height = rectData.Height};

            Canvas.SetLeft(wpfRectangle, rectData.X);
            Canvas.SetTop(wpfRectangle, rectData.Y);

            return wpfRectangle;
        }

        public SharedModels.Shapes.Shape CreateDataShape(System.Windows.Shapes.Shape wpfShape)
        {
            return new SharedModels.Shapes.Rectangle
            {
                X = Canvas.GetLeft(wpfShape),
                Y = Canvas.GetTop(wpfShape),
                Width = wpfShape.Width,
                Height = wpfShape.Height
            };
        }
    }
}
