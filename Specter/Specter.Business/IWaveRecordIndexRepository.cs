namespace Specter.Business;

public interface IWaveRecordIndexRepository
{
    Task SaveAsync(FileInfo fileInfo, WaveRecordIndex waveRecordIndex);
}