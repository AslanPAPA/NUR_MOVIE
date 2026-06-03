using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using NUR.Data;

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

            try
            {
                string username = txtRegUsername.Text.Trim();
                string email = txtRegEmail.Text.Trim();
                string password = txtRegPassword.Password;
                string confirmPassword = txtRegConfirmPassword.Password;

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
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

                using (var db = new NurDbContext())
                {
                    if (await db.AppUsers.AnyAsync(u => u.Username == username))
                    {
                        MessageBox.Show("Пользователь с таким именем уже существует.");
                        return;
                    }

                    string passwordHash = await Task.Run(() =>
                        BCrypt.Net.BCrypt.HashPassword(password));


                    var newUser = new AppUser
                    {
                        Username = username,
                        Password = passwordHash,
                        Email = string.IsNullOrWhiteSpace(email) ? null : email
                    };

                    db.AppUsers.Add(newUser);
                    db.SaveChanges();
                }

                MessageBox.Show("Регистрация успешна!");
                LoadingText.Visibility = Visibility.Collapsed;
                txtRegUsername.Clear();
                txtRegPassword.Clear();
                txtRegConfirmPassword.Clear();
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += "\n\nВнутренняя ошибка: " + ex.InnerException.Message;
                }

                MessageBox.Show(errorMessage, "Детали ошибки", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                txtRegUsername.IsEnabled = true;
                txtRegEmail.IsEnabled = true;
                txtRegPassword.IsEnabled = true;
                txtRegConfirmPassword.IsEnabled = true;
                btnRegister.IsEnabled = true;


            }


        }
    }
}