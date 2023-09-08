namespace Specter.ViewModel.AnalysisPage;

public class AnalysisDeviceViewModel
{
    private readonly IDecibelsReaderProvider _decibelsReaderProvider;
    public AnalysisDeviceViewModel(
        AudioRecordViewModel audioRecord, 
        DeviceRecordViewModel deviceRecord, 
        IDecibelsReaderProvider decibelsReaderProvider)
    {
        AudioRecord = audioRecord;
        DeviceRecord = deviceRecord;
        _decibelsReaderProvider = decibelsReaderProvider;
    }
    public AudioRecordViewModel AudioRecord { get; }
    public DeviceRecordViewModel DeviceRecord { get; }
    public DateTime StartTime => AudioRecord.StartTime;
    public string Device => DeviceRecord.Name;
    public Direction Direction => AudioRecord.Direction;
    public Decibel Min => DeviceRecord.Min;
    public Decibel Avg => DeviceRecord.Avg;
    public Decibel Max => DeviceRecord.Max;
    public double Minus30db => DeviceRecord.Minus30db;
    public double Minus40db => DeviceRecord.Minus40db;
    public double Minus50db => DeviceRecord.Minus50db;

    public bool Analysis
    {
        get => DeviceRecord.Analysis;
        set => DeviceRecord.Analysis = value;
    }

    public IEnumerable<Decibel> GetInputLevels()
    {
        var reader = _decibelsReaderProvider.Provide(
            AudioRecord.AudioRecord,
            DeviceRecord.DeviceRecord);
        return reader.Read();
    }
}