using Specter.Business;

namespace Specter.Repository;

public class DecibelsReader : IDecibelsReader
{
    private readonly BinaryReader _reader;

    public DecibelsReader(Stream stream)
    {
        _reader = new(stream);
    }
    
    public IEnumerable<Decibel> Read()
    {
        while (_reader.BaseStream.Position < _reader.BaseStream.Length)
        {
            yield return new Decibel(_reader.ReadDouble());
        }
    }
}