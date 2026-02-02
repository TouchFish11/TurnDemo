using Core.Res;
using Core.Service;
using Core.Singleton;

namespace Core.ScriptableObject
{
    /// <summary>
    /// SO������
    /// </summary>
    public class ScriptableObjectManager : SingletonBase<ScriptableObjectManager>, IScriptableObjectManager
    {
        private ScriptableObjectManager()
        {

        }

        /// <summary>
        /// ����ScriptableObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T LoadSO<T>(string path) where T : UnityEngine.ScriptableObject
        {
            return ServiceLocator.Get<IResourcesManager>().Load<T>(path);
        }
    }
}
