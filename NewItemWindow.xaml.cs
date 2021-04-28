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
using System.IO;
using System.Text.RegularExpressions;


namespace SimpleApplicationWPF
{
    /// <summary>
    /// Product logic for the NewItemWindow.xaml class
    /// </summary>
    public partial class NewItemWindow : Window
    {
        protected string selectedItemPath = null;
        protected TreeViewItem selectedItem = null;
        public NewItemWindow(TreeViewItem __selectedItem, string __path)
        {
            InitializeComponent();
            this.Owner = App.Current.MainWindow;
            this.selectedItemPath = __path;
            this.selectedItem = __selectedItem;
        }

        private void  Ok_Click (object sender, RoutedEventArgs e)
        {
            //Checking file name
            if ((bool)typeFile.IsChecked && !Regex.IsMatch(newItemName.Text, @"^[a-zA-Z0-9_~-]{1,8}(.txt|.php|.html)$"))
            {
                string msg = "Zmień nazwę!";
                MessageBox.Show(msg, "Błąd!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if ((bool)typeDirectory.IsChecked && newItemName.Text == "")
            {
                string msg = "Brak nazwy!";
                MessageBox.Show(msg, "Błąd!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            } 
            else
            {
                string newName = newItemName.Text;
                string newPath = selectedItemPath + @"\" + newName;

                if (File.Exists(newPath))
                {
                    string msg = "Zmień nazwę! Plik już istnieje!";
                    MessageBox.Show(msg, "Błąd!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (Directory.Exists(newPath))
                {
                    string msg = "Zmień nazwę! Folder już istnieje!";
                    MessageBox.Show(msg, "Błąd!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    FileAttributes attributesNewFile = SetAttributes();

                    if ((bool)typeFile.IsChecked)
                    {
                        File.Create(newPath);
                    }
                    else if ((bool)typeDirectory.IsChecked)
                    {
                        Directory.CreateDirectory(newPath);
                    }

                    File.SetAttributes(newPath, attributesNewFile);

                    if ((bool)typeFile.IsChecked)
                    {
                        var item = new TreeViewItem
                        {
                            Header = newItemName.Text,
                            Tag = newPath,
                            ContextMenu = ((MainWindow)Application.Current.MainWindow).FindResource("ContextMenuFile") as System.Windows.Controls.ContextMenu
                        };
                        selectedItem.Items.Add(item);
                    }
                    else if ((bool)typeDirectory.IsChecked)
                    {
                        var item = new TreeViewItem
                        {
                            Header = newItemName.Text,
                            Tag = newPath,
                            ContextMenu = ((MainWindow)Application.Current.MainWindow).FindResource("ContextMenuDirectory") as System.Windows.Controls.ContextMenu
                        };
                        selectedItem.Items.Add(item);
                    }
                    this.Close();
                }
            }
        }

        private FileAttributes SetAttributes()
        {
            FileAttributes attributesNewFile = FileAttributes.Normal;
            if ((bool)readOnly.IsChecked)
            {
                attributesNewFile |= FileAttributes.ReadOnly; 
            }
            if ((bool)archive.IsChecked)
            {
                attributesNewFile |= FileAttributes.Archive;
            }
            if ((bool)hidden.IsChecked)
            {
                attributesNewFile |= FileAttributes.Hidden;
            }
            if ((bool)system.IsChecked)
            {
                attributesNewFile |= FileAttributes.System;
            }
            return attributesNewFile;
        }
    }
}
