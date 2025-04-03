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
            return string.IsNullOrEmpty(json)? new List<User>(): JsonSerializer.Deserialize<List<User>>(json);
        }

        public void SaveUsers(List<User> users)
        {
            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
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
