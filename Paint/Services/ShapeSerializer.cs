using System.Text.Json;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.IO;
using Client.Models;
using System.Collections.Generic;
using System.Windows;

namespace Client.Services
{
    public static class ShapeSerializer
    {
        
        public static void LoadFromJson(Canvas canvas, List<ShapeData> shapes)
        {
            canvas.Children.Clear();

            if (shapes == null)
            {
                MessageBox.Show("Empty file");
            }

            foreach (var data in shapes)
            {
                Shape shape = null;
                Brush stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.StrokeColor)); // seting the color even tho we using just black. (future proofing)

                // object build
                switch (data.ShapeType)
                {
                    case nameof(Rectangle):
                    {
                        shape = new Rectangle { Width = data.Width, Height = data.Height };
                        Canvas.SetLeft(shape, data.X);
                        Canvas.SetTop(shape, data.Y);
                        break;
                    }
                    case nameof(Ellipse):
                    { 
                        shape = new Ellipse { Width = data.Width, Height = data.Height };
                        Canvas.SetLeft(shape, data.X);
                        Canvas.SetTop(shape, data.Y);
                        break;
                    }
                    case nameof(Line):
                    {
                        shape = new Line
                        {
                            X1 = data.X,
                            Y1 = data.Y,
                            X2 = data.X2,
                            Y2 = data.Y2
                        };
                        break;
                    }
                }

                // object paint
                if (shape != null)
                {
                    shape.Stroke = stroke;
                    shape.StrokeThickness = data.StrokeThickness;
                    canvas.Children.Add(shape);
                }
            }
        }

        public static ShapeData ConvertShapeToData(Shape shape)
        {
            var data = new ShapeData
            {
                ShapeType = shape.GetType().Name,
                StrokeColor = "#000000", // suporting just black for now
                StrokeThickness = shape.StrokeThickness
            };

            if (shape is Line line)
            {
                data.X = line.X1;
                data.Y = line.Y1;
                data.X2 = line.X2;
                data.Y2 = line.Y2;
            }
            else
            {
                data.X = Canvas.GetLeft(shape);
                data.Y = Canvas.GetTop(shape);
                data.Width = shape.Width;
                data.Height = shape.Height;
            }

            return data;
        }

    }
}
