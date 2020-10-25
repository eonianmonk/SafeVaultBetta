using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using SafeVaultAlpha.Events;
using SafeVaultAlpha.Types;
using SafeVaultAlpha.Windows;
using SafeVaultBetta.Types;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Controls;
using SafeVaultAlpha.Cryptography;
using Newtonsoft.Json;
using SafeVaultBetta.Windows;

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
            var username = ((ComboBoxItem)this.UsersComboBox.SelectedItem).Content.ToString();
            string pathString = Path.Combine(FolderUrl, username + FileConsts.Extenstion);

            if (inputPassBytes == Users.Where(u => u.Username == username).First().Password)
            {
                byte[] source = null;
                string pass = null;

                byte[] decryptionKey = null;

                if (fs != null && UserPassword.Password == "")
                {
                    fs.Read(source, FileConsts.FileSign.Length + StaticHash.SHA1HashLength, 4);
                    int minFileSize = BitConverter.ToInt32(source);
                    fs.Read(source, (Int32)fs.Length/2 - minFileSize/2, minFileSize/ 2);
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
                    byte[] encryptedData = null;
                    Stream fs = File.OpenRead(pathString);

                    int encryptedDataOffset = FileConsts.FileSign.Length + StaticHash.SHA1HashLength + 4; 
                    fs.Read(encryptedData, encryptedDataOffset, (Int32)fs.Length - encryptedDataOffset - FileConsts.FileSign.Length);

                    if (encryptedData.Length != 0)
                    {
                        var cipher = new MyAES256();
                        byte[] userCreds = cipher.DecryptAes(decryptionKey, encryptedData);

                        string rowData = Encoding.UTF8.GetString(userCreds);
                        var creds = JsonConvert.DeserializeObject<List<UserCred>>(rowData);

                        var nw = new CreditantialsWindow(creds);
                        nw.Show();
                        this.Close();
                    }

                }
            }
        }

        private void UserCreated(object sender, UserCreatedEventArgs e)
        {
            Users.Add(e.User);
            string pathString = Path.Combine(FolderUrl, e.User.Username + FileConsts.Extenstion);

            if (NoUsersGrid.Visibility == Visibility.Visible)
            {
                this.NoUsersGrid.Visibility = Visibility.Collapsed;
                this.AnyUsersGrid.Visibility = Visibility.Visible;
            }

            if (File.Exists(pathString))
            {
                MessageBox.Show("Such user already exists in selected folder!", "Error", MessageBoxButton.OK);
                return;
            }
            // User creation (file)
            else
            {
                using (FileStream fs = File.Create(pathString))
                {
                    fs.Write(FileConsts.FileSign);                               // 0x de ad be ef
                    fs.Write(e.User.Password);                                   // 32 bytes SHA256
                    fs.Write(BitConverter.GetBytes(FileConsts.MinimalFileSize)); // integer v possible file byte length to sign
                    fs.Write(FileConsts.FileSign);                               // 0x de ad be ef
                }
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

                    if (file.Read(temp, (Int32)file.Length/2, FileConsts.MinimalFileSize/2) == 0) 
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
            User temp = null;
            byte[] fileExtDet = null;
            int offset = FileConsts.FileSign.Length;

            foreach (var dir in Directory.GetFiles(this.FolderUrl + '\\', "*.dat"))
            {
                this.fs = File.OpenRead(dir);
                this.fs.Read(fileExtDet, 0, offset);
                
                if(fileExtDet == FileConsts.FileSign)
                {
                    this.fs.Read(fileExtDet, (int)this.fs.Length - offset, offset);

                    if (fileExtDet == FileConsts.FileSign)
                    {
                        temp = new User(this.fs, dir.Split('\\').Last().Split('.')[0]);
                        Users.Add(temp);
                    }
                }

                if(temp == null)
                {
                    MessageBox.Show("Malformed file.", "Error", MessageBoxButton.OK);
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
