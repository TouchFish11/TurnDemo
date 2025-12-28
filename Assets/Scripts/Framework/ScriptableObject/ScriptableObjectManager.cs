using UnityEngine;

namespace Framework
{
    /// <summary>
    /// SOπ‹¿Ì∆˜
    /// </summary>
    public class ScriptableObjectManager : SingletonBase<ScriptableObjectManager>, IScriptableObjectManager
    {
        private ScriptableObjectManager()
        {

        }

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
