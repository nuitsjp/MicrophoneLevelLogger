namespace Specter.Repository;

public class DecibelsWriter : IDecibelsWriter
{
    private readonly BinaryWriter _writer;

    public DecibelsWriter(Stream stream)
    {
        _writer = new(stream);
    }
    
    public void Dispose()
    {
        _writer.Flush();
        _writer.Close();
        _writer.Dispose();
    }

    public void Write(Decibel decibel)
    {
        _writer.Write(decibel.AsPrimitive());
    }
}