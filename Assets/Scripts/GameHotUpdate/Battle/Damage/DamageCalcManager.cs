using System.Collections.Generic;
using Core.Log;
using Core.Singleton;
using Game.Battle.Context;
using Game.Battle.Damage;
using Game.Battle.Objects;
using Game.Battle.Skill.Enum;
using Game.Tasks;
using GameHotUpdate.Battle.Damage.Strategys;
using GameHotUpdate.Battle.Event.General;

namespace GameHotUpdate.Battle.Damage
{
    /// <summary>
    /// �˺����������
    /// </summary>
    public class DamageCalcManager : SingletonBase<DamageCalcManager>, IDamageCalcManager
    {
        // �����ֵ�
        private readonly Dictionary<E_DamageType, IDamageStrategy> _strategys = new();

        private DamageCalcManager()
        {

        }

        public void Init(IBattleContext context)
        {
            _strategys.Clear();
            // ��ʼ���������
            _strategys.Add(E_DamageType.Direct, new DirectDamageStrategy());
            _strategys.Add(E_DamageType.Dot, new DotDamageStrategy());
            _strategys.Add(E_DamageType.Break, new BreakDamageStrategy());
            _strategys.Add(E_DamageType.True, new TrueDamageStrategy());

            // ������Ҫ�����˺����¼�
            context.GetEventBus().AddListener<ToughnessBrokenEvent>(OnToughnessBrokenEvent);
        }

        /// <summary>
        /// ���㼼���˺�
        /// </summary>
        /// <param name="source">������</param>
        /// <param name="target">Ŀ��</param>
        /// <param name="skillInfo"></param>
        /// <param name="damageResult"></param>
        /// <returns>�����˺�</returns>
        public void CalcSkillDamage(IBattleEntityObject source, IBattleEntityObject target,SkillInfo skillInfo, out DamageResult damageResult)
        {
            E_DamageType damageType = skillInfo.f_damageType.ToDamageType();
            if (_strategys.TryGetValue(damageType, out IDamageStrategy strategy))
            {
                //����ÿ�������˺�
                strategy.CalcDamage(source, target, skillInfo, out damageResult);
                // �ַ�Ӧ���˺��¼���ս�����������ʾ�˺��ı���
                source.Context.GetEventBus().TriggerEvent(new ApplyDamageEvent(source.Context, damageResult));
            }
            else
            {
                damageResult = default;
                LogManager.LogError("δʵ�ֶ�Ӧ���˺�����");
            }
        }

        /// <summary>
        /// �����¼��ص�
        /// ��������˺�
        /// </summary>
        /// <param name="toughnessBrokenEvent"></param>
        private void OnToughnessBrokenEvent(ToughnessBrokenEvent toughnessBrokenEvent)
        {
            CalcBrokenDamage(toughnessBrokenEvent.Breaker, toughnessBrokenEvent.Target, toughnessBrokenEvent.SkillInfo, out DamageResult result);
            toughnessBrokenEvent.Target.TryTakeDamage(result);
        }

        private void CalcBrokenDamage(IBattleEntityObject source, IBattleEntityObject target, SkillInfo skillInfo, out DamageResult damageResult)
        {
            if (_strategys.TryGetValue(E_DamageType.Break, out IDamageStrategy strategy))
            {
                //����ÿ�������˺�
                strategy.CalcDamage(source, target, skillInfo, out damageResult);
                // �ַ�Ӧ���˺��¼���ս�����������ʾ�˺��ı���
                source.Context.GetEventBus().TriggerEvent(new ApplyDamageEvent(source.Context, damageResult));
            }
            else
            {
                damageResult = default;
                LogManager.LogError("δʵ�ֶ�Ӧ���˺�����");
            }
        }

        ///// <summary>
        ///// ����Dot�˺�
        ///// </summary>
        ///// <param name="attacker">������</param>
        ///// <param name="target">Ŀ��</param>
        ///// <param name="damageType">�˺�����</param>
        ///// <param name="extraData"></param>
        ///// <returns>�����˺�</returns>
        //public void CalcDotDamage(IBattleEntityObject source, IBattleEntityObject target, IDotBuff dot)
        //{
        //    //if (_strategyDic.TryGetValue(E_DamageType.Dot, out IDamageStrategy strategy))
        //    //{
        //    //    UIMgr.Instance.GetPanel<BattlePanel>((panel) =>
        //    //    {
        //    //        //���������˺�
        //    //        int tempDmg = dot.CalcSkillDamage();
        //    //        target.ProcessDamage(new DamageResult());
        //    //        //�ַ��¼�
        //    //        //EventCenter.Instance.EventTrigger(E_EventType.OnApplyDamage, new ApplyDamageEvent(attacker, target, tempDmg));
        //    //        //��ʾ�˺�
        //    //        CreateDamageText(tempDmg, target);
        //    //        //��ʾ�ۼ��˺�
        //    //        panel.UpdateCumulativeDamageText(dmg: _currentTotalDamage += tempDmg);
        //    //    });
        //    //}
        //    //else
        //    //{
        //    //    DebugMgr.LogError("δʵ�ֶ�Ӧ�Ĳ���");
        //    //}
        //}
    }
}
