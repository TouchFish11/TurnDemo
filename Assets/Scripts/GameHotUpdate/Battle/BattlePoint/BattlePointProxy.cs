using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Config;
using Core.Log;
using Core.Pool;
using Core.Service;
using Game.Battle;
using Game.Battle.Context;
using Game.Battle.Input;
using Game.Battle.Objects;
using Game.Objects;
using GameHotUpdate.Objects;
using UnityEngine;

namespace GameHotUpdate.Battle.BattlePoint
{
    /// <summary>
    /// 场景战斗点代理
    /// </summary>
    public class BattlePointProxy : IBattlePointProxy
    {
        // 点信息列表
        private readonly List<PointInfo> pointInfos = new();
        // 战斗上下文
        private IBattleContext context;
        // 当前相机旋转角度
        private float currentXAngle;
        // 当前怪物数量
        private int currentMonsterCount;
        
        // X轴旋转角度限制
        private const float minXAngle = -3f;
        private const float maxXAngle = 3f;
        // 旋转叠加速度
        private const float rotateAddSpeed = 5f;
        // 旋转灵敏度
        private const float rotateSpeed = 1.5f;
        // 预先的层级数组
        private readonly int[] preLayers =
        {
            1 << LayerMask.NameToLayer("MonsterObject"),
            1 << LayerMask.NameToLayer("Environment"),
            1 << LayerMask.NameToLayer("VFX"),
        };
        // 玩家层级数组
        private readonly int[] roleLayers = {
            LayerMask.NameToLayer("PlayerObject1"),
            LayerMask.NameToLayer("PlayerObject2"),
            LayerMask.NameToLayer("PlayerObject3"),
            LayerMask.NameToLayer("PlayerObject4")
        };
        
        // 怪物中心点x值
        private readonly byte[] bytes = { 6, 4, 2, 0 };
        
        /// <summary>
        /// 场景战斗点
        /// </summary>
        public Game.Battle.BattlePoint BattlePoint { get; }

        /// <summary>
        /// 当前激活相机
        /// </summary>
        public Camera CurrentActiveCamera { get; private set; }

        public BattlePointProxy()
        {
            BattlePoint = UnityEngine.Object.FindFirstObjectByType<Game.Battle.BattlePoint>();
        }

        /// <summary>
        /// 获取角色层级
        /// </summary>
        /// <param name="index">创建时的索引</param>
        /// <returns></returns>
        public int GetRoleLayer(int index)
        {
            return roleLayers[index];
        }
        
        /// <summary>
        /// 初始化战斗点对象
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="players"></param>
        public void InitProxy(IBattleContext ctx, List<IBattleEntityObject> players)
        {
            this.context = ctx;
            var index = 0;
            foreach (var roleTrans in BattlePoint.GetRoleTransforms())
            {
                if (index >= players.Count)
                {
                    break;
                }
                
                var pointInfo = new PointInfo(roleTrans, players[index], bytes[index]);
                pointInfos.Add(pointInfo);
                index++;
            }
            
            ServiceLocator.Get<IBattleInputHandler>().OnDrag += OnDrag;
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
        public async void UpdateCamera(IBattleEntityObject battleEntity)
        {
            try
            {
                if (battleEntity is PlayerObject)
                {
                    // 更新怪物位置
                    UpdateMonsterPos(battleEntity);
                    // 创建相机到指定位置点
                    await CreateCameraAtPos(battleEntity.EntityPosIndex);
                    // 更新相机Mask
                    UpdateCameraMask(battleEntity.EntityPosIndex);
                    // 初始化当前旋转角度为相机初始角度
                    currentXAngle = 0;
                }
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(BattlePointProxy)}.{nameof(UpdateCamera)}：{e.Message}");
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
        private async Task CreateCameraAtPos(int entityPosIndex)
        {
            if(CurrentActiveCamera)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(CurrentActiveCamera.gameObject);
                CurrentActiveCamera = null;
            }
            
            // 创建相机到指定位置点
            var cameraTrans = BattlePoint.GetRoleCameraTransByIndex(entityPosIndex);
            var cameraObj = await ServiceLocator.Get<IObjectBuilder>()
                .GetGameobject(EAssetBundleType.Camera, ResKeyCollection.BattleCamera, cameraTrans);
            CurrentActiveCamera = cameraObj.GetComponent<Camera>();
            CurrentActiveCamera.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// 更新相机Mask
        /// </summary>
        /// <param name="currentPosIndex"></param>
        private void UpdateCameraMask(int currentPosIndex)
        {
            ResetCameraMask();
            var mask = 0;
            for (var i = currentPosIndex; i < roleLayers.Length; i++)
            {
                mask |= 1 << roleLayers[i];
            }
            
            CurrentActiveCamera.cullingMask |= mask;
        }

        /// <summary>
        /// 重置相机Mask层级
        /// </summary>
        private void ResetCameraMask()
        {
            // 重置层级
            CurrentActiveCamera.cullingMask = 0;
            var mask = 0;
            foreach (var preLayer in preLayers)
            {
                mask |= preLayer;
            }

            CurrentActiveCamera.cullingMask = mask;
        }
        
        /// <summary>
        /// 滑动事件回调
        /// </summary>
        /// <param name="deltaX"></param>
        private void OnDrag(float deltaX)
        {
            // 转换为旋转角度
            currentXAngle += deltaX * rotateAddSpeed * Time.deltaTime;
            currentXAngle = Mathf.Clamp(currentXAngle, minXAngle, maxXAngle);
            // 应用旋转（使用欧拉角，直观且易控制）
            var targetRot = Quaternion.Euler(0, currentXAngle, 0f);
            CurrentActiveCamera.transform.localRotation = Quaternion.Slerp(CurrentActiveCamera.transform.localRotation, targetRot, Time.deltaTime * rotateSpeed);
        }

        public void Dispose()
        {
            ServiceLocator.Get<IBattleInputHandler>().OnDrag -= OnDrag;
        }
    }
}
