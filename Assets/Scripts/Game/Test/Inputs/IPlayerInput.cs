namespace Game.Test.Inputs
{
    public interface IPlayerInput : IInputEvent
    {
        // 每帧更新输入
        void OnUpdateInput();
    }
}
