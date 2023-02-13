namespace LogAggregator;

public class Microphone
{
    public static IReadOnlyList<Microphone> Microphones = new List<Microphone>
    {
        new("EarPods", "マイク (Realtek USB2.0 Audio)"),
        new("H111", "マイク (Realtek USB2.0 Audio)"),
        new("H340", "Logi USB Headset H340"),
        new("H390", "Logi USB Headset"),
        new("ATH", "マイク (USB PnP Sound Device)"),
        new("PC5", "マイク (Realtek USB2.0 Audio)"),
        new("Evolve20", "ヘッドセット マイク (Jabra EVOLVE 20 SE MS)"),
        new("Evolve2 30", "Microphone (Jabra Evolve2 30)"),
        new("H700", "マイク (Anker Soundsync)"),
        new("OpenComm", "ヘッドセット (OpenComm by Shokz Hands-Free AG Audio)"),
        new("Liberty", "ヘッドセット (Soundcore Liberty Air 2 Pro Hands-Free AG Audio)"),
        new("Recon Air", "マイク (Recon Air)"),
        new("tusq", "マイク (Realtek USB2.0 Audio)"),
        new("Air3", "ヘッドセット (SOUNDPEATS Air3 Deluxe HS Hands-Free AG Audio)"),
        new("Elite 85t", "ヘッドセット (Jabra Elite 85t Hands-Free AG Audio)"),
    };

    public static Microphone FindByProductName(string productName) =>
        Microphones.Single(x => x.ProductName == productName);
    private Microphone(string productName, string systemName)
    {
        ProductName = productName;
        SystemName = systemName;
    }

    /// <summary>
    /// 製品名（フォルダ名）
    /// </summary>
    public string ProductName { get; }
    /// <summary>
    /// Windowsに登録される名称
    /// </summary>
    public string SystemName { get; }
}