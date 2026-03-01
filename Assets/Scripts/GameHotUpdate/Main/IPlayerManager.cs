using Core.Components;

namespace GameHotUpdate.Main
{
    /// <summary>
    /// ��ҹ������ӿ�
    /// </summary>
    public interface IPlayerManager
    {
        IEntityObject MainPlayer { get; }

        /// <summary>
        /// ��������û�
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task CreatePlayer(uint uid);

        /// <summary>
        /// �������
        /// </summary>
        void Clear();
    }
}
