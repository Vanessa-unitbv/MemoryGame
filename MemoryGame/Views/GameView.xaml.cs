using MemoryGame.Models;
using MemoryGame.ViewModels;
using System;
using System.Windows;

namespace MemoryGame.Views
{
    public partial class GameView : Window
    {
        private GameViewModel _viewModel;

        public GameView()
        {
            InitializeComponent();
            _viewModel = new GameViewModel();
            this.DataContext = _viewModel;
        }

        // Constructor care primește utilizatorul curent
        public GameView(User currentUser)
        {
            InitializeComponent();
            _viewModel = new GameViewModel();
            this.DataContext = _viewModel;

            // Setăm utilizatorul curent
            if (currentUser != null)
            {
                _viewModel.CurrentPlayer = currentUser;
                // Setăm titlul ferestrei să includă numele utilizatorului
                this.Title = $"Game Setup - {currentUser.Username}";
            }
        }
    }
}