namespace MicrophoneLevelLogger.Client.Controller.SelectSpeaker;

public interface ISelectSpeakerView
{
    bool TrySelectSpeaker(IEnumerable<ISpeaker> speakers, ISpeaker current, out ISpeaker selected);
}