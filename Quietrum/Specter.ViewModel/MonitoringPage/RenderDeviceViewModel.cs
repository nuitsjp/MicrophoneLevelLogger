namespace Specter.ViewModel.MonitoringPage;

public partial class RenderDeviceViewModel
{
    public RenderDeviceViewModel(IRenderDevice renderDevice)
    {
        Device = renderDevice;
    }

    public DeviceId Id => Device.Id;
    public string Name => Device.Name;
    public IRenderDevice Device { get; }
    
    public void PlayLooping(CancellationToken token)
    {
        Device.PlayLooping(token);
    }
}