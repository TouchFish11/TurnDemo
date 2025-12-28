
/// <summary>
///  Û±Íπ‹¿Ì∆˜
/// </summary>
public interface IMouseManager
{
    bool Visible { get; }

    void ReleaseMouseVisible(string sorce);
    void RequestMouseVisible(string sorce);
}
