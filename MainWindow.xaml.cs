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

        // ContextMenu - opening file data as a text
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

        // ContextMenu - deleting file
        void FileDelete_Click(object sender, RoutedEventArgs e)
        {
            if (TreeView.SelectedItem != null)
            {
                TreeViewItem selectedFile = (TreeViewItem)TreeView.SelectedItem; //object -> TreeViewItem => explicit conversion
                string selectedFilePath = (string)selectedFile.Tag;
                FileInfo selectedFileInfo = new FileInfo(selectedFilePath);

                FileDelete(selectedFileInfo);

                TreeViewItem parent = (TreeViewItem)selectedFile.Parent; //object -> TreeViewItem => explicit conversion
                parent.Items.Remove(TreeView.SelectedItem);

            }
            else return;
        }

        // Deleting file core function
        void FileDelete(FileInfo __selectedFile)
        {
            string selectedFilePath = __selectedFile.FullName; 
            FileAttributes selectedFileAttributes = File.GetAttributes(selectedFilePath);

            // Delete ReadOnly
            if (selectedFileAttributes.HasFlag(System.IO.FileAttributes.ReadOnly))
            {
                File.SetAttributes(selectedFilePath, selectedFileAttributes & ~System.IO.FileAttributes.ReadOnly);
            }

            try
            {
                File.Delete(selectedFilePath);
            }
            catch (IOException exc)
            {
                Console.WriteLine(exc.Message);
                return;
            }
        }

        // ContextMenu - creating new directory - obening NewItemWindow
        void DirectoryCreate_Click(object sender, RoutedEventArgs e)
        {
            // xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            if (TreeView.SelectedItem != null)
            {
                NewItemWindow newItem = new NewItemWindow();
                newItem.ShowDialog();
            } 
            else return;
        }

        // ContextMenu - deleting directory with content
        void DirectoryDelete_Click(object sender, RoutedEventArgs e)
        {
            if (TreeView.SelectedItem != null)
            {
                TreeViewItem selectedDirectory = (TreeViewItem)TreeView.SelectedItem; //object -> TreeViewItem => explicit conversion
                string selectedDirectoryPath = (string)selectedDirectory.Tag; //object -> string => explicit conversion
                DirectoryInfo selectedDirectoryInfo = new DirectoryInfo(selectedDirectoryPath);

                DirectoryDelete(selectedDirectoryInfo);

                if (selectedDirectory.Parent.GetType() == typeof(TreeViewItem) && selectedDirectory.Parent != null)
                {
                    TreeViewItem parent = (TreeViewItem)selectedDirectory.Parent; //object -> TreeViewItem => explicit conversion
                    parent.Items.Remove(TreeView.SelectedItem);
                }
                else
                {
                    TreeView.Items.Clear();
                }
            }
            else return;
        }

        // Deleting directory core function with loop
        void DirectoryDelete(DirectoryInfo __selectedDirectoryInfo)
        {
            string selectedDirectoryPath = __selectedDirectoryInfo.FullName; //object -> string => explicit conversion
            FileAttributes selectedDirectoryAttributes = File.GetAttributes(selectedDirectoryPath);

            // Delete ReadOnly
            if (selectedDirectoryAttributes.HasFlag(System.IO.FileAttributes.ReadOnly))
            {
                File.SetAttributes(selectedDirectoryPath, selectedDirectoryAttributes & ~System.IO.FileAttributes.ReadOnly);
            }

            foreach (var d in __selectedDirectoryInfo.GetDirectories())
            {
                DirectoryDelete(d);
            }

            foreach (var f in __selectedDirectoryInfo.GetFiles())
            {
                FileDelete(f);
            }

            try
            {
                Directory.Delete(selectedDirectoryPath);
            }
            catch (IOException exc)
            {
                Console.WriteLine(exc.Message);
                return;
            }
        }

        // Verifying item attributes
        void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<Object> e)
        {
            if (TreeView.SelectedItem != null)
            {
                TreeViewItem selectedFile = (TreeViewItem)TreeView.SelectedItem; //object -> TreeViewItem => explicit conversion
                string selectedFilePath = (string)selectedFile.Tag; //object -> string => explicit conversion
                FileInfo file = new FileInfo(selectedFilePath);

                FileAttributes.Text = file.SetAttributes();
            }
            else
            {
                FileAttributes.Text = "----";
                return;
            }
        }
    }
}
