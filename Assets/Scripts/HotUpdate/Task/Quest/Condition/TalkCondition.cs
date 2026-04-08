using Core.GlobalEvent;
using Core.Service;
using HotUpdate.Common.Events;
using HotUpdate.Config.Quest;
using HotUpdate.Config.Quest.Config;

namespace HotUpdate.Task.Quest.Condition
{
    /// <summary>
    /// 对话条件
    /// </summary>
    public sealed class TalkCondition : QuestCondition<DialogueConditionConfig>
    {
        public TalkCondition(DialogueConditionConfig questConditionConfig) : base(questConditionConfig)
        {
            
        }

        public override void Enable(QuestNodeData questNodeData)
        {
            this.questNodeData = questNodeData;
            ServiceLocator.Get<IEventCenter>().Subscribe<DialogueEvent>(OnDialogueEvent);
        }

        private void OnDialogueEvent(DialogueEvent dialogueEvent)
        {
            if (dialogueEvent.EntityID != conditionConfig.targetNpcId) return;
            
            questNodeData.Progress += 1;
            InvokeOnComplete();
        }

        public override void Disable()
        {
            ServiceLocator.Get<IEventCenter>().Unsubscribe<DialogueEvent>(OnDialogueEvent);
        }
    }
}
