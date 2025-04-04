using MemoryGame.ViewModels;
using System;
using System.Windows;

namespace MemoryGame.Views
{
    public partial class LoginView : Window
    {
        private LoginViewModel _viewModel;
        public LoginView()
        {
            InitializeComponent();
            _viewModel = (LoginViewModel)Resources["LoginViewModel"];
            _viewModel.ShowNewUserDialogRequested += (s, e) => ShowNewUserDialog();
            _viewModel.CloseNewUserDialogRequested += (s, e) => HideNewUserDialog();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public void ShowNewUserDialog()
        {
            NewUserDialog.Visibility = Visibility.Visible;
        }
        public void HideNewUserDialog()
        {
            NewUserDialog.Visibility = Visibility.Collapsed;
        }
    }
}