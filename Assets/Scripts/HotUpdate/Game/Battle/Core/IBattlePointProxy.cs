using System.Collections.Generic;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Point;
using UnityEngine;

namespace HotUpdate.Game.Battle.Core
{
    public interface IBattlePointProxy
    {
        BattlePoint BattlePoint { get; }
        
        void Reset();

        /// <summary>
        /// 初始化战斗点对象
        /// </summary>
        /// <param name="context"></param>
        void Init(IBattleContext context);

        /// <summary>
        /// 初始化角色战斗点，依赖玩家战斗实体对象创建完成
        /// </summary>
        /// <param name="roles"></param>
        void SetPointInfos(List<IBattleEntityObject> roles);

        /// <summary>
        /// 更新怪物在场景上的位置和之间的相对位置
        /// </summary>
        /// <param name="playerRole">释放技能的玩家角色对象</param>
        void UpdateMonsterPos(IBattleEntityObject playerRole);

        Transform GetRoleCameraRoot(PlayerObject playerObject);
        Transform GetRoleTransByIndex(int index);
    }
}
