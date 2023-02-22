namespace MicrophoneLevelLogger.Client.Controller;

public class CompositeController : IController
{
    private readonly ICompositeControllerView _view;

    public CompositeController(
        string name,
        string description,
        ICompositeControllerView view)
    {
        Name = name;
        Description = description;
        _view = view;
    }

    public CompositeController(
        ICompositeControllerView view)
    {
        Name = string.Empty;
        Description = string.Empty;
        _view = view;
    }

    public string Name { get; }
    public string Description { get; }
    public List<IController> Controllers { get; } = new();

    public CompositeController AddController(IController controller)
    {
        Controllers.Add(controller);
        return this;
    }

    public async Task ExecuteAsync()
    {
        while (true)
        {
            if (_view.TrySelectController(this, out var selected))
            {
                await selected.ExecuteAsync();
            }
            else
            {
                break;
            }
        }
    }
}