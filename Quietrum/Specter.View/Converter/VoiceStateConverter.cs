using System;
using System.Globalization;
using System.Windows.Data;

namespace Specter.View.Converter;

public class VoiceStateConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is VoiceState voiceState)
        {
            return voiceState == VoiceState.With;
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool voiceState)
        {
            return voiceState ? VoiceState.With : VoiceState.Without;
        }

        return null;
    }
}