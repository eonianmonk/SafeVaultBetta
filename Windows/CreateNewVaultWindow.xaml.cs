using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using SafeVaultAlpha.Cryptography;
using SafeVaultAlpha.Events;
using SafeVaultAlpha.RegularExpressions;
using SafeVaultAlpha.Types;
using SafeVaultBetta.Types;

namespace SafeVaultAlpha.Windows
{
    /// <summary>
    /// Interaction logic for CreateNewVaultWindow.xaml
    /// </summary>
    public partial class CreateNewVaultWindow : Window
    {
        private FileStream ChosenForVerificationFile;
        public event EventHandler<UserCreatedEventArgs> NewUser;

        public CreateNewVaultWindow()
        {
            InitializeComponent();
            this.BadBaseDataOverlayGrid.Visibility = Visibility.Hidden;
            this.UseFileInsteadCheckBox.IsChecked = false;
        }

        private void OnUserDataChanged(object sender, RoutedEventHandler a)
        {
            if (this.FirstPassInput.Password.Length > 12 &&
               this.ConfirmalPassInput.Password.Length == this.FirstPassInput.Password.Length &&
               this.UsernameInput.Text.Length > 0)
            {
                this.ConfirmInputDataButton.IsEnabled = true;
            }
            else
            {
                this.ConfirmInputDataButton.IsEnabled = false;
            }
        }

        private void OnBadBaseDataOKClick(object sender, RoutedEventArgs e)
        {
            this.BadBaseDataOverlayGrid.Visibility = Visibility.Hidden;
        }

        private void ConfirmInputUserData(object sender, RoutedEventArgs e)
        {
            bool isFailure = false;

            if (Regex.IsMatch(this.UsernameInput.Text, InputRegexes.InvalidUsername))
            {
                this.ErrorNotificationTBlock.Text = "Username mustn't contain: ./\\\" *?:<>|";
            }
            else if (!this.UseFileInsteadCheckBox.IsChecked.Value)
            {
                if (this.ConfirmalPassInput.Password != this.FirstPassInput.Password)
                {
                    if (Regex.IsMatch(this.ConfirmalPassInput.Password, InputRegexes.StrongPasswordRegex))
                    {
                        if (!File.Exists(this.UsernameInput.Text + ".dat"))
                        {
                            isFailure = true;
                        }
                        else this.ErrorNotificationTBlock.Text = "Such user already exists";
                    }
                    else this.ErrorNotificationTBlock.Text = "Entered password doesn't meet all the requirments";

                }
                else this.ErrorNotificationTBlock.Text = "Passwords don't coinside.";
            }
            else
            {
                if (ChosenForVerificationFile != null)
                {
                    if (ChosenForVerificationFile.Length >= FileConsts.VerificationFileMaxLength)
                    {
                        if (ChosenForVerificationFile.Length <= FileConsts.MinimalFileSize)
                        {
                            isFailure = true;
                        }
                        else this.ErrorNotificationTBlock.Text = "Size must exceed "+FileConsts.MinimalFileSize+"bytes";
                    }
                    else this.ErrorNotificationTBlock.Text = "Size mustn't exceed 512 MB";
                }
                else this.ErrorNotificationTBlock.Text = "You haven't chosen a file";
            }
            if (!isFailure)
            {
                this.NewUser(this, new UserCreatedEventArgs(this.CreateUser()));
                this.Close();
            }
            else
            {
                this.BadBaseDataOverlayGrid.Visibility = Visibility.Visible;
            }
        }

        private void ChangeVerificatonType(object sender, RoutedEventArgs e)
        {
            if (this.UseFileInsteadCheckBox.IsChecked == true)
            {
                this.UsualPassGrid.Visibility = Visibility.Hidden;
                this.ConfirmalPassInput.Password = "";
                this.FirstPassInput.Password = "";
            }
            else
            {
                this.FilePassGrid.Visibility = Visibility.Hidden;
                this.ChosenForVerificationFile = null;
            }
        }

        private void ChooseFileForSigning(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".";
            Nullable<bool> result = dialog.ShowDialog();

            if (result == true)
            {
                this.ChosenForVerificationFile = dialog.OpenFile() as FileStream;
            }
        }

        private User CreateUser()
        {
            byte[] bytePass;

            if (this.UseFileInsteadCheckBox.IsChecked == true)
            {
                byte[] fileRead = null;
                ChosenForVerificationFile.Read(fileRead, 0, (Int32)ChosenForVerificationFile.Length);
                bytePass = StaticHash.StaticSha256(fileRead);
            }
            else
            {
                bytePass = StaticHash.StaticSha256(Encoding.UTF8.GetBytes(this.FirstPassInput.Password));
            }

            User user = new User
            {
                Username = this.UsernameInput.Text,
                Password = bytePass,
                FileMinimalLength = FileConsts.MinimalFileSize
            };

            return user;
        }
    }
}
