using NAudio.CoreAudioApi;

namespace Specter.Business;

public record WaveRecordIndex(
    DeviceId DeviceId,
    DataFlow DataFlow,
    string Name,
    string SystemName);