using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
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
using SafeVaultBetta.Types;

namespace SafeVaultBetta.Windows
{
    /// <summary>
    /// Interaction logic for CreditantialsWindow.xaml
    /// </summary>
    public partial class CreditantialsWindow : Window
    {
        private List<UserCred> data;
        private int totalIds = default;

        public CreditantialsWindow(List<UserCred> data)
        {
            this.data = data;
            this.DataContentGrid.ItemsSource = this.data;

            if (this.data.Any())
            {
                int cts = GetCurrentUnixSeconds();
                foreach (var i in this.data)
                {
                    i.Id = totalIds++;
                    i.IsHidden = true;
                    i.CurrentTimeStamp = cts;
                }
            }
            else
            {
                this.NoCredsGrid.Visibility = Visibility.Visible;
                this.DataContentGrid.Visibility = Visibility.Hidden;
            }
            
            InitializeComponent();
            this.DataContentGrid.ItemsSource = this.data;
            this.DataContentGrid.Items.Refresh();

        }

        private void PasswordVisibilityChanged(object sender, RoutedEventArgs e)
        {
            var _sender = sender as CheckBox;
            bool isChecked = _sender.IsChecked.HasValue ? _sender.IsChecked.Value : false;

            this.data.Where(i => i.Id == (int)_sender.Tag).First().IsHidden = isChecked;

            this.DataContentGrid.Items.Refresh();
        }

        private void AddUserClick(object sender, RoutedEventArgs e)
        {
            var newCU = new NewUserCredsWindow();
            newCU.NewUC += this.UserCredCreated;
            newCU.Show();
        }

        private void UserCredCreated(object sender, UserCredAddedEvent e)
        {
            this.data.Add(e.Creds);
        }

        private int GetCurrentUnixSeconds()
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan unixTS = DateTime.Now - origin;
            return (int)unixTS.TotalSeconds;
        }
    }
}
