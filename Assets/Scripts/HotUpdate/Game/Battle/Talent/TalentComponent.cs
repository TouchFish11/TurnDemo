using System.Collections.Generic;
using Core.Log;
using HotUpdate.Base.Battle.Event;
using HotUpdate.Base.Battle.Object;
using HotUpdate.Game.Battle.Core;

namespace HotUpdate.Game.Battle.Talent
{
    /// <summary>
    /// �츳�����������ɫ���츳�������Զ������¼���
    /// </summary>
    public class TalentComponent : BattleComponent, ITalentComponent
    {
        private readonly List<ITalent> _talents = new List<ITalent>();

        public override void BattleInit(IBattleEntityObject battleEntity)
        {
            base.BattleInit(battleEntity);

            // �������п��ܴ����츳���¼��������ã��������ඩ�ģ�
            //BattleEntity.Context.GetEventBus().AddListener<TurnStartEvent>(OnBattleEventHandler);
            //BattleEntity.Context.GetEventBus().AddListener<TurnEndEvent>(OnBattleEventHandler);
        }

        /// <summary>
        /// ͳһ�¼��������ַ��¼��������츳�ж��Ƿ񴥷�
        /// </summary>
        /// <param name="battleEvent"></param>
        private void OnBattleEventHandler(BattleEvent battleEvent)
        {
            // if (battleEvent is TurnStartEvent turnStartEvent)
            // {
            //     foreach (var talent in _talents)
            //     {
            //         //talent.OnTurnStartHandler(turnStartEvent);
            //     }
            // }

            // if (battleEvent is TurnEndEvent turnEndEvent)
            // {
            //     foreach (var talent in _talents)
            //     {
            //         talent.OnTurnEndHandler(turnEndEvent);
            //     }
            // }

            foreach (var talent in _talents)
            {
                if (talent.CanTrigger(battleEvent, BattleEntity))
                {
                    talent.Execute(battleEvent, BattleEntity);
                }
            }
        }

        /// <summary>
        /// �����츳�����ñ��󶨣������츳������ô˷�����
        /// </summary>
        /// <param name="talent"></param>
        public void AddTalent(ITalent talent)
        {
            _talents.Add(talent);
            Logger.Log($"{BattleEntity.GameObject.name}�����츳��{talent.Name}");
        }
    }
}
