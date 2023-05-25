namespace Quietrum;

public class MicrophoneConfig
{
    public MicrophoneConfig(
        MicrophoneId id, 
        string name, 
        bool measure)
    {
        Id = id;
        Name = name;
        Measure = measure;
    }

    /// <summary>
    /// ID
    /// </summary>
    public MicrophoneId Id { get; }
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 計測するか、しないか取得する。
    /// </summary>
    public bool Measure { get; set; }
}