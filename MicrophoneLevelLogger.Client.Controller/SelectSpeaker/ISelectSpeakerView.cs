namespace MicrophoneLevelLogger.Client.Controller.SelectSpeaker;

/// <summary>
/// スピーカー選択ビュー
/// </summary>
public interface ISelectSpeakerView
{
    /// <summary>
    /// スピーカーを選択する。
    /// </summary>
    /// <param name="speakers"></param>
    /// <param name="current"></param>
    /// <param name="selected"></param>
    /// <returns></returns>
    bool TrySelectSpeaker(IEnumerable<ISpeaker> speakers, ISpeaker current, out ISpeaker selected);
}