using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DI;
using Core.Mono.MonoFunction;
using HotUpdate.Base;
using HotUpdate.Base.Manager;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Layer;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Point;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Battle.Core
{
    /// <summary>
    /// 场景战斗点代理
    /// </summary>
    public class BattlePointProxy : IBattlePointProxy, IDisposable
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
        /// 更新怪物位置
        /// </summary>
        /// <param name="battleEntity"></param>
        public void UpdateMonsterPos(IBattleEntityObject battleEntity)
        {
            // 更新怪物中心位置
            UpdateMonsterCenter(battleEntity);
            // 更新怪物之间的相对位置
            SortMonsterTrans();
        }

        /// <summary>
        /// 更新相机
        /// 传入行动的玩家或被攻击的玩家
        /// </summary>
        /// <param name="battleEntity">当前操作的玩家对象</param>
        public async Task UpdateCamera(IBattleEntityObject battleEntity)
        {
            try
            {
                if (battleEntity is PlayerObject)
                {
                    // 更新怪物位置
                    UpdateMonsterPos(battleEntity);
                    // 创建相机到指定位置点
                    var camera = await CreateCameraAtPos(battleEntity.EntityPosIndex);
                    // 更新相机Mask
                    UpdateCameraMask(camera, battleEntity.EntityPosIndex);
                }
                else
                {
                    Logger.LogError($"相机更新失败，当前实体为：{battleEntity}");
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(BattlePointProxy)}.{nameof(UpdateCamera)}：{e.Message}");
            }
        }
        
        /// <summary>
        /// 更新怪物中心位置
        /// </summary>
        /// <param name="battleEntity"></param>
        private void UpdateMonsterCenter(IBattleEntityObject battleEntity)
        {
            var pointInfo = pointInfos.Find(info => info.BattleEntity == battleEntity);
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

        /// <summary>
        /// 创建相机到指定位置
        /// </summary>
        /// <param name="entityPosIndex"></param>
        private Task<Camera> CreateCameraAtPos(int entityPosIndex)
        {
            // 创建相机到指定位置点
            var cameraTrans = BattlePoint.GetRoleCameraTransByIndex(entityPosIndex);
            return DIContainer.GetInstance<IBattleCameraManager>().CreateCamera(cameraTrans, Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// 更新相机Mask
        /// </summary>
        /// <param name="CurrentActiveCamera"></param>
        /// <param name="currentPosIndex"></param>
        private static void UpdateCameraMask(Camera CurrentActiveCamera, int currentPosIndex)
        {
            var mask = ResetCameraMask();
            // 根据当前玩家位置索引，只渲染符合的角色
            var roleLayers = LayerGeter.GetRoleLayers();
            for (var i = currentPosIndex; i < roleLayers.Length; i++)
            {
                mask |= 1 << roleLayers[i];
            }
            CurrentActiveCamera.cullingMask = mask;
        }

        /// <summary>
        /// 重置相机Mask层级
        /// </summary>
        private static int ResetCameraMask()
        {
            var mask= LayerGeter.GetPreBitLayer();
            // TODO：暂时写所有怪物，后续优化
            mask |= LayerGeter.GetMonsterBitLayer();
            
            return mask;
        }

        public void Dispose()
        {
            pointInfos.Clear();
            pointInfos = null;
        }
    }
}
