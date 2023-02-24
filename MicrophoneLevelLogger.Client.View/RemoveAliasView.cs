using MicrophoneLevelLogger.Client.Controller.RemoveAlias;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

/// <summary>
/// 別名削除ビュー
/// </summary>
public class RemoveAliasView : IRemoveAliasView
{
    /// <summary>
    /// 削除する別名を選択する。
    /// </summary>
    /// <param name="settings"></param>
    /// <param name="alias"></param>
    /// <returns></returns>
    public bool TrySelectRemoveAlias(Settings settings, out Alias alias)
    {
        if (settings.Aliases.Empty())
        {
            // ReSharper disable once LocalizableElement
            Console.WriteLine("別名は設定されていません。");
            alias = default!;
            return false;
        }

        const string cancel = "取りやめる";

        var items = settings.Aliases
            .Select(x => $"システム名 : {x.SystemName} 別名 : {x.Name}")
            .ToList();
        items.Add(cancel);

        var selected = Prompt.Select("削除する別名を選択してください。", items);
        if (selected == cancel)
        {
            alias = default!;
            return false;
        }

        alias = settings.Aliases[items.IndexOf(selected)];
        return true;
    }
}