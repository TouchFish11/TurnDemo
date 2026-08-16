using HotUpdate.Base.ECModule;

namespace HotUpdate.Game.Interact
{
    /// <summary>
    /// 交互对象
    /// </summary>
    public abstract class InteractObject : EntityObject, IInteractable
    {
        // 对象交互策略
        private IInteractStrategy _interactStrategy;

        protected sealed override void OnInit()
        {
            var interactTrigger = AddComponent<InteractTrigger>();
            interactTrigger.Init(this);
            OnInteractInit();
        }

        protected virtual void OnInteractInit()
        {
            
        }

        public void SetInteractStrategy(IInteractStrategy strategy)
        {
            _interactStrategy = strategy;
        }

        public void Interact(IEntityObject entityObject)
        {
            _interactStrategy?.Interact(this);
        }
    }
}
