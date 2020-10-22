using System;
using System.IO;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using SafeVaultAlpha.Events;
using SafeVaultAlpha.Types;

namespace SafeVaultAlpha
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<User> Users;
        private string FolderUrl;

        public MainWindow()
        {
            InitializeComponent();
            this.Users = new List<User>();
        }


        private void GetFolder(string folderUrl)
        {
            Stream fileStream;
            foreach (var dir in Directory.GetFiles(folderUrl + '\\', "*.dat"))
            {
                fileStream = File.OpenRead(dir);
                User temp = new User { Username = dir.Split('\\').Last().Split('.')[0] };
                fileStream.Read(temp.Password, 0, 32);
                Users.Add(temp);
            }
        }

    }
}
