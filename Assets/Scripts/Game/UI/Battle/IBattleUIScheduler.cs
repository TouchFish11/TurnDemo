using Core.UI.MVC;
using Game.Battle.Context;
using Game.Battle.Objects;
using UnityEngine;

namespace Game.UI.Battle
{
    public interface IBattleUIScheduler
    {
        GameObject GameObject { get; }    
        
        /// <summary>
        /// ������������ر�ǡ�����UI
        /// �����ж�ǰ����
        /// </summary>
        /// <param name="context"></param>
        /// <param name="target"></param>
        void UpdateCameraAndHideMarkerAndMonsterUI(IBattleContext context, IBattleEntityObject target);

        /// <summary>
        /// �սἼ�ͷ�ʱ
        /// </summary>
        void UltimateCasting();

        IuiController BattleController { get; }
    }
}
