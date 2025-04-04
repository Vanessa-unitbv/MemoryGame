using MemoryGame.ViewModels;
using System;
using System.Windows;

namespace MemoryGame.Views
{
    public partial class GameBoardView : Window
    {
        private GameBoardViewModel _viewModel;

        public GameBoardView(GameBoardViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            DataContext = _viewModel;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (_viewModel != null)
            {
                _viewModel.StopTimer();
            }
            if (Application.Current.MainWindow == this)
            {
                var loginView = new LoginView();
                loginView.Show();
                Application.Current.MainWindow = loginView;
            }
        }
    }
}