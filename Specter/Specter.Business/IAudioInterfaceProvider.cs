﻿namespace Specter.Business;

/// <summary>
/// IAudioInterfaceのプロバイダー
/// </summary>
public interface IAudioInterfaceProvider
{
    /// <summary>
    /// IAudioInterfaceを解決する。
    /// </summary>
    /// <returns></returns>
    Task<IAudioInterface> ResolveAsync();
}