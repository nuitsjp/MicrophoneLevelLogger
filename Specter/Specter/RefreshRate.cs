﻿namespace Specter;

public struct RefreshRate
{
    public int Value { get; }
    public TimeSpan Interval { get; }

    public RefreshRate(int value)
    {
        Value = value;
        Interval = TimeSpan.FromSeconds(1f / value);
    }
}