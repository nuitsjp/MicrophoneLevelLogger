namespace Specter.ViewModel.AnalysisPage;

public partial class DeviceRecordViewModel
{
    private readonly IAnalyzer _analyzer;
    private readonly DeviceRecord _deviceRecord;

    private bool _analysis;

    public DeviceRecordViewModel(DeviceRecord deviceRecord, IAnalyzer analyzer)
    {
        _deviceRecord = deviceRecord;
        _analyzer = analyzer;
    }

    public DeviceRecord DeviceRecord => _deviceRecord;
    public string Name => _deviceRecord.Name;
    public Decibel Min => _deviceRecord.Min;
    public Decibel Avg => _deviceRecord.Avg;
    public Decibel Max => _deviceRecord.Max;
    public double Minus30db => _deviceRecord.Minus30db;
    public double Minus40db => _deviceRecord.Minus40db;
    public double Minus50db => _deviceRecord.Minus50db;

    public bool Analysis
    {
        get => _analysis;
        set
        {
            _analysis = value;
            _analyzer.UpdateTarget(this);
        }
    }
}