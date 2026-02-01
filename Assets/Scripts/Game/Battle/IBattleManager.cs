using System.Threading.Tasks;
using Core.UI.MVC;
using Game.Battle.Context;

namespace Game.Battle
{
    /// <summary>
    /// ս���������ӿ�
    /// </summary>
    public interface IBattleManager
    {
        /// <summary>
        /// ��ȡ������
        /// </summary>
        /// <returns></returns>
        IBattleContext GetContext();

        /// <summary>
        /// ����ս��
        /// �ⲿ����
        /// </summary>
        /// <param name="controller">战斗控制器</param>
        Task StartBattle(IuiController controller);
    }
}
