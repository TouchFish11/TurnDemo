using UnityEngine;

namespace Core.EditorRes
{
    /// <summary>
    /// 编辑器资源管理器接口
    /// </summary>
    public interface IEditorResManager
    {
        /// <summary>
        /// 加载编辑器资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetName">资源名</param>
        /// <param name="suffixName">后缀，None为无后缀</param>
        /// <returns></returns>
        T LoadEditorAsset<T>(string assetName, string suffixName = "") where T : Object;
    }
}
