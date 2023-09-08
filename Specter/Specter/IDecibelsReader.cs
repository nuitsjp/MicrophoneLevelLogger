namespace Specter;

public interface IDecibelsReader
{
    public IEnumerable<Decibel> Read();
}