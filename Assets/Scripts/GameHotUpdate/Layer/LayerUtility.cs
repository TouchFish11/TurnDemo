using UnityEngine;

namespace GameHotUpdate.Layer
{
    /// <summary>
    /// 层级工具类
    /// </summary>
    public static class LayerUtility
    {
        /// <summary>
        /// 递归设置物体及其所有子物体的 Layer
        /// </summary>
        public static void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }
    }
}
