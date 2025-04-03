using System;
using System.Globalization;
using System.Windows.Data;

namespace MemoryGame.Converters
{
    /// <summary>
    /// Convertește o valoare null în false și orice altceva în true
    /// </summary>
    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}