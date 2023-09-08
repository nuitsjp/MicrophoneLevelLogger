using System.Reactive.Disposables;
using System.Text.Json;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Specter.Repository;

public class AudioRecordInterface : IAudioRecordInterface, IDisposable
{
    private static readonly string RootDirectory = "Record";
    private static readonly string AudioRecordFile = "AudioRecord.json";

    private readonly CompositeDisposable _compositeDisposable = new();
    private readonly ReactiveCollection<AudioRecord> _audioRecords = new();
    private readonly FileSystemWatcher _fileSystemWatcher = new();
    public ReadOnlyReactiveCollection<AudioRecord> AudioRecords { get; }

    public AudioRecordInterface()
    {
        _audioRecords.AddTo(_compositeDisposable);
        _fileSystemWatcher.AddTo(_compositeDisposable);
        
        AudioRecords = _audioRecords.ToReadOnlyReactiveCollection();
    }

    public bool Activated { get; private set; }

    public async Task ActivateAsync()
    {
        if (Activated) return;
        
        var records = await LoadAsync();
        foreach (var record in records)
        {
            _audioRecords.Add(record);
        }
        
        // 監視するディレクトリを設定
        _fileSystemWatcher.Path = RootDirectory;

        // 監視するファイルのパターン（この場合は.txtファイル）
        _fileSystemWatcher.Filter = $@"*.json";
        
        // サブディレクトリも監視する
        _fileSystemWatcher.IncludeSubdirectories = true;


        // イベントハンドラを追加
        _fileSystemWatcher.Created += FileSystemWatcherOnCreated;
        _fileSystemWatcher.Deleted += FileSystemWatcherOnDeleted;

        // 監視を開始
        _fileSystemWatcher.EnableRaisingEvents = true;

        Activated = true;
    }

    private void FileSystemWatcherOnDeleted(object sender, FileSystemEventArgs e)
    {
    }

    private async void FileSystemWatcherOnCreated(object sender, FileSystemEventArgs e)
    {
        _audioRecords.Add(await LoadAsync(e.FullPath));
    }

    public async Task<IEnumerable<AudioRecord>> LoadAsync()
    {
        var directories = Directory.GetDirectories(RootDirectory);

        List<AudioRecord> records = new();
        foreach (var directory in directories)
        {
            var file = Path.Combine(directory, AudioRecordFile);
            if (File.Exists(file) is false)
                continue;
            
            records.Add(await LoadAsync(file));
        }

        return records;
    }

    private async Task<AudioRecord> LoadAsync(string file)
    {
        await using var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
        return await JsonSerializer.DeserializeAsync<AudioRecord>(stream, JsonEnvironments.Options);
    }

    public void Dispose()
    {
        _compositeDisposable.Dispose();
    }
}