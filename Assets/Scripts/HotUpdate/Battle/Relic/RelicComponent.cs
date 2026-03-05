using System;
using System.Collections.Generic;
using System.Linq;
using HotUpdate.Battle.Core;

namespace HotUpdate.Battle.Relic
{
    /// <summary>
    /// ���������������ɫ������������������ص���/��װЧ����
    /// ս����洢�������ݣ�ս��ʱͨ�����ݳ�ʼ�����������Ȼ���ڶ�̬��������Ч��
    /// </summary>
    public class RelicComponent : BattleComponent, IRelicComponent
    {
        // ��װ������
        private readonly List<IRelic> _equippedRelics = new List<IRelic>();
        // ������װӳ��
        private readonly Dictionary<int, IRelicSetEffect> _activeSetEffects = new Dictionary<int, IRelicSetEffect>();

        /// <summary>
        /// װ����������������������ô˷�����������������룩
        /// </summary>
        /// <param name="relic"></param>
        public void EquipRelic(IRelic relic)
        {
            _equippedRelics.Add(relic);
            Console.WriteLine($"{BattleEntity.GameObject.name}װ��������{relic.Name}");

            // �����������Լӳ�
            foreach (var effect in relic.SingleEffects)
            {
               // Caster.GetComponent<PropertyComponent>().AddRelicBonus(effect.RelicBoun, effect.BounValue);
            }

            // �����װЧ����ͳ��ͬ��װ���������������򼤻
            CheckAndActivateSetEffects();
        }

        /// <summary>
        /// ��װЧ�������߼���������װ���������ⲿ��
        /// </summary>
        private void CheckAndActivateSetEffects()
        {
            // ͳ�Ƹ���װ��װ������
            var setCount = _equippedRelics.GroupBy(r => r.RelicID)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var (setId, count) in setCount)
            {
                // �����ñ���ȡ��Ӧ��װЧ����ʾ���������׶�ӦQuantumRelicSetEffect��
                var setEffect = RelicSetEffectFactory.Create(setId);
                if (setEffect == null || count < setEffect.RequiredCount) continue;

                // ������װЧ����ע�������ߣ�
                setEffect.SetOwner(BattleEntity);
                setEffect.Activate(BattleEntity);
                _activeSetEffects.Add(setId, setEffect);
            }
        }
    }
}
