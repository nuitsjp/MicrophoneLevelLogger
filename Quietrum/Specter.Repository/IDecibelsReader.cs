using Specter.Business;

namespace Specter.Repository;

public interface IDecibelsReader
{
    public IEnumerable<Decibel> Read();
}