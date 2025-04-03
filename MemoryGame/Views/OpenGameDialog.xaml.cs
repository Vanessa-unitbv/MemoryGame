using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using MemoryGame.Models;

namespace MemoryGame.Views
{
    public partial class OpenGameDialog : Window
    {
        public GameState SelectedGame { get; private set; }
        private List<GameState> _savedGames;

        public OpenGameDialog()
        {
            InitializeComponent();
            LoadSavedGames();
        }

        private void LoadSavedGames()
        {
            _savedGames = new List<GameState>();
            string saveDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SavedGames");

            if (Directory.Exists(saveDir))
            {
                var savedFiles = Directory.GetFiles(saveDir, "*.mem");

                foreach (var file in savedFiles)
                {
                    try
                    {
                        string json = File.ReadAllText(file);
                        var gameState = JsonSerializer.Deserialize<GameState>(json);

                        if (gameState != null)
                        {
                            // Adăugăm doar jocurile care nu sunt terminate
                            if (!gameState.IsCompleted)
                            {
                                gameState.FilePath = file;
                                _savedGames.Add(gameState);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // Ignorăm fișierele care nu pot fi deserializate
                    }
                }
            }

            // Sortăm jocurile după data salvării (cele mai recente primele)
            _savedGames = _savedGames.OrderByDescending(g => g.SavedAt).ToList();
            SavedGamesList.ItemsSource = _savedGames;

            // Dacă nu există jocuri salvate neterminate, afișăm un mesaj
            if (_savedGames.Count == 0)
            {
                MessageBox.Show("Nu există jocuri salvate neterminate.",
                              "Informație", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (SavedGamesList.SelectedItem is GameState selectedGame)
            {
                SelectedGame = selectedGame;
                DialogResult = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}