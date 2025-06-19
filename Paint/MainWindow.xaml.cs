using Client.Network;
using Client.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharedModels;
using System.Windows.Controls.Primitives;
using Client;
using System.Text.Json;

namespace Paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string selectedShape = "Line";
        private Point startPoint;
        private Shape currentShape;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(DrawingCanvas);

            switch (selectedShape)
            {
                case "Line":
                {
                    currentShape = new Line
                    {
                        X1 = startPoint.X,
                        Y1 = startPoint.Y,
                        X2 = startPoint.X,
                        Y2 = startPoint.Y,
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    };
                    break;
                }

                case "Rectangle":
                { 
                    currentShape = new Rectangle
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    };
                    Canvas.SetLeft(currentShape, startPoint.X);
                    Canvas.SetTop(currentShape, startPoint.Y);
                    break;
                }

                case "Circle":
                {
                    currentShape = new Ellipse
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    };
                    Canvas.SetLeft(currentShape, startPoint.X);
                    Canvas.SetTop(currentShape, startPoint.Y);
                    break;
                }
            }

            if (currentShape != null)
            {
                DrawingCanvas.Children.Add(currentShape);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && currentShape != null)
            {
                var pos = e.GetPosition(DrawingCanvas);

                if (currentShape is Line line)
                {
                    line.X2 = pos.X;
                    line.Y2 = pos.Y;
                }
                else
                {
                    double width = Math.Abs(pos.X - startPoint.X);
                    double height = Math.Abs(pos.Y - startPoint.Y);

                    currentShape.Width = width;
                    currentShape.Height = height;

                    Canvas.SetLeft(currentShape, Math.Min(pos.X, startPoint.X));
                    Canvas.SetTop(currentShape, Math.Min(pos.Y, startPoint.Y));
                }
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            currentShape = null;
        }

        private void ClearCanvas()
        {
            DrawingCanvas.Children.Clear();
        }


        private void LoadCanvasButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: give the clinet the file names from the server to choose from.


            var jsonString = ""; // get the json presentation from server.
            var shapes = JsonSerializer.Deserialize<List<ShapeData>>(jsonString);

            ShapeSerializer.LoadFromJson(DrawingCanvas, shapes);

        }

        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new UploadDialog();
            dialog.Owner = this;  

            bool result = (bool)dialog.ShowDialog();

            // checking file name validation 
            if (result == true)
            {
                
                MessageBox.Show("File name '" + dialog.FileName + "' was accepted!");

                var shapes = DrawingCanvas.Children
                .OfType<System.Windows.Shapes.Shape>() // geting the type names
                .Select(s => ShapeSerializer.ConvertShapeToData(s)) // for each type geting json presentation
                .ToList();

                // TODO: make the massage contain all the shapes with the valid file name.

                var client = new ShapeClient();
                await client.SendShapesAsync(shapes); // TODO: need fixing.

                MessageBox.Show("Shapes sent to server.");
            }
            else
            {
                MessageBox.Show($"File name '" + dialog.FileName + "' was rejected! Try a different name.");
            }

        }

        private void LineButton_Click(object sender, RoutedEventArgs e) => selectedShape = "Line";
        private void RectangleButton_Click(object sender, RoutedEventArgs e) => selectedShape = "Rectangle";
        private void CircleButton_Click(object sender, RoutedEventArgs e) => selectedShape = "Circle";
        private void ClearButton_Click(object sender, RoutedEventArgs e) => ClearCanvas();
    }
}
