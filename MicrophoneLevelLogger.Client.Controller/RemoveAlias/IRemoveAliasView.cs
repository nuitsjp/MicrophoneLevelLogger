namespace MicrophoneLevelLogger.Client.Controller.RemoveAlias;

public interface IRemoveAliasView
{
    bool TrySelectRemoveAlias(Settings settings, out Alias alias);
}