namespace Specter.Repository;

public interface IWaveWriter : IDisposable
{
    void Write(byte[] bytes, int offset, int count);
}