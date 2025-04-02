using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace MemoryGame.Converters
{
    /// <summary>
    /// Convertește un boolean în valoare de opacitate (0.3 pentru true, 1.0 pentru false)
    /// Folosit pentru a face cardurile potrivite semi-transparente
    /// </summary>
    public class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? 0.3 : 1.0;
            }
            return 1.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Inversează valoarea unui boolean
    /// </summary>
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return false;
        }
    }

    /// <summary>
    /// Verifică dacă un string este o cale validă către o imagine
    /// </summary>
    /// <summary>
    /// Verifică dacă un string este o cale validă către o imagine
    /// </summary>
    public class IsImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string path)
            {
                try
                {
                    // Verificăm dacă este o cale de fișier validă
                    if (File.Exists(path))
                    {
                        // Verificăm extensia fișierului
                        string extension = Path.GetExtension(path).ToLower();
                        return extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif" || extension == ".bmp";
                    }

                    // Dacă începe cu "Value_" sau nu conține extensia fișierului, este o valoare numerică
                    if (path.StartsWith("Value_") || !path.Contains("."))
                    {
                        return false;
                    }

                    // Pentru căi absolve din exemple, verificăm dacă teminația este o extensie de imagine
                    string fileName = Path.GetFileName(path);
                    if (fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg") ||
                        fileName.EndsWith(".png") || fileName.EndsWith(".gif") ||
                        fileName.EndsWith(".bmp"))
                    {
                        return true;
                    }
                }
                catch
                {
                    // În caz de excepție, considerăm că nu este o cale validă
                    return false;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}