using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Network.Server _server = new Network.Server();
        private FileSystemWatcher _watcher;
        //public static MainWindow Instance { get; private set; }
        //public ObservableCollection<string> FileNames { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            //Instance = this;
            //FileNames = new ObservableCollection<string>();
            //FileListBox.ItemsSource = FileNames;

            _ = _server.StartAsync();
        }

        private void Load_ListBox_Files(object sender, RoutedEventArgs e)
        {
            string canvasDir = LocalModels.LocalModels.canvasDirectory;
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, canvasDir);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            var files = Directory.GetFiles(fullPath, "*.json");
            FileListBox.ItemsSource = files.Select(f => System.IO.Path.GetFileName(f)).ToList();

            //LoadFiles(fullPath);

            //_watcher = new FileSystemWatcher(fullPath, "*.json")
            //{
            //    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            //    EnableRaisingEvents = true
            //};

            //_watcher.Created += OnFolderChanged;
            //_watcher.Deleted += OnFolderChanged;
            //_watcher.Renamed += OnFolderChanged;

            //_watcher.EnableRaisingEvents = true;
        }

        //private void OnFolderChanged(object sender, FileSystemEventArgs e)
        //{
        //    Dispatcher.Invoke(() => LoadFiles(_watcher.Path));
        //}

        //private void LoadFiles(string folderPath)
        //{
        //    var files = Directory.GetFiles(folderPath, "*.json");
        //    FileListBox.ItemsSource = files.Select(f => System.IO.Path.GetFileName(f)).ToList();
        //}

        private async void Suspend_Click(object sender, RoutedEventArgs e)
        {
            _ = _server.StopAsync();
        }
    }
}
