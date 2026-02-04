using UnityEngine;

namespace GameHotUpdate.Layer
{
    /// <summary>
    /// 层级获取器
    /// </summary>
    public static class LayerGeter
    {
        /// <summary>
        /// 获取角色位运算后的层级
        /// </summary>
        /// <returns></returns>
        public static int GetRoleBitLayer()
        {
            var mask = 0;

            mask |= GetBitLayer("PlayerObject1");
            mask |= GetBitLayer("PlayerObject2");
            mask |= GetBitLayer("PlayerObject3");
            mask |= GetBitLayer("PlayerObject4");

            return mask;
        }

        /// <summary>
        /// 获取怪物位运算后的层级
        /// </summary>
        /// <returns></returns>
        public static int GetMonsterBitLayer()
        {
            return GetBitLayer("MonsterObject");
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
        /// 获取怪物层级
        /// </summary>
        /// <returns></returns>
        public static int GetMonsterLayer()
        {
            return GetBitLayer("MonsterObject");
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
