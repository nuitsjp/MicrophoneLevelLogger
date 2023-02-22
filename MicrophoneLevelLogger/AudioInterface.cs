﻿using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace MicrophoneLevelLogger;

public class AudioInterface : IAudioInterface
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly IReadOnlyList<IMicrophone> _microphones;
    public AudioInterface(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
        _microphones = LoadAllMicrophones(_settingsRepository.Load()).ToList();
    }

    public AudioInterface(ISettingsRepository settingsRepository, params IMicrophone[] microphones)
    {
        _settingsRepository = settingsRepository;
        _microphones = microphones;
    }

    private IEnumerable<Microphone> LoadAllMicrophones(Settings settings)
    {
        using var enumerator = new MMDeviceEnumerator();
        var mmDevices = enumerator
            .EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active)
            .ToArray();
        try
        {
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                var capability = WaveIn.GetCapabilities(i);
                var name = capability.ProductName;
                // 名称が長いとWaveIn側の名前は途中までしか取得できないため、前方一致で判定する
                var mmDevice = mmDevices.SingleOrDefault(x => x.FriendlyName.StartsWith(name));
                if (mmDevice is not null)
                {
                    var microphoneId = new MicrophoneId(mmDevice.ID);
                    var alias = settings.Aliases.SingleOrDefault(x => x.Id == microphoneId)?.Name ?? mmDevice.FriendlyName;
                    yield return new Microphone(
                        microphoneId,
                        alias,
                        mmDevice.FriendlyName, 
                        i,
                        settings.DisabledMicrophones.NotContains(microphoneId) 
                            ? MicrophoneStatus.Enable 
                            : MicrophoneStatus.Disable);
                }
            }
        }
        finally
        {
            foreach (var mmDevice in mmDevices)
            {
                mmDevice.DisposeQuiet();
            }
        }

    }

    public void Dispose()
    {
        foreach (var microphone in GetMicrophones())
        {
            try
            {
                microphone.Dispose();
            }
            catch
            {
                // ignore
            }
        }
        GC.SuppressFinalize(this);
    }

    public IEnumerable<IMicrophone> GetMicrophones(MicrophoneStatus status = MicrophoneStatus.Enable) =>
        _microphones.Where(x => status.HasFlag(x.Status));

    public async Task<ISpeaker> GetSpeakerAsync()
    {
        var settings = await _settingsRepository.LoadAsync();
        using var emurator = new MMDeviceEnumerator();
        if (settings.SelectedSpeakerId is null)
        {
            using var mmDevice = emurator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            return new Speaker(new SpeakerId(mmDevice.ID), mmDevice.FriendlyName);
        }
        else
        {
            using var mmDevice = emurator.GetDevice(settings.SelectedSpeakerId?.AsPrimitive());
            return new Speaker(new SpeakerId(mmDevice.ID), mmDevice.FriendlyName);
        }
    }

    public IEnumerable<ISpeaker> GetSpeakers()
    {
        using var emurator = new MMDeviceEnumerator();
        var mmDevices = emurator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
        foreach (var mmDevice in mmDevices)
            using (mmDevice)
            {
                yield return new Speaker(new SpeakerId(mmDevice.ID), mmDevice.FriendlyName);
            }
    }

    public void ActivateMicrophones()
    {
        var tasks = GetMicrophones()
            .Select(x => x.ActivateAsync());
        Task.WaitAll(tasks.ToArray());
    }
}