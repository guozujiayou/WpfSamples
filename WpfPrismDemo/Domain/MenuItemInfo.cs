using System.Windows;

namespace WpfPrismDemo.Domain;

public class MenuItemInfo
{
    public string? Name { get; set; }
    public string? Icon { get; set; }

    private object? _content;

    public object? Content => _content ??= CreateContent();

    //View对象
    private readonly Type _contentType;
    //DataContext
    private readonly object? _dataContext;

    public MenuItemInfo(string name, string icon, Type contentType, object? dataContext = null)
    {
        Name = name;
        Icon = icon;
        _contentType = contentType;
        _dataContext = dataContext;
    }

    private object? CreateContent()
    {
        var content = Activator.CreateInstance(_contentType);
        if (_dataContext != null && content is FrameworkElement element)
        {
            element.DataContext = _dataContext;
        }
        return content;
    }
}
