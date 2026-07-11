using Core.Log;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Battle.Layer
{
    /// <summary>
    /// 层级获取器
    /// </summary>
    public static class LayerGeter
    {
        // 预先定义的层级数组
        private static readonly int[] preLayers =
        {
            LayerMask.NameToLayer("Environment"),
            LayerMask.NameToLayer("VFX"),
        };
        
        // 玩家层级数组
        private static readonly int[] roleLayers = {
            LayerMask.NameToLayer("PlayerObject1"),
            LayerMask.NameToLayer("PlayerObject2"),
            LayerMask.NameToLayer("PlayerObject3"),
            LayerMask.NameToLayer("PlayerObject4")
        };
        
        // 怪物层级数据
        private static readonly int[] monsterLayers = {
            LayerMask.NameToLayer("MonsterObject1"),
            LayerMask.NameToLayer("MonsterObject2"),
            LayerMask.NameToLayer("MonsterObject3"),
            LayerMask.NameToLayer("MonsterObject4"),
            LayerMask.NameToLayer("MonsterObject5"),
        };

        public static int[] GetRoleLayers()
        {
            return roleLayers;
        }

        /// <summary>
        /// 获取预定义的mask
        /// </summary>
        /// <returns></returns>
        public static int GetPreBitLayer()
        {
            var mask = 0;
            foreach (var layer in preLayers)
            {
                mask |= 1 << layer;
            }

            return mask;
        }
        
        /// <summary>
        /// 获取角色位运算后的层级
        /// </summary>
        /// <returns></returns>
        public static int GetRoleBitLayer()
        {
            var mask = 0;
            foreach (var roleLayer in roleLayers)
            {
                mask |= 1 << roleLayer;
            }

            return mask;
        }

        /// <summary>
        /// 获取怪物位运算后的层级
        /// </summary>
        /// <returns></returns>
        public static int GetMonsterBitLayer()
        {
            var mask = 0;
            foreach (var monsterLayer in monsterLayers)
            {
                mask |= 1 << monsterLayer;
            }

            return mask;
        }
        
        /// <summary>
        /// 获取位运算后的层级
        /// </summary>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public static int GetBitLayer(string layerName)
        {
            return 1 << LayerMask.NameToLayer(layerName);
        }
        
        /// <summary>
        /// 获取角色层级
        /// </summary>
        /// <returns></returns>
        public static int GetRoleLayer()
        {
            var mask = 0;

            mask |= GetLayer("PlayerObject1");
            mask |= GetLayer("PlayerObject2");
            mask |= GetLayer("PlayerObject3");
            mask |= GetLayer("PlayerObject4");

            return mask;
        }
        
        /// <summary>
        /// 获取指定角色层级
        /// </summary>
        /// <returns></returns>
        public static int GetRoleLayerByIndex(int index)
        {
            if (index < roleLayers.Length)
            {
                return roleLayers[index];
            }
            
            Logger.LogError(ELogTags.Battle, $"{nameof(LayerGeter)}.{nameof(GetRoleLayerByIndex)}：索引越界，当前索引：{index}");
            return -1;
        }
        
        /// <summary>
        /// 获取指定怪物层级
        /// </summary>
        /// <returns></returns>
        public static int GetMonsterLayerByIndex(int index)
        {
            if (index < monsterLayers.Length)
            {
                return monsterLayers[index];
            }
            
            Logger.LogError(ELogTags.Battle, $"{nameof(LayerGeter)}.{nameof(GetMonsterLayerByIndex)}：索引越界，当前索引：{index}");
            return -1;
        }
        
        /// <summary>
        /// 获取层级
        /// </summary>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public static int GetLayer(string layerName)
        {
            return LayerMask.NameToLayer(layerName);
        }
    }
}
