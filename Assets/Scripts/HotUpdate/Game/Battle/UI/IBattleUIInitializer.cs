using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.UI
{
    public interface IBattleUIInitializer : IDisposable
    {
        /// <summary>
        /// 初始化玩家角色UI
        /// 为每个玩家实体创建并初始化角色状态UI，包括属性、图标、必杀技等信息
        /// </summary>
        /// <param name="battleEntities">玩家战斗实体集合</param>
        /// <returns>异步任务</returns>
        Task InitPlayerUIs(IEnumerable<IBattleEntityObject> battleEntities);

        /// <summary>
        /// 初始化怪物UI
        /// 为每个怪物实体创建并初始化普通怪物状态UI（如血条等），支持空参数传入
        /// </summary>
        /// <param name="battleEntities">怪物战斗实体集合</param>
        Task InitMonsterUIs(IEnumerable<IBattleEntityObject> battleEntities);
    }
}
