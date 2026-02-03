using Core.Service;
using Core.UI;
using Game.Battle.Context;
using Game.Battle.Objects;
using Game.Battle.Skill.Handler;
using Game.Battle.Status;
using Game.Battle.TargetSelect;
using GameHotUpdate.UI.Battle.Base;

namespace GameHotUpdate.Battle.Skill.Skills
{
    /// <summary>
    /// ���＼��
    /// �����ɫ���ܼ̳�
    /// </summary>
    public abstract class MonsterSkill : Skill
    {
        protected MonsterSkill(IBattleEntityObject caster, int skillId, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, statusAddStrategy)
        {

        }

        /// <summary>
        /// ���＼���ͷ�ǰִ��
        /// ����UI����߼�
        /// </summary>
        /// <param name="context"></param>
        protected override void OnPreCast(IBattleContext context)
        {
            base.OnPreCast(context);
            // ���������������
            context.GetProxy().UpdateCamera(MainTarget);
            // �໥���򡢿��򹥻������
            context.GetTurnManager().UpdateEntityLookAt(MainTarget);
            // ����Ŀ��ѡ��
            ServiceLocator.Get<ITargetSelectManager>().InActiveSelectTarget();
            // ���ع���UI
            BattleController controller = ServiceLocator.Get<IUIManager>().GetController<BattleController>();
            controller.UiInitializer.InitMonsterUI(null);
            // ������UI
            controller.BattleUiManager.ClearSelectMarker();
            // �������UI
            controller.BattleUiManager.SetOperator(null);
            // ����Ϊ�����ж���ʾ
            controller.BattleUiManager.SetActTipActive(E_ActTipType.Monster);
        }
    }
}
