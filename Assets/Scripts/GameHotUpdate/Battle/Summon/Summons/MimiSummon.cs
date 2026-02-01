using System.Collections;
using System.ComponentModel;
using Game.Battle.Damage;
using Game.Battle.Objects;
using GameHotUpdate.Battle.Event;
using GameHotUpdate.Objects;

namespace GameHotUpdate.Battle.Summon.Summons
{
    /// <summary>
    /// �����ٻ���
    /// </summary>
    public class MimiSummon : BattleObject, ISummon
    {
        public IBattleEntityObject Owner { get; private set; }

        public void Init(IBattleEntityObject owner)
        {
            Owner = owner;
            // ���ġ����˼����ͷ��¼��������˷ż���ʱ���ٻ���Эͬ������(��ѡ)
            //BattleEventBus.AddListener<SkillCastEvent>(OnOwnerSkillCastHandler);
        }

        /// <summary>
        /// �¼��ص��������ͷż��ܺ��ٻ���Эͬ����
        /// </summary>
        /// <param name="evt"></param>
        private void OnOwnerSkillCastHandler(SkillCastEvent evt)
        {
            //// ����Ӧ�ٻ��ߵļ����ͷţ�����ʣ���ж�����
            //if (evt.Caster != Owner || RemainingActionTimes <= 0) return;

            //Console.WriteLine($"\n{Name}��Ӧ{Owner.Name}�ļ��ܣ�����Эͬ������");
            //// Эͬ���������ý�ɫ���˺�API��
            //var summonDamage = Owner.GetAttribute(AttributeType.BaseAtk) * _ЭͬAttackRatio;
            //evt.Target.TakeDamage(summonDamage);
            //Console.WriteLine($"{evt.Target.Name}�ܵ�{Name}��Эͬ�˺���{summonDamage}��");

            //// �����ж�����
            //ConsumeActionTime();
            //if (RemainingActionTimes <= 0)
            //    Console.WriteLine($"{Name}�ж������ľ�����ʧ��");
        }

        public bool GetBattleComponent<TComponent>(out TComponent component) where TComponent : IComponent
        {
            bool isTrue = TryGetComponent<TComponent>(out TComponent c);
            component = c;
            return isTrue;
        }

        public override void Heal(int value)
        {
            // �ٻ��ﲻ�ɻظ�������չΪ�ɻظ���
        }

        //public override void TakeDamage(int damage, E_ElementType propertyType)
        //{
        //    // �ٻ��ﲻ�ɱ�����������չΪ�ɱ�������
        //}

        protected override IEnumerator OnExceuteAction()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnPreTakeDamage(DamageResult damageResult)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerator Die()
        {
            throw new System.NotImplementedException();
        }
    }
}
