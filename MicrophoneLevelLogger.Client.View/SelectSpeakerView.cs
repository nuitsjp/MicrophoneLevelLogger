using MicrophoneLevelLogger.Client.Controller.SelectSpeaker;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

/// <summary>
/// スピーカー選択ビュー
/// </summary>
public class SelectSpeakerView : ISelectSpeakerView
{
    /// <summary>
    /// スピーカーを選択する。
    /// </summary>
    /// <param name="speakers"></param>
    /// <param name="current"></param>
    /// <param name="selected"></param>
    /// <returns></returns>
    public bool TrySelectSpeaker(IEnumerable<ISpeaker> speakers, ISpeaker current, out ISpeaker selected)
    {
        const string cancel = "取りやめる";

        var speakerList = speakers.ToList();
        var items = speakerList
            .Select(x => x.Name)
            .ToList();
        items.Add(cancel);

        var item = Prompt.Select("有効化するマイクを選択してください。", items, defaultValue: current.Name);
        if (item == cancel)
        {
            selected = default!;
            return false;
        }

        selected = speakerList.Single(x => x.Name == item);
        return true;
    }
}