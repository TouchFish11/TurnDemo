using Core.Reflection;
using GameHotUpdate.Battle.Object;

namespace GameHotUpdate.Battle.Status
{
    public interface IStatusFactory : IFactory
    {
        /// <summary>
        /// 根据状态ID创建对应的状态实例
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="statusId">状态唯一标识ID</param>
        /// <param name="sorucer"></param>
        /// <returns>实现IStatus接口的状态实例；若未找到对应ID的状态类，返回null</returns>
        IStatus GetStatus(IBattleEntityObject sorucer, IBattleEntityObject owner,int statusId);
    }
}
