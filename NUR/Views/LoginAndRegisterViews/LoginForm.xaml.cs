using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using NUR.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace NUR.Views.LoginAndRegisterViews
{
    public partial class LoginForm : UserControl
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {

            string username = txtLoginUsername.Text.Trim();
            string password = txtLoginPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Вы ввели пустые данные!");
                return;
            }

            txtLoginUsername.IsEnabled = false;
            txtLoginPassword.IsEnabled = false;
            btnLogin.IsEnabled = false;

            LoadingText.Visibility = Visibility.Visible;

            try
            {
             

                using (var db = new NurDbContext())
                {
                    var user = await db.AppUsers
                        .FirstOrDefaultAsync(u => u.Username == username);

                    if (user == null)
                    {
                        MessageBox.Show("Пользователь с таким логином не найден в базе.");
                        return;
                    }
                    bool isPasswordValid = await Task.Run(() =>
                        BCrypt.Net.BCrypt.Verify(password, user.Password));

                    if (isPasswordValid)
                    {
                        UserSession.Username = user.Username;

                        MessageBox.Show("Успешный вход!");
                        LoadingText.Visibility = Visibility.Collapsed;

                        MainWindow mainWin = new MainWindow();
                        mainWin.Show();

                        Window.GetWindow(this)?.Close();
                    }
                    else
                    {
                        MessageBox.Show("Неверный пароль.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Критическая ошибка при входе: {ex.Message}");
            }
            finally
            {
                txtLoginUsername.IsEnabled = true;
                txtLoginPassword.IsEnabled = true;
                btnLogin.IsEnabled = true;
            }
        }
    }
}