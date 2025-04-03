using System;
using System.Windows;
using MemoryGame.Models;
using MemoryGame.ViewModels;

namespace MemoryGame.Views
{
    public partial class OpenGameDialog : Window
    {
        private OpenGameDialogViewModel _viewModel;

        public GameState SelectedGame => _viewModel.SelectedGame;

        public OpenGameDialog()
        {
            InitializeComponent();

            _viewModel = new OpenGameDialogViewModel();
            DataContext = _viewModel;

            // Abonăm la evenimentul de închidere
            _viewModel.RequestClose += result =>
            {
                DialogResult = result;
                Close();
            };
        }

        public void RefreshGamesList()
        {
            _viewModel.RefreshGamesList();
        }
    }
}