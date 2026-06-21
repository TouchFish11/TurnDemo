using System.Collections;

namespace HotUpdate.Game.Battle.Object
{
    /// <summary>
    /// 实体对象战斗死亡处理器
    /// </summary>
    public interface IDeathHandler
    {
        /// <summary>
        /// 初始化实体
        /// </summary>
        /// <param name="entity"></param>
        void InitEntity(IBattleEntityObject entity);
        
        /// <summary>
        /// 处理死亡逻辑
        /// </summary>
        /// <returns></returns>
        public IEnumerator HandleDeath();

    }
}
