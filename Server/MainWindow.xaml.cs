using Server.Network;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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
            string canvasDir = LocalModels.LocalModels.canvasDirectory;
            string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, canvasDir);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            var files = Directory.GetFiles(fullPath, "*.json");
            FileListBox.ItemsSource = files.Select(f => System.IO.Path.GetFileName(f));

        }

        private async Task StartServerAsync(CancellationToken token)
        {
            try
            {
                var server = new ShapeServer();
                await server.StartAsync();

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
