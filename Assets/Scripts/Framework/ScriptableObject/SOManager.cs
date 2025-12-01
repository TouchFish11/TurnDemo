using UnityEngine;

namespace Framework
{
    /// <summary>
    /// SOπ‹¿Ì∆˜
    /// </summary>
    public class SOManager : SingletonBase<SOManager>
    {
        private SOManager() { }

        /// <summary>
        /// º”‘ÿScriptableObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T LoadSO<T>(string path) where T : ScriptableObject
        {
            return ResourcesManager.Instance.Load<T>(path);
        }
    }
}
