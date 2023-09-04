using Specter.Business;

namespace Specter.Repository;

/// <summary>
/// IDetailWriterファクトリー
/// </summary>
public class DetailRepositoryFactory : IDetailRepositoryFactory
{
    /// <summary>
    /// IDetailWriterを生成する。
    /// </summary>
    /// <param name="writer"></param>
    /// <returns></returns>
    public IDetailRepository Create(StreamWriter writer) => new DetailRepository(writer);
}