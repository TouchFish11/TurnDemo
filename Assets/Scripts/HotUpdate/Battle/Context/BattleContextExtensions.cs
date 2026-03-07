using System.Collections.Generic;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Object;

namespace HotUpdate.Battle.Context
{
    public static class BattleContextExtensions
    {
        /// <summary>
        /// 获取所有存活的实体
        /// </summary>
        /// <param name="context"></param>
        /// <param name="battleEntitys"></param>
        public static void GetAliveEntitys(this IBattleContext context, List<IBattleEntityObject>  battleEntitys)
        {
            foreach (var battleEntityObject in context.GetAliveEntitys())
            {
                battleEntitys.Add(battleEntityObject);
            }
        }

        /// <summary>
        /// 获取所有存活的怪物实体
        /// </summary>
        /// <param name="context"></param>
        /// <param name="battleEntitys"></param>
        public static void GetAliveMonsterEntitys(this IBattleContext context, List<IBattleEntityObject> battleEntitys)
        {
            foreach (var battleEntityObject in context.GetAliveMonsterEntitys())
            {
                battleEntitys.Add(battleEntityObject);
            }
        }

        /// <summary>
        /// 获取所有存活的玩家实体
        /// </summary>
        /// <param name="context"></param>
        /// <param name="battleEntitys"></param>
        public static void GetAlivePlayerEntitys(this IBattleContext context, List<IBattleEntityObject> battleEntitys)
        {
            foreach (var battleEntityObject in context.GetAlivePlayerEntitys())
            {
                battleEntitys.Add(battleEntityObject);
            }
        }

        /// <summary>
        /// 获取存活怪物数量
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int GetAliveMonsterEntityCount(this IBattleContext context)
        {
            var count = 0;
            foreach (var aliveMonsterEntity in context.GetAliveMonsterEntitys())
            {
                count++;
            }

            return count;
        }
        
        /// <summary>
        /// 获取存活玩家数量
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int GetAlivePlayerEntityCount(this IBattleContext context)
        {
            var count = 0;
            foreach (var aliveMonsterEntity in context.GetAlivePlayerEntitys())
            {
                count++;
            }

            return count;
        }
    }
}
