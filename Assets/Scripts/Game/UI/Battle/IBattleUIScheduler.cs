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
        /// �սἼ����ʱUI�仯
        /// ��ʾ�սἼ��ɫ���桢�����ж���ʾ
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skillInfo"></param>
        void UltimateTriggerChangeUI(IBattleEntityObject caster, SkillInfo skillInfo);

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
