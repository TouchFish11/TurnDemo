namespace HotUpdate.Core.Battle.Condition
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
