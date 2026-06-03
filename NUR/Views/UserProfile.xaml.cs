using System.Windows.Controls;

namespace NUR.Views
{

    public partial class UserProfile : UserControl
    {
        public UserProfile()
        {
            InitializeComponent();
            txtUsername.Text = $"Привет, {UserSession.Username}!";
        }
    }
}
