using Core.GlobalEvent;

namespace HotUpdate.Common.Events
{
    /// <summary>
    /// 对话事件
    /// </summary>
    public class DialogueEvent : Event
    {
        /// <summary>
        /// 对话实体ID
        /// </summary>
        public int EntityID { get; }
        
        public DialogueEvent(int entityID)
        {
            EntityID = entityID;
        }
    }
}
