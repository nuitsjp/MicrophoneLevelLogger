using Specter.Business;

namespace Specter.Repository;

public interface IDecibelsWriter : IDisposable
{
    public void Write(Decibel decibel);
}