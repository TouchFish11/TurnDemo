using HotUpdate.Battle.Context;

namespace HotUpdate.Battle.Condition
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
