using Game.Battle.Context;

namespace Game.Battle.Condition
{
    /// <summary>
    /// ս����������
    /// </summary>
    public interface IBattleOverCondition
    {
        /// <summary>
        /// ������
        /// </summary>
        /// <returns>trueΪ����</returns>
        bool CheckOver(IBattleContext context);
    }
}
