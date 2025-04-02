using MemoryGame.Models;
using MemoryGame.Services;
using MemoryGame.ViewModels;
using System;
using System.Windows;

namespace MemoryGame.Views
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : Window
    {
        private GameViewModel _viewModel;

        public GameView()
        {
            InitializeComponent();
            _viewModel = new GameViewModel();
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// Constructor care primește utilizatorul curent
        /// </summary>
        /// <param name="currentUser">Utilizatorul curent</param>
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

            // Înregistrăm pentru a primi mesaje de navigare
            Messenger.Default.Register<NavigationMessage>(OnNavigationRequested);
        }

        // Metoda care gestionează cererile de navigare
        private void OnNavigationRequested(NavigationMessage message)
        {
            if (message != null && message.Destination == "LoginView")
            {
                // Navigare înapoi la login este gestionată deja de ViewModel
            }
        }

        // Curățăm resursele la închiderea ferestrei
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Eliberăm resursele din ViewModel
            //if (_viewModel != null)
            //{
            //    _viewModel.Cleanup();
            //}
        }
    }
}