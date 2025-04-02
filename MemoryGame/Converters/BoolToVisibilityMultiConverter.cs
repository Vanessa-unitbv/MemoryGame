using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MemoryGame.Converters
{
    /// <summary>
    /// Converter pentru a transforma un boolean în Visibility, cu opțiunea de a inversa rezultatul
    /// </summary>
    public class BoolToVisibilityMultiConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                // Parametrul 'parameter' poate fi folosit pentru a inversa logic
                bool shouldInvert = parameter != null && parameter.ToString() == "Invert";

                boolValue = shouldInvert ? !boolValue : boolValue;

                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                bool result = visibility == Visibility.Visible;
                bool shouldInvert = parameter != null && parameter.ToString() == "Invert";

                return shouldInvert ? !result : result;
            }

            return false;
        }
    }
}