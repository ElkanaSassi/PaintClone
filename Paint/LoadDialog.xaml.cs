using Client.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for LoadDialog.xaml
    /// </summary>
    public partial class LoadDialog : Window
    {
        public string FileName { get; private set; }
        private readonly ShapeClient _client;

        public LoadDialog(ShapeClient client)
        {
            InitializeComponent();
            _client = client;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var files = _client.GetStoredFilesInServer();

            FileListBox.ItemsSource = files.Select(f => System.IO.Path.GetFileName(f));
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            string input = (string)FileListBox.SelectedItem;

            FileName = input;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
