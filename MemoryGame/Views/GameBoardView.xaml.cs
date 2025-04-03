using MemoryGame.ViewModels;
using System;
using System.Windows;

namespace MemoryGame.Views
{
    /// <summary>
    /// Interaction logic for GameBoardView.xaml
    /// </summary>
    public partial class GameBoardView : Window
    {
        private GameBoardViewModel _viewModel;

        public GameBoardView(GameBoardViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            DataContext = _viewModel;
        }

        // Dacă se închide fereastra direct, ne asigurăm că oprim timer-ul
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Când fereastra este închisă, verificăm dacă ea este încă MainWindow
            // Dacă da, deschidem fereastra de login
            if (Application.Current.MainWindow == this)
            {
                var loginView = new LoginView();
                loginView.Show();
                Application.Current.MainWindow = loginView;
            }
        }
    }
}