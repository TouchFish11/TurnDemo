namespace Game.Inputs
{
    public interface IPlayerInput : IInputEvent
    {
        // 每帧更新输入
        void OnUpdateInput();
    }
}
