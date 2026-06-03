using System;
using System.Windows;
using System.Windows.Input;

namespace NUR.Views
{
    public partial class LoginReg : Window
    {
        public LoginReg()
        {
            InitializeComponent();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) this.DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}