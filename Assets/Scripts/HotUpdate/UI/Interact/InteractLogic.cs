using Core.DI;
using Core.GlobalEvent;
using Core.GlobalEvent.Events;
using Core.Log;
using Core.Pool;
using Core.UI;
using HotUpdate.Base.Manager;
using HotUpdate.Game.Interact;

namespace HotUpdate.UI.Interact
{
    public class InteractLogic : IUILogic<InteractUI, InteractLogic>, IPoolData
    {
        [Inject] private IEventCenter _eventCenter;
        [Inject] private IPlayerManager _playerManager;
        [Inject] private IPoolManager _poolManager;
        
        // 交互的对象
        private IInteractable _currentInteractable;

        /// <summary>
        /// 交互UI的提示文本
        /// </summary>
        public string InteractTipText => _currentInteractable == null ? string.Empty : _currentInteractable.InteractTip;
        
        public InteractUI View { get; private set; }

        public void Init(InteractUI view, IInteractable interactable)
        {
            View = view;
            _currentInteractable = interactable;
        }

        public void TriggerInteract()
        {
            if (_currentInteractable == null)
            {
                var messageEvent = EventSource.Get<GlobalMessageEvent>();
                messageEvent.Message = "当前交互的对象不存在";
                _eventCenter.TriggerEvent(messageEvent);
                
                Logger.LogError(ELogTags.Interact, "The current interactive object does not exist");
                return;
            }
            
            _currentInteractable.Interact(_playerManager.MainPlayer);
        }
        
        public void Dispose()
        {
            _poolManager.PushData(this);
        }

        void IPoolData.ResetData()
        {
            _currentInteractable = null;
        }
    }
}
