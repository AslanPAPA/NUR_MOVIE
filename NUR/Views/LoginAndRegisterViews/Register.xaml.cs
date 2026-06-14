using NUR.Data;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace NUR.Views.LoginAndRegisterViews
{
    public partial class Register : UserControl
    {
        public Register()
        {
            InitializeComponent();
        }

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
          

            txtRegUsername.IsEnabled = false;
            txtRegEmail.IsEnabled = false;
            txtRegPassword.IsEnabled = false;
            txtRegConfirmPassword.IsEnabled = false;
            btnRegister.IsEnabled = false;

            LoadingText.Visibility = Visibility.Visible;
            bool hasInternet = await InternetHelper.HasInternet();

            if (!hasInternet)
            {
                MessageBox.Show(
                    "Для регистрации необходимо подключение к интернету.");
                LoadingText.Visibility = Visibility.Collapsed;

                return;
            }

            try
            {
                string username = txtRegUsername.Text.Trim();
                string email = txtRegEmail.Text.Trim();
                string password = txtRegPassword.Password;
                string confirmPassword = txtRegConfirmPassword.Password;

                if (string.IsNullOrEmpty(username) ||
                    string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Вы ввели пустые данные!");
                    return;
                }

                if (password != confirmPassword)
                {
                    MessageBox.Show("Пароли не совпадают!");
                    return;
                }

                if (password.Length < 8)
                {
                    MessageBox.Show("Пароль должен содержать минимум 8 символов.");
                    return;
                }

                bool isPwned = await PwnedPasswordChecker.IsPasswordPwned(password);

                if (isPwned)
                {
                    MessageBox.Show("Этот пароль был в утечках. Выберите другой.");
                    return;
                }

                var registerData = new
                {
                    username = username,
                    password = password,
                    email = string.IsNullOrWhiteSpace(email) ? null : email
                };

                var json = JsonSerializer.Serialize(registerData);

                var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await ApiClient.Instance.PostAsync(
                    "http://185.246.222.35:8080/api/register/",
                    content
                );

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Регистрация успешна!");

                    txtRegUsername.Clear();
                    txtRegEmail.Clear();
                    txtRegPassword.Clear();
                    txtRegConfirmPassword.Clear();
                }
                else
                {
                    string error =
                        await response.Content.ReadAsStringAsync();

                    MessageBox.Show(error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            finally
            {
                LoadingText.Visibility = Visibility.Collapsed;

                txtRegUsername.IsEnabled = true;
                txtRegEmail.IsEnabled = true;
                txtRegPassword.IsEnabled = true;
                txtRegConfirmPassword.IsEnabled = true;
                btnRegister.IsEnabled = true;
            }
        }
    }
}