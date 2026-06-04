using NUR.Data;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

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
                var loginData = new
                {
                    username = username,
                    password = password
                };

                var json = JsonSerializer.Serialize(loginData);

                var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await ApiClient.Instance.PostAsync(
                    "http://185.246.222.35:8080/api/login/",
                    content
                );

                if (response.IsSuccessStatusCode)
                {
                    var result =
                        await response.Content.ReadAsStringAsync();

                    var loginResponse =
                        JsonSerializer.Deserialize<LoginResponse>(result);

                    ApiClient.SetToken(loginResponse.token, loginResponse.username);

                    UserSession.Username =
                        loginResponse.username;

                    MessageBox.Show("Успешный вход!");


                    MainWindow mainWin = new MainWindow();
                    mainWin.Show();

                    Window.GetWindow(this)?.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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