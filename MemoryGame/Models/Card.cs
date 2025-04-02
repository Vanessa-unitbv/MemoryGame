using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MemoryGame.Models
{
    /// <summary>
    /// Reprezintă un jeton/card din joc
    /// </summary>
    public class Card : INotifyPropertyChanged
    {
        private bool _isFlipped;
        private bool _isMatched;
        private string _imagePath;
        private int _id;

        /// <summary>
        /// Identificatorul unic al cardului
        /// </summary>
        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Calea către imaginea cardului
        /// </summary>
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                if (_imagePath != value)
                {
                    _imagePath = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Indică dacă cardul este întors (cu fața în sus)
        /// </summary>
        public bool IsFlipped
        {
            get => _isFlipped;
            set
            {
                if (_isFlipped != value)
                {
                    _isFlipped = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Indică dacă cardul a fost deja potrivit cu perechea sa
        /// </summary>
        public bool IsMatched
        {
            get => _isMatched;
            set
            {
                if (_isMatched != value)
                {
                    _isMatched = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Indică valoarea cardului (folosită pentru a găsi perechi)
        /// </summary>
        public int Value { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}