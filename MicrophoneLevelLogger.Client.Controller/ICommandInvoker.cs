namespace MicrophoneLevelLogger.Client.Controller;

/// <summary>
/// MicrophoneLevelLogger全体をコントロールし実行する。
/// </summary>
public interface ICommandInvoker
{
    Task InvokeAsync();
}

