using Core.Log;
using Core.Service;
using Core.Singleton;
using Core.UI;
using Core.UI.MVC;
using Game.UI.Battle;
using GameHotUpdate.Battle.UI.Base;
using UnityEngine;

namespace GameHotUpdate.Battle.UI
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
