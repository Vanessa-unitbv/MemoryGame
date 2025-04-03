using System;
using System.Collections.Generic;

namespace MemoryGame.Models
{
    /// <summary>
    /// Clasa pentru stocarea stării jocului, folosită pentru salvarea și încărcarea jocurilor
    /// </summary>
    public class GameState
    {
        /// <summary>
        /// Numele utilizatorului care joacă
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// Categoria de imagini aleasă (1, 2 sau 3)
        /// </summary>
        public int Category { get; set; }

        /// <summary>
        /// Numărul de rânduri de pe tabla de joc
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// Numărul de coloane de pe tabla de joc
        /// </summary>
        public int Columns { get; set; }

        /// <summary>
        /// Timpul total alocat jocului (în secunde)
        /// </summary>
        public int TotalTime { get; set; }

        /// <summary>
        /// Timpul rămas la momentul salvării (în secunde)
        /// </summary>
        public int RemainingTime { get; set; }

        /// <summary>
        /// Data și ora la care a fost salvat jocul
        /// </summary>
        public DateTime SavedAt { get; set; }

        /// <summary>
        /// Calea către fișierul de salvare (nu este serializată)
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public string FilePath { get; set; }

        /// <summary>
        /// Lista cardurilor și starea lor la momentul salvării
        /// </summary>
        /// 
        public bool IsCompleted { get; set; }
        /// <summary>
        /// Indică dacă cardul este întors (cu fața în sus)
        /// </summary>
        /// 
        public List<CardState> Cards { get; set; } = new List<CardState>();

        [System.Text.Json.Serialization.JsonIgnore]
        public string GameMode
        {
            get
            {
                return (Rows == 4 && Columns == 4) ? "Standard" : "Custom";
            }
        }
    }


    /// <summary>
    /// Clasa pentru stocarea stării unui card, folosită la salvarea jocului
    /// </summary>
    public class CardState
    {
        /// <summary>
        /// Identificatorul cardului
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Valoarea cardului (folosită pentru a găsi perechi)
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Calea către imaginea cardului
        /// </summary>
        public string ImagePath { get; set; }


        /// <summary>
        /// Indică dacă jocul a fost terminat (câștigat sau pierdut)
        /// </summary>
        public bool IsFlipped { get; set; }

        /// <summary>
        /// Indică dacă cardul a fost deja potrivit cu perechea sa
        /// </summary>
        public bool IsMatched { get; set; }

        /// <summary>
        /// Poziția cardului pe tabla de joc (index în grila de joc)
        /// </summary>
        public int Position { get; set; }
    }
}