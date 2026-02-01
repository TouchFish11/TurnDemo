using Core.EventCenter;

namespace Game.Tasks
{
    public class DialogueEvent : Event
    {
        public int NpcId { get; set; }
    }
}
