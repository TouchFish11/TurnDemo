namespace HotUpdate.Game.Interact
{
    /// <summary>
    /// 交互策略接口
    /// </summary>
    public interface IInteractStrategy
    {
        public void Interact(IInteractable interactObject);
    }
}
