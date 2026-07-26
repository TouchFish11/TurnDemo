using HotUpdate.Base.ECModule;

namespace HotUpdate.Game.Interact
{
    /// <summary>
    /// NPC对象
    /// </summary>
    public class NpcObject : EntityObject, IInteractable
    {
        // 对象交互策略
        private IInteractStrategy _interactStrategy;
        
        /// <summary>
        /// 是否显示对象头顶浮动文本
        /// </summary>
        public bool IsShowFloatingText { get; set; }

        /// <summary>
        /// NPC对象信息
        /// </summary>
        public NpcInfo NpcInfo { get; private set; }

        /// <summary>
        /// NPC专用初始化
        /// </summary>
        /// <param name="npcInfo"></param>
        public void InitNpc(NpcInfo npcInfo)
        {
            NpcInfo = npcInfo;
        }
        
        protected override void OnInit()
        {
            var interactTrigger = AddComponent<InteractTrigger>();
            interactTrigger.Init(this);
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
