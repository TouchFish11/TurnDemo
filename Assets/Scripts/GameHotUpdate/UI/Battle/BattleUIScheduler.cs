using Core.Log;
using Core.Reflection;
using Core.Service;
using Core.Singleton;
using Core.UI;
using Core.UI.MVC;
using Game.Battle;
using Game.Battle.Context;
using Game.Battle.Objects;
using Game.Battle.TargetSelect;
using Game.UI.Battle;
using Game.UI.Battle.SkillKey.Provider;
using GameHotUpdate.Objects;
using GameHotUpdate.UI.Battle.Base;
using GameHotUpdate.UI.Battle.SkillKey.Provider;
using UnityEngine;

namespace GameHotUpdate.UI.Battle
{
    /// <summary>
    /// ս��UI������
    /// �Ƕ����ϵ��õĿ���ʹ���¼�ͨ�ţ���ϵ��õ�ʹ�õ�����ͨ��
    /// </summary>
    public class BattleUIScheduler : SingletonAutoMono<BattleUIScheduler>, IBattleUIScheduler
    {
        public IuiController BattleController => _battleController;
        
        private BattleController _battleController;
        
        public GameObject GameObject { get; private set; }

        private void Awake()
        {
            GameObject = gameObject;
            _battleController = ServiceLocator.Get<IUIManager>().GetController<BattleController>();
            LogManager.Log($"_battleController:{_battleController}");
        }
        
        /// <summary>
        /// �սἼ����ʱUI�仯
        /// ��ʾ�սἼ��ɫ���桢�����ж���ʾ
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skillInfo"></param>
        public void UltimateTriggerChangeUI(IBattleEntityObject caster, SkillInfo skillInfo)
        {
            // ��ʾ��ɫ����
            _battleController.BattleUiManager.ShowPaiting((caster as PlayerObject)?.RoleInfo, skillInfo);
            // �����ж���ʾ
            _battleController.BattleUiManager.SetActTipActive(E_ActTipType.Hide);
            // �����սἼUI��ʾ
            var provider = ServiceLocator.Get<IFactoryManager>()
                .GetFactory<ISkillKeyUIDataProviderFactory, SkillKeyUIDataProviderFactory>()
                .GetCastSkillCondition<UltimateSkillKeyUIDataProvider>();
            _battleController.BattleUiManager.UpdateOperator(caster, provider);
            // ���¹���Ѫ��λ��
            _battleController.UiInitializer.InitMonsterUI(caster.Context.GetAliveMonsterEntitys());
        }

        /// <summary>
        /// ������������ر�ǡ�����UI
        /// �����ж�ǰ����
        /// </summary>
        /// <param name="context"></param>
        /// <param name="target"></param>
        public void UpdateCameraAndHideMarkerAndMonsterUI(IBattleContext context, IBattleEntityObject target)
        {
            // ���������������
            ServiceLocator.Get<IBattlePoint>().ActiveCamera(target);
            // �໥���򡢿��򹥻������
            context.GetTurnManager().UpdateEntityLookAt(target);
            // ���ع���UI
            ServiceLocator.Get<IUIManager>().GetController<BattleController>().UiInitializer.InitMonsterUI(null);
            // ����Ŀ��ѡ��
            ServiceLocator.Get<ITargetSelectManager>().InActiveSelectTarget();
            // ������UI
            _battleController.BattleUiManager.ClearSelectMarker();
            // �������UI
            _battleController.BattleUiManager.SetOperator(null);
            // ����Ϊ�����ж���ʾ
            _battleController.BattleUiManager.SetActTipActive(E_ActTipType.Monster);
        }

        /// <summary>
        /// �սἼ�ͷ�ʱ
        /// </summary>
        public void UltimateCasting()
        {
            // ������UI
            _battleController.BattleUiManager.ClearSelectMarker();
            _battleController.BattleUiManager.SetOperator(null);
            _battleController.BattleUiManager.SetActTipActive(E_ActTipType.Hide);
        }
    }
}
