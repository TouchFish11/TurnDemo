namespace HotUpdate.Game.Battle.Object.Conditions
{
    public interface IDeathCondition
    {
        /// <summary>
        /// 能否死亡
        /// </summary>
        /// <param name="battleEntity"></param>
        /// <returns></returns>
        bool CanDie(IBattleEntityObject battleEntity);
    }
}
