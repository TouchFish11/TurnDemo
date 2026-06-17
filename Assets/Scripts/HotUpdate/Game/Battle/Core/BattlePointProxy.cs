using System;
using System.Collections.Generic;
using System.Linq;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Point;
using UnityEngine;

namespace HotUpdate.Game.Battle.Core
{
    /// <summary>
    /// 场景战斗点代理
    /// </summary>
    public class BattlePointProxy : IDisposable
    {
        // 怪物中心点x值
        private readonly float[] monstetCenterXs = { 6f, 4f, 2f, 0f };
        // 点信息列表
        private List<PointInfo> pointInfos = new();
        // 战斗上下文
        private IBattleContext context;
        // 当前怪物数量
        private int currentMonsterCount;
        
        /// <summary>
        /// 场景上的战斗点
        /// </summary>
        public BattlePoint BattlePoint { get; } = UnityEngine.Object.FindFirstObjectByType<BattlePoint>();

        /// <summary>
        /// 初始化战斗点对象
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="players"></param>
        public void InitProxy(IBattleContext ctx, List<IBattleEntityObject> players)
        {
            context = ctx;
            var index = 0;
            foreach (var roleTrans in BattlePoint.GetRoleTransforms())
            {
                if (index >= players.Count)
                {
                    break;
                }
                
                var pointInfo = new PointInfo(roleTrans, players[index], monstetCenterXs[index]);
                pointInfos.Add(pointInfo);
                index++;
            }
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
            return BattlePoint.GetRoleCameraTransByIndex(playerObject.EntityPosIndex);
        }

        public Transform GetRoleTransByIndex(int index)
        {
            return BattlePoint.GetRoleTransByIndex(index);
        }

        /// <summary>
        /// 更新怪物中心位置
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
        /// 排序怪物相对位置
        /// </summary>
        private void SortMonsterTrans()
        {
            // 更新怪物之间的相对位置，居中显示
            var newLiveCount = context.GetAliveMonsterEntitys().Count();
            if (currentMonsterCount == newLiveCount)
            {
                return;
            }

            var monsters = context.GetSceneMonsters();
            switch (newLiveCount)
            {
                // 居中显示，放在索引2的位置
                case 1:
                {
                    // 更新位置索引
                    var monster = monsters[0];
                    monster.EntityPosIndex = 2;
                    // 设置父对象
                    monster.GameObject.transform.SetParent(BattlePoint.GetMonsterTransByIndex(2), false);
                    break;
                }
                case 2:
                    // 从中间往右放
                    for (var i = 0; i < monsters.Count; i++)
                    {
                        var index = i + 2;
                        var monster = monsters[i];
                        monster.EntityPosIndex = index;
                        // 设置父对象
                        monster.GameObject.transform.SetParent(BattlePoint.GetMonsterTransByIndex(index), false);
                    } 
                    break;
                case 3:
                    // 往中间放
                    for (var i = 0; i < monsters.Count; i++)
                    {
                        var index = i + 1;
                        var monster = monsters[i];
                        monster.EntityPosIndex = index;
                        // 设置父对象
                        monster.GameObject.transform.SetParent(BattlePoint.GetMonsterTransByIndex(index), false);
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
                        monster.GameObject.transform.SetParent(BattlePoint.GetMonsterTransByIndex(index), false);
                    }

                    break;
                }
            }
            currentMonsterCount = newLiveCount;
        }
        
        public void Dispose()
        {
            pointInfos.Clear();
            pointInfos = null;
        }
    }
}
