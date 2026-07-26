namespace HotUpdate.Game.Slot
{
    public interface IGridSelectable<out T> : IGridInteractive<T>
    {
        bool Selected { get; set; }
    }
}
