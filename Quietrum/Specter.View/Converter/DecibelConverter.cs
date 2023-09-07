using System;
using System.Globalization;
using System.Windows.Data;
using Specter.Business;

namespace Specter.View.Converter;

public class DecibelConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Decibel decibel)
        {
            string formatString = parameter as string ?? "{0}";
            return string.Format(formatString, decibel.AsPrimitive());
        }

        return null;
    }

    public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}