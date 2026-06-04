using NUR.Data;
using System.Windows;
using System.Windows.Controls;

namespace NUR.Views
{

    public partial class UserProfile : UserControl
    {
        public UserProfile()
        {
            InitializeComponent();

            string name = UserSession.Username ?? "Гость";
            txtUsername.Text = $"Привет, {name}!";
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            NUR.Properties.Settings.Default.AuthToken = "";
            NUR.Properties.Settings.Default.Save();

            ApiClient.Token = null;

            LoginReg loginWin = new LoginReg();
            loginWin.Show();

            Window.GetWindow(this)?.Close();
        }
    }
}
