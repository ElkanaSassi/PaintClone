﻿using Client.Network;
using Client.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Client;
using SharedModels;
using SharedModels.CommunicationModels;
using System.Net.Sockets;

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
        private Client.Network.Client client;
        private Button _selectedShapeButton;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                client = new Client.Network.Client();
                SetStatusOnline();

            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                SetStatusOfflineMode();
            }
            finally
            {
                lineButton.Opacity = 1;
                _selectedShapeButton = lineButton;

                //CloseConnection();
            }
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


        private async void LoadCanvasButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //client = new Client.Network.Client();
                //SetStatusOnline();

                var dialog = new LoadDialog(client);
                dialog.Owner = this;

                bool? result = dialog.ShowDialog();

                if (result == true)
                {
                    await client.RequestCanvasLoadAsync(dialog.FileName);

                    _ = client.ReceiveAndRenderCanvasAsync(DrawingCanvas);
                }
                else
                {
                    // LoadDialog was closed
                }
                
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.Message);
                SetStatusOfflineMode();
            }
            //finally
            //{
            //    CloseConnection().Wait();
            //}
        }
    
        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //client = new Client.Network.Client();
                //SetStatusOnline();

                var dialog = new UploadDialog();
                dialog.Owner = this;  

                bool? result = dialog.ShowDialog();

                if (result == true && await client.ValidateFileNameAsync(dialog.FileName))
                {
                
                    MessageBox.Show("File name '" + dialog.FileName + "' was accepted!");

                    var shapes = DrawingCanvas.Children
                    .OfType<System.Windows.Shapes.Shape>() // geting the type names
                    .Select(s => ShapeService.ConvertWpfShapeToData(s)) // for each type geting json presentation
                    .ToList();

                    ResponseInfo response = await client.SendShapesAsync(shapes, dialog.FileName);


                    MessageBox.Show("Saving status from server: " + response.IsSuccess + ".");
                }
                else
                {
                    MessageBox.Show($"File name '" + dialog.FileName + "' was rejected! Try a different name.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                SetStatusOfflineMode();
            }
            //finally
            //{
            //    CloseConnection();
            //}
        }

        private void LineButton_Click(object sender, RoutedEventArgs e)
        {
            selectedShape = "Line";
            HighlightSelectedButton(sender as Button);
        }

        private void RectangleButton_Click(object sender, RoutedEventArgs e)
        {
            selectedShape = "Rectangle";
            HighlightSelectedButton(sender as Button);
        }

        private void CircleButton_Click(object sender, RoutedEventArgs e)
        {
            selectedShape = "Circle";
            HighlightSelectedButton(sender as Button);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearCanvas();
        }

        private void HighlightSelectedButton(Button clickedButton)
        {
            if (_selectedShapeButton != null)
                _selectedShapeButton.Opacity = 0.3;

            _selectedShapeButton = clickedButton;
            _selectedShapeButton.Opacity = 1;
        }

        private void SetStatusOnline()
        {
            StatusDot.Fill = new SolidColorBrush(Colors.Green);
            StatusText.Text = "Server Online";
        }

        private void SetStatusOfflineMode()
        {
            StatusDot.Fill = new SolidColorBrush(Colors.Green);
            StatusText.Text = "Offline Mode";
        }

        private void SetStatusOffline()
        {
            StatusDot.Fill = new SolidColorBrush(Colors.Red);
            StatusText.Text = "Server Offline";
        }
        //private void CloseConnection()
        //{
        //    client.Close();
        //    client = null;
        //    SetStatusOffline();
        //}
    }
}
