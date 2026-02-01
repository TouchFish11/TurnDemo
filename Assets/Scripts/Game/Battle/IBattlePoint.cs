using System.Collections.Generic;
using Game.Battle.Context;
using Game.Battle.Objects;
using UnityEngine;

namespace Game.Battle
{
    public interface IBattlePoint
    {
        GameObject GameObject { get; }
        
        /// <summary>
        /// 当前激活相机
        /// </summary>
        UnityEngine.Camera CurrentActiveCamera { get; }

        Transform MonsterCenter { get; }

        /// <summary>
        ///  初始化战斗点对象
        /// </summary>
        /// <returns></returns>
        void InitBattlePoint(IBattleContext context, List<IBattleEntityObject> players);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Transform> GetPlayerTransforms();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Transform> GetMonsterTransforms();

        Transform GetPlayerTransByIndex(int index);
        Transform GetMonsterTransByIndex(int index);

        /// <summary>
        /// 激活指定相机
        /// 传入行动的玩家或被攻击的玩家
        /// </summary>
        /// <param name="battleEntity">当前操作的玩家对象</param>
        void ActiveCamera(IBattleEntityObject battleEntity);
    }
}
