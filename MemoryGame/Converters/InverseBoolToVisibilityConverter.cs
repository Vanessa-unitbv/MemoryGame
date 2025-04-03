﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MemoryGame.Converters
{
    /// <summary>
    /// Inversează valoarea unui boolean și o convertește în Visibility
    /// </summary>
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}