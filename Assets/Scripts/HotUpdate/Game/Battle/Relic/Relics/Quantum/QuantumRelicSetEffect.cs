using System;
using HotUpdate.Base;

namespace HotUpdate.Game.Battle.Relic.Relics.Quantum
{
    /// <summary>
    /// ������4����Ч����ʵ����װЧ���ӿڣ���Ϊ���������
    /// </summary>
    public class QuantumRelicSetEffect : IRelicSetEffect
    {
        public string SetName { get; } = "����֮Ӱ";
        public int RequiredCount { get; } = 4;

        public IBattleEntityObject Owner { get; private set; }

        //public IEntityObject EntityObject { get; private set; }

        public IBattleEntityObject BattleEntity { get; private set; }

        // private float _additionalDamageRatio = 0.5f; // ׷���˺����ʣ����ñ���ȡ��
        
        public virtual void BattleInit(IBattleEntityObject battleEntity)
        {
            BattleEntity = battleEntity;
        }

        public void SetOwner(IBattleEntityObject owner)
        {
            Owner = owner;
        }

        public void Activate(IBattleEntityObject owner)
        {
            Console.WriteLine($"{owner.GameObject.name}����{SetName}4����Ч����������׷�������˺�");

            // ���ġ������ͷ��¼������ж��Ƿ񱩻�������׷���˺���
            //BattleEntity.Context.GetEventBus().AddListener<SkillCastEvent>(OnSkillCastHandler);

            // �����߼���2����/4���׻������Լӳɣ�ֱ�ӵ��ý�ɫ����API��
            var attributeBonus = RequiredCount switch
            {
                2 => new RelicEffect(E_RelicBoun.CriticalRate, 12),
                4 => new RelicEffect(E_RelicBoun.CriticalRate, 12), // ʾ����4���׼̳�2����Ч��
                _ => throw new NotImplementedException()
            };

            // owner.GetComponent<PropertyComponent>().AddRelicBonus(attributeBonus.RelicBoun, attributeBonus.BounValue);
        }

        // private void OnSkillCastHandler(SkillCastEvent skillCastEvent)
        // {
        //     // ����������1. ����װ�������ͷż��� 2. ���ܴ����������򻯣������˺�>150�ж�Ϊ������
        //     //if (skillCastEvent.Caster != Owner || skillCastEvent.Damage <= 150)
        //     //{
        //     //    return;
        //     //}
        //
        //     Console.WriteLine($"\n������Ч����{SetName}4���״�����");
        //
        //     //int additionalDamage = (int)(skillCastEvent.Caster.GetComponent<PropertyComponent>().GetProperty<BattleProperty>().TotalAtk * _additionalDamageRatio);
        //     //skillCastEvent.Targets[0].TakeDamage(additionalDamage, E_ElementType.Quantum, E_DamageType.Direct);
        //     //Console.WriteLine($"{skillCastEvent.Targets[0].GameObject.name}�ܵ�����׷���˺���{additionalDamage}��");
        // }

        public void Destroy()
        {
            throw new NotImplementedException();
        }
    }
}
