namespace Specter;

public interface IDecibelsWriter : IDisposable
{
    public void Write(Decibel decibel);
}