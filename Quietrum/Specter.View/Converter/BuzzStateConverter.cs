﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Specter.View.Converter;

public class BuzzStateConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BuzzState buzzState)
        {
            return buzzState == BuzzState.With;
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool buzzState)
        {
            return buzzState ? BuzzState.With : BuzzState.Without;
        }

        return null;
    }
}