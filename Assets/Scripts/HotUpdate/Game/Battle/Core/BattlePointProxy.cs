using System.Collections.Generic;
using System.Linq;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Point;
using UnityEngine;

namespace HotUpdate.Game.Battle.Core
{
    /// <summary>
    /// 场景战斗点代理
    /// </summary>
    public class BattlePointProxy : IBattlePointProxy
    {
        // 场景战斗点对象
        private BattlePoint _battlePoint;
        // 怪物中心点x值
        private readonly float[] monstetCenterXs = { 6f, 4f, 2f, 0f };
        // 点信息列表
        private readonly List<PointInfo> pointInfos = new();
        // 战斗上下文
        private IBattleContext context;
        // 上次场上存活怪物数量
        private int _lastLiveMonesterCount;

        /// <summary>
        /// 场景上的战斗点
        /// </summary>
        public BattlePoint BattlePoint
        {
            get
            {
                return _battlePoint ??= UnityEngine.Object.FindFirstObjectByType<BattlePoint>();
            }
        }
        
        /// <summary>
        /// 初始化战斗点对象
        /// </summary>
        /// <param name="context"></param>
        /// <param name="roles"></param>
        public void InitProxy(IBattleContext context, List<IBattleEntityObject> roles)
        {
            var index = 0;
            foreach (var roleTrans in BattlePoint.RoleTrans)
            {
                if (index >= roles.Count)
                {
                    break;
                }
                
                var pointInfo = new PointInfo(roleTrans, roles[index], monstetCenterXs[index]);
                pointInfos.Add(pointInfo);
                index++;
            }
            this.context = context;
        }

        /// <summary>
        /// 更新怪物在场景上的位置和之间的相对位置
        /// </summary>
        /// <param name="playerRole">释放技能的玩家角色对象</param>
        public void UpdateMonsterPos(IBattleEntityObject playerRole)
        {
            // 更新怪物中心位置
            UpdateMonsterCenter(playerRole);
            // 更新怪物之间的相对位置
            SortMonsterTrans();
        }

        public Transform GetRoleCameraRoot(PlayerObject playerObject)
        {
            return BattlePoint.RoleCamerasTrans[playerObject.EntityPosIndex];
        }

        public Transform GetRoleTransByIndex(int index)
        {
            return BattlePoint.RoleTrans[index];
        }

        /// <summary>
        /// 更新怪物中心位置，确保怪物整体能显示在角色的右方
        /// </summary>
        /// <param name="playerRole">释放技能的玩家角色对象</param>
        private void UpdateMonsterCenter(IBattleEntityObject playerRole)
        {
            var pointInfo = pointInfos.Find(info => info.BattleEntity == playerRole);
            var nowPos = BattlePoint.MonsterCenter.position;
            nowPos.x = pointInfo.MonsterCenterX;
            BattlePoint.MonsterCenter.transform.position = nowPos;
        }

        /// <summary>
        /// 排序怪物相对位置，整体居中显示
        /// </summary>
        private void SortMonsterTrans()
        {
            // 更新怪物之间的相对位置
            var newLiveCount = context.GetAliveMonsterEntitys().Count();
            if (_lastLiveMonesterCount == newLiveCount)
                return;

            var monsters = context.SceneMonsterObjects;
            switch (newLiveCount)
            {
                // 居中显示，放在索引2的位置
                case 1:
                {
                    // 更新位置索引
                    var monster = monsters[0];
                    monster.EntityPosIndex = 2;
                    monster.GameObject.transform.SetParent(BattlePoint.MonsterTrans[2], false);
                    break;
                }
                case 2:
                    // 从中间往右放
                    for (var i = 0; i < monsters.Count; i++)
                    {
                        var index = i + 2;
                        var monster = monsters[i];
                        monster.EntityPosIndex = index;
                        monster.GameObject.transform.SetParent(BattlePoint.MonsterTrans[index], false);
                    } 
                    break;
                case 3:
                    // 往中间放
                    for (var i = 0; i < monsters.Count; i++)
                    {
                        var index = i + 1;
                        var monster = monsters[i];
                        monster.EntityPosIndex = index;
                        monster.GameObject.transform.SetParent(BattlePoint.MonsterTrans[index], false);
                    } 
                    break;
                case 4:
                {
                    // 从中间往右放
                    for (var i = 0; i < monsters.Count; i++)
                    {
                        var index = i + 1;
                        var monster = monsters[i];
                        monster.EntityPosIndex = index;
                        // 设置父对象
                        monster.GameObject.transform.SetParent(BattlePoint.MonsterTrans[index], false);
                    }

                    break;
                }
            }
            
            _lastLiveMonesterCount = newLiveCount;
        }

        public void Reset()
        {
            pointInfos.Clear();
            context = null;
            _battlePoint = null;
            _lastLiveMonesterCount = -1;
        }
    }
}
