using System.Text.Json;

namespace Quietrum.Repository;

public abstract class RepositoryBase<T> where T : class
{
    private static readonly AsyncLock Lock = new();
    protected async Task<T> LoadAsync(FileInfo fileInfo, Func<T> getDefault)
    {
        using (await Lock.LockAsync())
        {
            if (fileInfo.Exists is false)
            {
                await SaveAsync(fileInfo, getDefault());
            }

            await using var stream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
            return (await JsonSerializer.DeserializeAsync<T>(stream, JsonEnvironments.Options))!;
        }
    }
    
    protected async Task SaveAsync(FileInfo fileInfo, T value)
    {
        using (await Lock.LockAsync())
        {
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            await using var stream = new FileStream(fileInfo.FullName, FileMode.Create, FileAccess.Write);
            await JsonSerializer.SerializeAsync(stream, value, JsonEnvironments.Options);
        }
    }
}