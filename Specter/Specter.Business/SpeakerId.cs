using UnitGenerator;

namespace Specter.Business;

/// <summary>
/// スピーカーID
/// </summary>
[UnitOf(typeof(string), UnitGenerateOptions.JsonConverter)]
public partial struct SpeakerId
{
}