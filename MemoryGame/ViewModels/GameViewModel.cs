using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using MemoryGame.Commands;
using MemoryGame.Models;
using MemoryGame.Views;

namespace MemoryGame.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        // Proprietăți pentru utilizator și joc
        private User _currentPlayer;
        private string _statusMessage;

        // Proprietăți pentru tipul de joc
        private bool _isStandardMode = true;
        private bool _isCustomMode = false;

        // Proprietăți pentru joc custom
        private int _customRows = 4;
        private int _customColumns = 4;
        private int _playerTimeSeconds = 60;

        // Proprietăți pentru categoria de imagini
        private bool _categoryOne = true;
        private bool _categoryTwo = false;
        private bool _categoryThree = false;

        // Proprietăți pentru dialoguri
        private bool _isStatisticsVisible = false;
        private bool _isAboutVisible = false;

        // Proprietăți publice
        public User CurrentPlayer
        {
            get => _currentPlayer;
            set
            {
                _currentPlayer = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(WinRateFormatted));
                OnPropertyChanged(nameof(HasNoRecentGames));
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        // Proprietăți pentru tipul de joc
        public bool IsStandardMode
        {
            get => _isStandardMode;
            set
            {
                if (_isStandardMode != value)
                {
                    _isStandardMode = value;
                    if (value)
                    {
                        IsCustomMode = false;
                        // Resetăm valorile la cele standard
                        CustomRows = 4;
                        CustomColumns = 4;
                    }
                    OnPropertyChanged();
                }
            }
        }

        public bool IsCustomMode
        {
            get => _isCustomMode;
            set
            {
                if (_isCustomMode != value)
                {
                    _isCustomMode = value;
                    if (value)
                    {
                        IsStandardMode = false;
                    }
                    OnPropertyChanged();
                }
            }
        }

        // Proprietăți pentru joc custom
        public int CustomRows
        {
            get => _customRows;
            set
            {
                if (value >= 2 && value <= 6) // Limitarea între 2 și 6
                {
                    _customRows = value;
                    OnPropertyChanged();
                }
            }
        }

        public int CustomColumns
        {
            get => _customColumns;
            set
            {
                if (value >= 2 && value <= 6) // Limitarea între 2 și 6
                {
                    _customColumns = value;
                    OnPropertyChanged();
                }
            }
        }

        public int PlayerTimeSeconds
        {
            get => _playerTimeSeconds;
            set
            {
                if (value > 0) // Timpul trebuie să fie pozitiv
                {
                    _playerTimeSeconds = value;
                    OnPropertyChanged();
                }
            }
        }

        // Proprietăți pentru categoria de imagini
        public bool CategoryOne
        {
            get => _categoryOne;
            set
            {
                if (_categoryOne != value)
                {
                    _categoryOne = value;
                    if (value)
                    {
                        CategoryTwo = false;
                        CategoryThree = false;
                    }
                    OnPropertyChanged();
                }
            }
        }

        public bool CategoryTwo
        {
            get => _categoryTwo;
            set
            {
                if (_categoryTwo != value)
                {
                    _categoryTwo = value;
                    if (value)
                    {
                        CategoryOne = false;
                        CategoryThree = false;
                    }
                    OnPropertyChanged();
                }
            }
        }

        public bool CategoryThree
        {
            get => _categoryThree;
            set
            {
                if (_categoryThree != value)
                {
                    _categoryThree = value;
                    if (value)
                    {
                        CategoryOne = false;
                        CategoryTwo = false;
                    }
                    OnPropertyChanged();
                }
            }
        }

        // Proprietăți pentru dialoguri
        public bool IsStatisticsVisible
        {
            get => _isStatisticsVisible;
            set
            {
                _isStatisticsVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsAboutVisible
        {
            get => _isAboutVisible;
            set
            {
                _isAboutVisible = value;
                OnPropertyChanged();
            }
        }

        // Proprietăți calculate
        public string WinRateFormatted
        {
            get
            {
                if (CurrentPlayer == null || CurrentPlayer.GamesPlayed == 0)
                    return "0%";

                double winRate = (double)CurrentPlayer.GamesWon / CurrentPlayer.GamesPlayed * 100;
                return $"{winRate:F1}%";
            }
        }

        public bool HasNoRecentGames
        {
            get
            {
                return CurrentPlayer == null || CurrentPlayer.GamesPlayed == 0;
            }
        }

        // Comenzi
        public ICommand NewGameCommand { get; }
        public ICommand OpenGameCommand { get; }
        public ICommand SaveGameCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand ShowStatisticsCommand { get; }
        public ICommand ShowAboutCommand { get; }
        public ICommand CloseStatisticsCommand { get; }
        public ICommand CloseAboutCommand { get; }
        public ICommand StartGameCommand { get; }
        public ICommand BackToLoginCommand { get; }

        // Constructor
        public GameViewModel()
        {
            // Inițializarea comenzilor
            NewGameCommand = new RelayCommand(_ => ResetGameSettings());
            OpenGameCommand = new RelayCommand(_ => OpenGame());
            SaveGameCommand = new RelayCommand(_ => SaveGame());
            ExitCommand = new RelayCommand(_ => BackToLogin());

            ShowStatisticsCommand = new RelayCommand(_ => IsStatisticsVisible = true);
            CloseStatisticsCommand = new RelayCommand(_ => IsStatisticsVisible = false);

            ShowAboutCommand = new RelayCommand(_ => IsAboutVisible = true);
            CloseAboutCommand = new RelayCommand(_ => IsAboutVisible = false);

            StartGameCommand = new RelayCommand(_ => StartGame(), _ => IsConfigurationValid());
            BackToLoginCommand = new RelayCommand(_ => BackToLogin());

            // Inițializarea valorilor implicite
            StatusMessage = "Ready to start";
        }

        // Metodă pentru a reseta setările jocului
        private void ResetGameSettings()
        {
            IsStandardMode = true;
            IsCustomMode = false;
            CustomRows = 4;
            CustomColumns = 4;
            PlayerTimeSeconds = 60;
            CategoryOne = true;
            CategoryTwo = false;
            CategoryThree = false;
        }

        // Metoda pentru a deschide un joc salvat
        private void OpenGame()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Memory Game Files (*.mem)|*.mem|All Files (*.*)|*.*",
                    Title = "Open Saved Game"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    // Aici va fi implementată logica de încărcare a jocului
                    string fileName = openFileDialog.FileName;
                    MessageBox.Show($"Selected file: {fileName}\nLoading game functionality will be implemented.",
                                    "Open Game",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening game: {ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        // Metoda pentru a salva jocul curent
        private void SaveGame()
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Memory Game Files (*.mem)|*.mem|All Files (*.*)|*.*",
                    Title = "Save Game",
                    DefaultExt = "mem"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    // Aici va fi implementată logica de salvare a jocului
                    string fileName = saveFileDialog.FileName;
                    MessageBox.Show($"Game will be saved to: {fileName}\nSaving functionality will be implemented.",
                                    "Save Game",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving game: {ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        // Metodă pentru a verifica dacă configurația jocului este validă
        private bool IsConfigurationValid()
        {
            // Verifică dacă avem un utilizator curent
            if (CurrentPlayer == null)
                return false;

            // Verifică configurația pentru joc custom
            if (IsCustomMode)
            {
                // Verifică dacă dimensiunile sunt între 2 și 6
                if (CustomRows < 2 || CustomRows > 6 || CustomColumns < 2 || CustomColumns > 6)
                    return false;

                // Verifică dacă numărul total de cartonașe este par
                if ((CustomRows * CustomColumns) % 2 != 0)
                    return false;
            }

            // Verifică dacă timpul per jucător este valid (pentru ambele tipuri de joc)
            if (PlayerTimeSeconds <= 0)
                return false;

            // Verifică dacă este selectată o categorie
            if (!CategoryOne && !CategoryTwo && !CategoryThree)
                return false;

            return true;
        }

        // Metoda pentru a începe jocul
        private void StartGame()
        {
            try
            {
                // Verificăm din nou validitatea configurației
                if (!IsConfigurationValid())
                {
                    MessageBox.Show("Configurația jocului nu este validă. Vă rugăm verificați setările.",
                                   "Configurație invalidă",
                                   MessageBoxButton.OK,
                                   MessageBoxImage.Warning);
                    return;
                }

                // Obținem categoria selectată
                int selectedCategory = 1;
                if (CategoryTwo) selectedCategory = 2;
                if (CategoryThree) selectedCategory = 3;

                // Obținem dimensiunile tablei de joc
                int rows = IsStandardMode ? 4 : CustomRows;
                int columns = IsStandardMode ? 4 : CustomColumns;

                // Obținem timpul per jucător
                int timePerPlayer = PlayerTimeSeconds;

                // Aici vom lansa jocul propriu-zis (aceasta va fi implementată ulterior)
                MessageBox.Show($"Începem jocul pentru {CurrentPlayer.Username}!\n" +
                              $"Categorie: {selectedCategory}\n" +
                              $"Dimensiuni: {rows}x{columns}\n" +
                              $"Timp per jucător: {timePerPlayer} secunde",
                              "Joc nou");

                // Aici se va implementa lansarea jocului propriu-zis
                // (Se va înlocui MessageBox-ul de mai sus cu codul pentru a deschide fereastra jocului)
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la începerea jocului: {ex.Message}",
                               "Eroare",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }

        // Metoda pentru a reveni la ecranul de login
        private void BackToLogin()
        {
            try
            {
                // Creăm o nouă instanță a ferestrei de login
                var loginView = new LoginView();

                // Afișăm fereastra de login
                loginView.Show();

                // Închide fereastra curentă
                if (Application.Current.MainWindow is GameView gameView)
                {
                    // Setăm fereastra de login ca MainWindow
                    Application.Current.MainWindow = loginView;
                    gameView.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la revenirea la ecranul de login: {ex.Message}",
                               "Eroare",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }

        // Implementare INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}