using Core.DI;
using Core.GlobalEvent;
using HotUpdate.Common.Config.Quest.Config;
using HotUpdate.Common.Events;

namespace HotUpdate.Game.Quests.Condition
{
    /// <summary>
    /// 对话条件
    /// </summary>
    public sealed class TalkCondition : QuestCondition<DialogueConditionConfig>
    {
        public TalkCondition(DialogueConditionConfig questConditionConfig) : base(questConditionConfig)
        {
            
        }

        public override void Enable()
        {
            DIContainer.GetInstance<IEventCenter>().Subscribe<DialogueEvent>(OnDialogueEvent);
        }

        private void OnDialogueEvent(DialogueEvent dialogueEvent)
        {
            if (dialogueEvent.EntityID != conditionConfig.targetNpcId) return;
            UpdateProgress(1);
        }

        public override void Disable()
        {
            DIContainer.GetInstance<IEventCenter>().Unsubscribe<DialogueEvent>(OnDialogueEvent);
        }
    }
}
