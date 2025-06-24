using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using SharedModels.Shapes;
using System.Collections.Generic;
using System.Windows;
using Client.Services.ShapeHandlers;

namespace Client.Services
{
    public static class ShapeService
    {
        // all the available shape handlers
        private static readonly Dictionary<string, ShapeHandler> _shapeHandlers = new Dictionary<string, ShapeHandler>
        {
            { nameof(SharedModels.Shapes.Rectangle), new RectangleHandler() },
            { nameof(SharedModels.Shapes.Ellipse), new EllipseHandler() },
            { nameof(SharedModels.Shapes.Line), new LineHandler() }
        };

        public static void LoadShapesToCanvas(Canvas canvas, List<SharedModels.Shapes.Shape> shapes)
        {
            canvas.Children.Clear();

            if (shapes == null)
            {
                MessageBox.Show("Empty file or no shapes to load.");
                return;
            }

            foreach (var data in shapes)
            {
                if (_shapeHandlers.TryGetValue(data.ShapeType, out var handler))
                {
                    System.Windows.Shapes.Shape wpfShape = handler.CreateWpfShape(data);

                    wpfShape.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.StrokeColor));
                    wpfShape.StrokeThickness = data.StrokeThickness;

                    canvas.Children.Add(wpfShape);
                }
            }
        }

        public static SharedModels.Shapes.Shape ConvertWpfShapeToData(System.Windows.Shapes.Shape wpfShape)
        {
            string shapeKey = wpfShape.GetType().Name;

            if (_shapeHandlers.TryGetValue(shapeKey, out var handler))
            {
                var dataShape = handler.CreateDataShape(wpfShape);

                dataShape.ShapeType = shapeKey;
                dataShape.StrokeColor = "#000000"; // supporting just black for now
                dataShape.StrokeThickness = wpfShape.StrokeThickness;

                return dataShape;
            }

            return null;
        }
    }
}
