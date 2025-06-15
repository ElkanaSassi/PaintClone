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
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for UploadDialog.xaml
    /// </summary>
    public partial class UploadDialog : Window
    {
        public string FileName { get; private set; }

        public UploadDialog()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            string input = FileNameTextBox.Text.Trim();

            if (IsValidFileName(input))
            {
                FileName = input;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Invalid file name. Please enter a valid file name.");
            }
        }

        private bool IsValidFileName(string fileName)
        {
            //TODO: send to the server for validation!

            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
