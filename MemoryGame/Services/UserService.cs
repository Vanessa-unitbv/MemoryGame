using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryGame.Models;
using System.IO;
using System.Text.Json;

namespace MemoryGame.Services
{
    public class UserService
    {
        private const string UsersFileName = "users.json";
        private string _filePath;

        public UserService()
        {
            // Folosim un path relativ față de directorul executabil
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _filePath = Path.Combine(baseDir, UsersFileName);
        }

        public List<User> LoadUsers()
        {
            if (!File.Exists(_filePath))
                return new List<User>();

            string json = File.ReadAllText(_filePath);
            var users = string.IsNullOrEmpty(json) ? new List<User>() : JsonSerializer.Deserialize<List<User>>(json);

            // Convertim căile relative la căi absolute pentru afișare
            foreach (var user in users)
            {
                if (!string.IsNullOrEmpty(user.ImagePath) && !Path.IsPathRooted(user.ImagePath))
                {
                    // Dacă avem o cale relativă, o facem absolută
                    user.ImagePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, user.ImagePath));
                }
            }

            return users;
        }

        public void SaveUsers(List<User> users)
        {
            // Facem o copie a listei pentru a nu modifica referințele originale
            var usersToSave = new List<User>();
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            foreach (var user in users)
            {
                // Creăm un nou obiect User cu aceleași proprietăți
                var userCopy = new User
                {
                    Username = user.Username,
                    GamesPlayed = user.GamesPlayed,
                    GamesWon = user.GamesWon
                };

                // Dacă există o cale de imagine, o convertim la relativă dacă e cazul
                if (!string.IsNullOrEmpty(user.ImagePath))
                {
                    if (Path.IsPathRooted(user.ImagePath) && user.ImagePath.StartsWith(baseDir, StringComparison.OrdinalIgnoreCase))
                    {
                        // Obținem calea relativă prin eliminarea directorului de bază
                        userCopy.ImagePath = user.ImagePath.Substring(baseDir.Length);

                        // Eliminăm orice separator de director de la început dacă există
                        if (userCopy.ImagePath.StartsWith("\\") || userCopy.ImagePath.StartsWith("/"))
                        {
                            userCopy.ImagePath = userCopy.ImagePath.Substring(1);
                        }
                    }
                    else
                    {
                        // Păstrăm calea așa cum este (poate fi deja relativă)
                        userCopy.ImagePath = user.ImagePath;
                    }
                }

                usersToSave.Add(userCopy);
            }

            string json = JsonSerializer.Serialize(usersToSave, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        private string ConvertToRelativePath(string absolutePath, string basePath)
        {
            if (string.IsNullOrEmpty(absolutePath) || !Path.IsPathRooted(absolutePath))
                return absolutePath;

            // Verifică dacă calea începe cu directorul de bază
            if (absolutePath.StartsWith(basePath))
            {
                return absolutePath.Substring(basePath.Length);
            }

            return absolutePath; // Returnează calea originală dacă nu poate fi convertită
        }

        public void AddUser(User newUser)
        {
            var users = LoadUsers();

            // Verifică dacă utilizatorul există deja
            if (users.Any(u => u.Username == newUser.Username))
                throw new Exception("Utilizatorul există deja!");

            users.Add(newUser);
            SaveUsers(users);
        }

        public void DeleteUser(string username)
        {
            var users = LoadUsers();
            users.RemoveAll(u => u.Username == username);
            SaveUsers(users);
        }
        public bool UserExists(string username)
        {
            var users = LoadUsers();
            return users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }
    }
}
