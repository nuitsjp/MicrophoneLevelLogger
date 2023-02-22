namespace MicrophoneLevelLogger;

/// <summary>
/// マイクの名称に対する別名
/// </summary>
public class Alias
{
    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="id"></param>
    /// <param name="systemName"></param>
    /// <param name="name"></param>
    public Alias(MicrophoneId id, string systemName, string name)
    {
        Id = id;
        SystemName = systemName;
        Name = name;
    }

    /// <summary>
    /// マイクのID
    /// </summary>
    public MicrophoneId Id { get; }
    /// <summary>
    /// マイクのOS上の名称
    /// </summary>
    public string SystemName { get; }
    /// <summary>
    /// このツール上で利用する名称。SystemNameの別名。
    /// </summary>
    public string Name { get; }
}