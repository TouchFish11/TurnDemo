using Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 天赋管理组件（角色的天赋容器，自动订阅事件）
    /// </summary>
    public class TalentComponent : BattleComponent, ITalentComponent
    {
        private readonly List<ITalent> _talents = new List<ITalent>();

        public override void Init(IEntityObject entityObject)
        {
            // 订阅所有可能触发天赋的事件（可配置，避免冗余订阅）
            BattleEventCenter.AddListener<TurnStartEvent>(OnBattleEventHandler);
            BattleEventCenter.AddListener<TurnEndEvent>(OnBattleEventHandler);
            BattleEventCenter.AddListener<SkillCastEvent>(OnBattleEventHandler);
        }

        /// <summary>
        /// 统一事件处理：分发事件给所有天赋判断是否触发
        /// </summary>
        /// <param name="battleEvent"></param>
        private void OnBattleEventHandler(BattleEvent battleEvent)
        {
            if (battleEvent is TurnStartEvent turnStartEvent)
            {
                foreach (var talent in _talents)
                {
                    talent.OnTurnStartHandler(turnStartEvent);
                }
            }

            if (battleEvent is TurnEndEvent turnEndEvent)
            {
                foreach (var talent in _talents)
                {
                    talent.OnTurnEndHandler(turnEndEvent);
                }
            }

            foreach (var talent in _talents)
            {
                if (talent.CanTrigger(battleEvent, EntityObject as IBattleEntityObject))
                {
                    talent.Execute(battleEvent, EntityObject as IBattleEntityObject);
                }
            }
        }

        /// <summary>
        /// 添加天赋（配置表绑定，新增天赋仅需调用此方法）
        /// </summary>
        /// <param name="talent"></param>
        public void AddTalent(ITalent talent)
        {
            _talents.Add(talent);
            LogManager.Log($"{(EntityObject as IBattleEntityObject).Name}激活天赋：{talent.Name}");
        }
    }
}
