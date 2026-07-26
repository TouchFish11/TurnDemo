using System;
using HotUpdate.Common.Config.Quest;

namespace HotUpdate.Game.Quests.Condition
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConditionTypeIdAttribute : Attribute
    {
        public EQuestConditionType ConditionType { get; private set; }
        
        public ConditionTypeIdAttribute(EQuestConditionType conditionType)
        {
            ConditionType = conditionType;
        }
    }
}
