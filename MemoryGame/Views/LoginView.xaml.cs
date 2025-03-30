using MemoryGame.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MemoryGame.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();

            var viewModel = (LoginViewModel)Resources["LoginViewModel"];
            viewModel.ShowNewUserDialogRequested += (s, e) => ShowNewUserDialog();
            viewModel.CloseNewUserDialogRequested += (s, e) => HideNewUserDialog();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Metode pentru a afișa/ascunde dialogul de creare utilizator
        // Acestea vor fi apelate din ViewModel
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
