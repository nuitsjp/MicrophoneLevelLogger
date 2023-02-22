using UnitGenerator;

namespace MicrophoneLevelLogger;

/// <summary>
/// スピーカーID
/// </summary>
[UnitOf(typeof(string), UnitGenerateOptions.JsonConverter)]
public partial struct SpeakerId
{
}