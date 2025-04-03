using MemoryGame.Commands;
using System;

namespace MemoryGame.ViewModels
{
    /// <summary>
    /// ViewModel pentru fereastra About
    /// </summary>
    public class AboutViewModel : ViewModelBase
    {
        /// <summary>
        /// Comanda pentru închiderea ferestrei
        /// </summary>
        public RelayCommand CloseCommand { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        public AboutViewModel()
        {
            CloseCommand = new RelayCommand(_ => RequestClose?.Invoke());
        }

        /// <summary>
        /// Eveniment pentru a semnala închiderea ferestrei
        /// </summary>
        public event Action RequestClose;
    }
}