using System.Threading.Tasks;
using Core.Components;

namespace Game.Main
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
        Task CreatePlayer(uint uid);

        /// <summary>
        /// �������
        /// </summary>
        void Clear();
    }
}
