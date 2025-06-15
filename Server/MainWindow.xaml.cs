using Client.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
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
using static System.Net.WebRequestMethods;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Task _serverTask;

        public MainWindow()
        {
            InitializeComponent();

            _cancellationTokenSource = new CancellationTokenSource();
            _serverTask = StartServerAsync(_cancellationTokenSource.Token);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string _canvasDirectory = "canvasSotredInServer";

            if (!Directory.Exists(_canvasDirectory))
            {
                Directory.CreateDirectory(_canvasDirectory);
            }

            var files = Directory.GetFiles(_canvasDirectory, "*.json");

            FileListBox.ItemsSource = files.Select(f => System.IO.Path.GetFileName(f));


        }

        private async Task StartServerAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    Console.WriteLine("Server running...");
                    await Task.Delay(1000, token); // checks for CancellationToken every 1 second.
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Server task cancelled.");
            }
            finally
            {
                Console.WriteLine("Server stopped.");
            }
        }

        private async void Suspend_Click(object sender, RoutedEventArgs e)
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                await _serverTask; // wait for the server to fully stop before disposing
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
                _serverTask = null;
            }
        }
    }
}
