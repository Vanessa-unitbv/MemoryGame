using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MemoryGame.Commands;
using MemoryGame.Models;
using MemoryGame.Services;
using Microsoft.Win32;

namespace MemoryGame.ViewModels
{
    public class GameBoardViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly UserService _userService;
        private readonly Random _random = new Random();
        private readonly List<string> _imageFiles = new List<string>();
        private DispatcherTimer _gameTimer;
        private Card _firstSelectedCard;
        private Card _secondSelectedCard;
        private bool _isCheckingMatch;
        private int _remainingTimeInSeconds;
        private int _score;
        private bool _gameEnded;

        #endregion

        #region Properties

        public ObservableCollection<Card> Cards { get; } = new ObservableCollection<Card>();

        private User _currentPlayer;
        public User CurrentPlayer
        {
            get => _currentPlayer;
            set
            {
                if (_currentPlayer != value)
                {
                    _currentPlayer = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _category;
        public int Category
        {
            get => _category;
            set
            {
                if (_category != value)
                {
                    _category = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _rows;
        public int Rows
        {
            get => _rows;
            set
            {
                if (_rows != value)
                {
                    _rows = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _columns;
        public int Columns
        {
            get => _columns;
            set
            {
                if (_columns != value)
                {
                    _columns = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _totalTimeInSeconds;
        public int TotalTimeInSeconds
        {
            get => _totalTimeInSeconds;
            set
            {
                if (_totalTimeInSeconds != value)
                {
                    _totalTimeInSeconds = value;
                    OnPropertyChanged();
                }
            }
        }

        public int RemainingTimeInSeconds
        {
            get => _remainingTimeInSeconds;
            set
            {
                if (_remainingTimeInSeconds != value)
                {
                    _remainingTimeInSeconds = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TimeDisplay));
                }
            }
        }

        public int Score
        {
            get => _score;
            set
            {
                if (_score != value)
                {
                    _score = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool GameEnded
        {
            get => _gameEnded;
            set
            {
                if (_gameEnded != value)
                {
                    _gameEnded = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TimeDisplay => $"{RemainingTimeInSeconds / 60:D2}:{RemainingTimeInSeconds % 60:D2}";

        #endregion

        #region Commands

        public ICommand CardClickCommand { get; }
        public ICommand SaveGameCommand { get; }
        public ICommand ExitCommand { get; }

        #endregion

        #region Constructor

        public GameBoardViewModel(User player, int category, int rows, int columns, int timeInSeconds)
        {
            _userService = new UserService();
            CurrentPlayer = player ?? throw new ArgumentNullException(nameof(player));
            Category = category;
            Rows = rows;
            Columns = columns;
            TotalTimeInSeconds = timeInSeconds;
            RemainingTimeInSeconds = timeInSeconds;

            // Inițializăm comenzile
            CardClickCommand = new RelayCommand(CardClick, CanCardClick);
            SaveGameCommand = new RelayCommand(_ => SaveGame(), _ => !GameEnded);
            ExitCommand = new RelayCommand(_ => Exit());

            // Încărcăm imaginile și creăm cardurile
            LoadImages();
            InitializeCards();

            // Pornim timerul
            StartTimer();
        }

        #endregion

        #region Game Logic Methods

        private void LoadImages()
        {
            try
            {
                string categoryFolder;
                switch (Category)
                {
                    case 1:
                        categoryFolder = "Animals";
                        break;
                    case 2:
                        categoryFolder = "Food";
                        break;
                    case 3:
                        categoryFolder = "Landscapes";
                        break;
                    default:
                        categoryFolder = "Animals";
                        break;
                }

                // Construim calea spre directorul Resurses/[Categoria]
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string imagesFolder = Path.Combine(baseDir, "Resurses", categoryFolder);

                // Verificăm dacă directorul există, dacă nu, încercăm un path absolut ca alternativă
                if (!Directory.Exists(imagesFolder))
                {
                    // Încercăm calea absolută bazată pe exemplul tău
                    imagesFolder = $@"C:\Users\palat\OneDrive\Documente\Desktop\A2S2\MVP\MemoryGame\MemoryGame\Resurses\{categoryFolder}";

                    if (!Directory.Exists(imagesFolder))
                    {
                        MessageBox.Show($"Directorul de imagini pentru categoria {categoryFolder} nu există la locația: {imagesFolder}",
                                        "Avertisment", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                // Obținem toate fișierele imagine din director
                string[] imageFiles = Directory.GetFiles(imagesFolder, "*.jpg")
                                        .Concat(Directory.GetFiles(imagesFolder, "*.png"))
                                        .Concat(Directory.GetFiles(imagesFolder, "*.gif"))
                                        .ToArray();

                if (imageFiles.Length == 0)
                {
                    MessageBox.Show($"Nu s-au găsit imagini în categoria {categoryFolder} la locația: {imagesFolder}",
                                    "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _imageFiles.AddRange(imageFiles);

                // Afișăm un mesaj pentru debug
                //MessageBox.Show($"S-au încărcat {_imageFiles.Count} imagini din directorul: {imagesFolder}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la încărcarea imaginilor: {ex.Message}",
                                "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeCards()
        {
            Cards.Clear();

            int totalCards = Rows * Columns;
            int pairsNeeded = totalCards / 2;

            // Verificăm dacă avem suficiente imagini
            if (_imageFiles.Count < pairsNeeded && _imageFiles.Count > 0)
            {
                // Dacă nu avem destule imagini, folosim ce avem și le repetăm
                while (_imageFiles.Count < pairsNeeded)
                {
                    _imageFiles.AddRange(_imageFiles.Take(Math.Min(pairsNeeded - _imageFiles.Count, _imageFiles.Count)));
                }
            }
            else if (_imageFiles.Count == 0)
            {
                // Dacă nu avem imagini deloc, folosim valori numerice
                for (int i = 0; i < pairsNeeded; i++)
                {
                    _imageFiles.Add($"Value_{i}");
                }
            }

            // Luăm doar numărul de imagini de care avem nevoie
            var selectedImages = _imageFiles.Take(pairsNeeded).ToList();

            // Creăm două carduri pentru fiecare imagine (pentru perechi)
            List<Card> allCards = new List<Card>();

            for (int i = 0; i < pairsNeeded; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    allCards.Add(new Card
                    {
                        Id = i * 2 + j,
                        ImagePath = selectedImages[i],
                        Value = i,
                        IsFlipped = false,
                        IsMatched = false
                    });
                }
            }

            // Amestecăm cardurile
            ShuffleCards(allCards);

            // Adăugăm cardurile în colecția observabilă
            foreach (var card in allCards)
            {
                Cards.Add(card);
            }
        }

        private void ShuffleCards(List<Card> cards)
        {
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                Card temp = cards[k];
                cards[k] = cards[n];
                cards[n] = temp;
            }
        }

        private void StartTimer()
        {
            _gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            _gameTimer.Tick += (s, e) =>
            {
                if (RemainingTimeInSeconds > 0)
                {
                    RemainingTimeInSeconds--;
                }
                else
                {
                    _gameTimer.Stop();
                    EndGame(false);
                }
            };

            _gameTimer.Start();
        }

        private void EndGame(bool isWin)
        {
            _gameTimer.Stop();
            GameEnded = true;

            // Actualizăm statisticile
            CurrentPlayer.GamesPlayed++;
            if (isWin)
                CurrentPlayer.GamesWon++;

            // Salvăm utilizatorul
            var users = _userService.LoadUsers();
            var userIndex = users.FindIndex(u => u.Username == CurrentPlayer.Username);
            if (userIndex >= 0)
            {
                users[userIndex] = CurrentPlayer;
                _userService.SaveUsers(users);
            }

            // Afișăm mesajul de final
            string message = isWin
                ? $"Felicitări, {CurrentPlayer.Username}! Ai câștigat jocul!\nScor: {Score}"
                : $"Timpul a expirat! Ai pierdut jocul, {CurrentPlayer.Username}.\nScor: {Score}";

            MessageBox.Show(message, "Joc terminat", MessageBoxButton.OK, isWin ? MessageBoxImage.Information : MessageBoxImage.Exclamation);
        }

        private bool CanCardClick(object parameter)
        {
            if (parameter is Card card)
            {
                return !card.IsFlipped && !card.IsMatched && !_isCheckingMatch && !GameEnded;
            }
            return false;
        }

        private void CardClick(object parameter)
        {
            if (!(parameter is Card card) || _isCheckingMatch || GameEnded)
                return;

            // Verificăm dacă cardul este deja întors sau potrivit
            if (card.IsFlipped || card.IsMatched)
                return;

            // Întoarcem cardul
            card.IsFlipped = true;

            // Verificăm dacă este primul sau al doilea card întors
            if (_firstSelectedCard == null)
            {
                _firstSelectedCard = card;
            }
            else
            {
                // Verificăm să nu fie același card cumva
                if (_firstSelectedCard == card)
                    return;

                _secondSelectedCard = card;
                _isCheckingMatch = true;

                // Verificăm dacă cele două carduri formează o pereche
                // Folosim Dispatcher pentru a permite UI-ului să se actualizeze înainte de verificare
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    CheckForMatch();
                }), System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        private void CheckForMatch()
        {
            if (_firstSelectedCard == null || _secondSelectedCard == null)
            {
                _isCheckingMatch = false;
                return;
            }

            bool isMatch = _firstSelectedCard.Value == _secondSelectedCard.Value;

            if (isMatch)
            {
                // Cardurile se potrivesc
                _firstSelectedCard.IsMatched = true;
                _secondSelectedCard.IsMatched = true;
                Score += 10; // Adăugăm puncte

                // Verificăm dacă toate cardurile au fost potrivite
                if (Cards.All(c => c.IsMatched))
                {
                    EndGame(true);
                }

                // Resetăm selecția
                _firstSelectedCard = null;
                _secondSelectedCard = null;
                _isCheckingMatch = false;
            }
            else
            {
                // Cardurile nu se potrivesc - folosim un timer în loc de Thread.Sleep
                var timer = new System.Windows.Threading.DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(500) // 0.5 secunde
                };

                timer.Tick += (s, e) =>
                {
                    // Oprim timer-ul după prima execuție
                    timer.Stop();

                    // Verificăm din nou dacă referințele sunt valide
                    if (_firstSelectedCard != null && _secondSelectedCard != null)
                    {
                        // Întoarcem cardurile cu fața în jos
                        _firstSelectedCard.IsFlipped = false;
                        _secondSelectedCard.IsFlipped = false;
                    }

                    // Resetăm selecția
                    _firstSelectedCard = null;
                    _secondSelectedCard = null;
                    _isCheckingMatch = false;
                };

                // Pornim timer-ul
                timer.Start();
            }
        }

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
                    GameState gameState = new GameState
                    {
                        PlayerName = CurrentPlayer.Username,
                        Category = Category,
                        Rows = Rows,
                        Columns = Columns,
                        TotalTime = TotalTimeInSeconds,
                        RemainingTime = RemainingTimeInSeconds,
                        SavedAt = DateTime.Now,
                        Cards = Cards.Select((card, index) => new CardState
                        {
                            Id = card.Id,
                            Value = card.Value,
                            ImagePath = card.ImagePath,
                            IsFlipped = card.IsFlipped,
                            IsMatched = card.IsMatched,
                            Position = index
                        }).ToList()
                    };

                    string json = JsonSerializer.Serialize(gameState, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(saveFileDialog.FileName, json);

                    MessageBox.Show($"Jocul a fost salvat cu succes în fișierul: {saveFileDialog.FileName}",
                                    "Salvare reușită", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la salvarea jocului: {ex.Message}",
                                "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static GameBoardViewModel LoadGame(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Fișierul de salvare nu există.",
                                    "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

                string json = File.ReadAllText(filePath);
                GameState gameState = JsonSerializer.Deserialize<GameState>(json);

                if (gameState == null)
                {
                    MessageBox.Show("Fișierul de salvare este invalid.",
                                    "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

                // Obținem utilizatorul
                var userService = new UserService();
                var users = userService.LoadUsers();
                var player = users.FirstOrDefault(u => u.Username == gameState.PlayerName);

                if (player == null)
                {
                    MessageBox.Show($"Utilizatorul {gameState.PlayerName} nu există. Vă rugăm să creați acest utilizator mai întâi.",
                                    "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

                // Creăm view model-ul
                var viewModel = new GameBoardViewModel(player, gameState.Category, gameState.Rows, gameState.Columns, gameState.TotalTime);

                // Setăm timpul rămas
                viewModel.RemainingTimeInSeconds = gameState.RemainingTime;

                // Actualizăm cardurile
                viewModel.Cards.Clear();
                foreach (var cardState in gameState.Cards.OrderBy(c => c.Position))
                {
                    viewModel.Cards.Add(new Card
                    {
                        Id = cardState.Id,
                        Value = cardState.Value,
                        ImagePath = cardState.ImagePath,
                        IsFlipped = cardState.IsFlipped,
                        IsMatched = cardState.IsMatched
                    });
                }

                return viewModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la încărcarea jocului: {ex.Message}",
                                "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private void Exit()
        {
            // Oprim timerul
            _gameTimer?.Stop();

            // Întrebăm utilizatorul dacă vrea să salveze jocul înainte de a ieși
            if (!GameEnded)
            {
                var result = MessageBox.Show("Doriți să salvați jocul înainte de a ieși?",
                                            "Salvare joc",
                                            MessageBoxButton.YesNoCancel,
                                            MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    SaveGame();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    // Repornim timerul și anulăm ieșirea
                    _gameTimer?.Start();
                    return;
                }
            }

            // Navigăm înapoi la ecranul de configurare joc
            Messenger.Default.Send(new NavigationMessage { Destination = "GameView", Parameter = CurrentPlayer });
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}