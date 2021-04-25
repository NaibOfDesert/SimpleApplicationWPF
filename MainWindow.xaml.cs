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
using System.Windows.Forms;
using System.IO;

namespace SimpleApplicationWPF
{
    /// <summary>
    /// Product logic for the MainWindow.xaml class
    /// </summary>
    public partial class MainWindow : Window
    {
        // Mian Window initialization
        public MainWindow()
        {
            InitializeComponent();
        }

        // Open directory browser
        void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog() { Description = "Wybierz folder" };
            dlg.ShowDialog();

            if (dlg.SelectedPath != null && dlg.SelectedPath.Length != 0)
            {
                DirectoryInfo path = new DirectoryInfo(dlg.SelectedPath);

                TreeView.Items.Clear();
                TreeView.Items.Add(NewTreeItem(path));
            }
            else return;
        }

        // Support for closing a window with a field button
        void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            // Close this window
            this.Close();
        }

        // Creating new element of TreeView with root
        public TreeViewItem NewTreeItem(DirectoryInfo __path)
        {
            var root = new TreeViewItem
            {
                Header = __path.Name,
                Tag = __path.FullName,
                ContextMenu = FindResource("ContextMenuDirectory") as System.Windows.Controls.ContextMenu,
            };

            try
            {
                foreach (var nextDirectory in __path.GetDirectories())
                {
                    root.Items.Add(NewTreeItem(nextDirectory));
                }
                foreach (var file in __path.GetFiles())
                {
                    var item = new TreeViewItem
                    {
                        Header = file.Name,
                        Tag = file.FullName,
                        ContextMenu = FindResource("ContextMenuFile") as System.Windows.Controls.ContextMenu,
                    };
                    root.Items.Add(item);
                }
            }
            catch (System.UnauthorizedAccessException exc)
            {
                Console.WriteLine(exc.Message);
            }

            return root;
        }

        // Opening file data as a text
        void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            if (TreeView.SelectedItem != null)
            {
                TreeViewItem selectedFile = (TreeViewItem)TreeView.SelectedItem; //object -> TreeViewItem => explicit conversion
                string selectedFilePath = (string)selectedFile.Tag; //object -> string => explicit conversion
                StreamReader streamReader = new StreamReader(selectedFilePath);

                FileData.Text = streamReader.ReadToEnd();
            }
            else return;
        }
        void FileDelete_Click(object sender, RoutedEventArgs e)
        {
            if (TreeView.SelectedItem != null)
            {
                TreeViewItem selectedFile = (TreeViewItem)TreeView.SelectedItem; //object -> TreeViewItem => explicit conversion
                string selectedFilePath = (string)selectedFile.Tag; //object -> string => explicit conversion
                FileInfo file = new FileInfo(selectedFilePath);

                if (file.FindAttribute(FileAttributes.ReadOnly))
                    file.DeleteAttribute(FileAttributes.ReadOnly);

                try
                {
                    File.Delete(selectedFilePath);
                }
                catch (System.UnauthorizedAccessException exc)
                {
                    Console.WriteLine(exc.Message);
                    return;
                }

                TreeViewItem parent = (TreeViewItem)selectedFile.Parent; //object -> TreeViewItem => explicit conversion
                parent.Items.Remove(TreeView.SelectedItem);
            }
            else return;
        }

        void  DirectoryCreate_Click(object sender, RoutedEventArgs e)
        {
            if (TreeView.SelectedItem != null)
            {
                NewItemWindow newItem = new NewItemWindow();
                newItem.ShowDialog();
            } 
            else return;
        }

        void DirectoryDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        // Event handling
        void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<Object> e)
        {

        }


    }
}
