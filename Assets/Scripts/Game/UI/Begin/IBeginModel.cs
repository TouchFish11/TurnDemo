using Core.UI.MVC;

namespace Game.UI.Begin
{
    public interface IBeginModel : IuiModel
    {
        float SilderProgress { get; set; }
        string TxtProgress { get; set; }
        string TxtPhase { get; set; }
        string TxtSize { get; set; }
        string TxtSpeed { get; set; }
        bool IsActiveProgress { get; set; }
    }
}
