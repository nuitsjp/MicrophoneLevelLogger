using System.IO;
using System.Threading.Tasks;

namespace Specter.Business;

public interface IWaveRecordIndexRepository
{
    Task SaveAsync(FileInfo fileInfo, WaveRecordIndex waveRecordIndex);
}