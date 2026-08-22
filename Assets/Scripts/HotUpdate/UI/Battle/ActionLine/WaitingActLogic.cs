using Core.DI;
using Core.Pool;
using Core.UI;
using UnityEngine;

namespace HotUpdate.UI.Battle.ActionLine
{
    /// <summary>
    /// 等待行动UI逻辑
    /// </summary>
    public class WaitingActLogic : IUILogic<WaitingActUI, WaitingActLogic>, IPoolData
    {
        [Inject] private IPoolManager _poolManager;
        
        // UI图标
        public Sprite Icon { get; private set; }
        // 实体对象ID
        private int _battleEntityId;
        
        public WaitingActUI View { get; private set; }
        
        public void Init(WaitingActUI view, Sprite icon, int battleEntityId)
        {
            View = view;
            Icon = icon;
            _battleEntityId = battleEntityId;
        }
        
        public void OnEnable()
        {
            
        }

        public void OnDisable()
        {

        }


        void IPoolData.ResetData()
        {
            
        }
        
        public void Dispose()
        {
            _poolManager.PushData(this);
        }
    }
}
