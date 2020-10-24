using System;
using System.IO;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using SafeVaultAlpha.Events;
using SafeVaultAlpha.Types;
using SafeVaultAlpha.Windows;
using SafeVaultBetta.Types;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Controls;
using SafeVaultAlpha.Cryptography;

namespace SafeVaultAlpha
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<User> Users;
        private Stream fs;
        private string FolderUrl;
        private byte[] inputPassBytes;

        public MainWindow()
        {
            InitializeComponent();

            this.Users = new List<User>();
            GetFolder(Directory.GetCurrentDirectory());
            this.UsersComboBox.ItemsSource = Users;

            if (Users.Any())
            {
                this.AnyUsersGrid.Visibility = Visibility.Visible;
                this.NoUsersGrid.Visibility = Visibility.Collapsed;
            }
            this.UserPassword.IsEnabled = false;
            this.ChooseFileButton.IsEnabled = false;
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            if (inputPassBytes == Users.Where(u => u.Username == ((ComboBoxItem)this.UsersComboBox.SelectedItem).Content.ToString()).First().Password)
            {
                byte[] source = null;
                string pass = null;

                byte[] decryptionKey = null;

                if (fs != null && UserPassword.Password == "")
                {
                    fs.Read(source, 0, FileConsts.MinimalFileSize);
                    source.CopyTo(decryptionKey, 0);
                    source.CopyTo(decryptionKey, FileConsts.MinimalFileSize);

                    decryptionKey = StaticHash.StaticSha256(source);
                }
                else if (UserPassword.Password != "" && fs == null)
                {
                    pass = String.Concat(Enumerable.Repeat<string>(UserPassword.Password, 2)); // UserPassword + UserPassword
                    decryptionKey = StaticHash.StaticSha256(Encoding.UTF8.GetBytes(pass));
                }
                else
                {
                    MessageBox.Show("Key input error.", "Error", MessageBoxButton.OK);
                    return;
                }
                if (decryptionKey != null)
                {

                    // form w data & decription
                }
            }
        }

        private void UserCreated(object sender, UserCreatedEventArgs e)
        {
            Users.Add(e.User);
            
            if (NoUsersGrid.Visibility == Visibility.Visible)
            {
                this.NoUsersGrid.Visibility = Visibility.Collapsed;
                this.AnyUsersGrid.Visibility = Visibility.Visible;
            }
        }

        private void onSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(((ComboBoxItem)((ComboBox)sender).SelectedItem).Content.ToString() == "")
            {
                this.UserPassword.Password = "";
                this.UserPassword.IsEnabled = false;
                this.ChooseFileButton.IsEnabled = false;
                this.ConfirmalButton.IsEnabled = false;
                this.fs = null;
            }
            else
            {
                this.UserPassword.IsEnabled = true;
                this.ChooseFileButton.IsEnabled = true;
            }
        }

        private void ManualPassInput(object sender, RoutedEventHandler a)
        {
            if(this.UserPassword.Password != "")
            {
                this.inputPassBytes = StaticHash.StaticSha256(Encoding.UTF8.GetBytes(this.UserPassword.Password));
            }
            if(this.inputPassBytes.Length == 0)
            {
                this.ConfirmalButton.IsEnabled = false;
            }
        }

        private void LoginUsingFile(object sender, RoutedEventArgs e)
        {
            var fdialog = new OpenFileDialog();
            Nullable<bool> res = fdialog.ShowDialog();

            if (res == true)
            {
                Stream file = fdialog.OpenFile();
                
                if (file.Length > FileConsts.VerificationFileMaxLength)
                {
                    MessageBox.Show("Very big file!", "Error", MessageBoxButton.OK);
                }
                else if(file.Length< FileConsts.MinimalFileSize)
                {
                    MessageBox.Show("Very small file!", "Error", MessageBoxButton.OK);
                }
                else
                {
                    byte[] temp=null;

                    if (file.Read(temp, 0, (int)file.Length) == 0) 
                    {
                        this.inputPassBytes = StaticHash.StaticSha256(temp);
                        this.UserPassword.Password = "";
                        this.UserPassword.IsEnabled = false;
                        this.ConfirmalButton.IsEnabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Unknown file read error. ", "Error", MessageBoxButton.OK);
                    }
                }
            }
        }

        private void CreateVaultButtonClick(object sender, RoutedEventArgs e)
        {
            CreateNewVaultWindow vaultInit = new CreateNewVaultWindow();
            vaultInit.NewUser += UserCreated;
            vaultInit.Show();
        }

        private void ChangeFolder(object sender, RoutedEventArgs e)
        {
            var fdialog = FolderDialog();

            if(fdialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                GetFolder(fdialog.FileName);
            }
        }

        private void GetFolder(string folderUrl)
        {
            Users.Clear();

            this.FolderUrl = folderUrl;
            User temp;
            byte[] fileExtDet = null;
            int offset = FileConsts.FileSign.Length;

            foreach (var dir in Directory.GetFiles(this.FolderUrl + '\\', "*.dat"))
            {
                this.fs = File.OpenRead(dir);
                this.fs.Read(fileExtDet, 0, offset);
                
                if(fileExtDet == FileConsts.FileSign)
                {
                    temp = new User(this.fs, dir.Split('\\').Last().Split('.')[0]);
                    Users.Add(temp);
                }
            }
        }

        private CommonOpenFileDialog FolderDialog()
        {
            var dlg = new CommonOpenFileDialog();

            dlg.EnsureReadOnly = true;
            dlg.IsFolderPicker = true;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.EnsurePathExists = true;
            dlg.AllowNonFileSystemItems = false;
            dlg.AddToMostRecentlyUsedList = false;
            dlg.Title = "Pick a folder with vaults";

            return dlg;
        }

    }
}
