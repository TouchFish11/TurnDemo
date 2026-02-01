namespace Core.ScriptableObject
{
    /// <summary>
    /// �ɽű����������ӿ�
    /// </summary>
    public interface IScriptableObjectManager
    {
        T LoadSO<T>(string path) where T : UnityEngine.ScriptableObject;
    }
}
