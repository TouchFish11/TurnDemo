using HotUpdate.Game.Battle.Object;
using UnityEngine;

namespace HotUpdate.Game.Point
{
    public readonly struct PointInfo
    {
        /// <summary>
        /// 点变换
        /// </summary>
        public Transform Point { get; }

        /// <summary>
        /// 该位置对应的角色
        /// </summary>
        public IBattleEntityObject BattleEntity { get; }

        /// <summary>
        /// 怪物中心点x值
        /// </summary>
        public float MonsterCenterX { get; }

        public PointInfo(Transform point, IBattleEntityObject battleEntity, float monsterCenterX)
        {
            Point = point;
            BattleEntity = battleEntity;
            MonsterCenterX = monsterCenterX;
        }
    }
}
