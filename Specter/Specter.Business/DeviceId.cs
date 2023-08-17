using UnitGenerator;

namespace Specter.Business;

/// <summary>
/// マイクID
/// </summary>
[UnitOf(typeof(string), UnitGenerateOptions.JsonConverter)]
public partial struct DeviceId
{
    
}