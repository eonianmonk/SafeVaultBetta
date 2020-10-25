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
using SafeVaultBetta.Types;

namespace SafeVaultBetta.Windows
{
    /// <summary>
    /// Interaction logic for CreditantialsWindow.xaml
    /// </summary>
    public partial class CreditantialsWindow : Window
    {
        private List<UserCred> data;

        public CreditantialsWindow(List<UserCred> data)
        {
            this.data = data;
            InitializeComponent();
        }
    }
}
