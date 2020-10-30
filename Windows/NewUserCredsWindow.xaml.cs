using SafeVaultBetta.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SafeVaultBetta.Events; 

namespace SafeVaultBetta.Windows
{
    /// <summary>
    /// Interaction logic for NewUserCredsWindow.xaml
    /// </summary>
    public partial class NewUserCredsWindow : Window
    {
        public event EventHandler<UserCredAddedEvent> NewUC;

        public NewUserCredsWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(this.Credbox.Text =="" || this.Credbox.Text == "")
            {
                MessageBox.Show("Not all fields completed.", "ERROR");
            }
            else
            {
                this.NewUC(this, new UserCredAddedEvent(CreateData()));
                this.Close();
            }
        }

        private UserCred CreateData()
        {
            var cts = GetCurrentUnixSeconds();
            return new UserCred
            {
                CredReference = this.Credbox.Text,
                Password = this.Credbox.Text,
                SetTimeStamp = cts,
                IsHidden = true,
                CurrentTimeStamp = cts
            };
        }

        private int GetCurrentUnixSeconds()
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan unixTS = DateTime.Now - origin;
            return (int)unixTS.TotalSeconds;
        }
    }
}
