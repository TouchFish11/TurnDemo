using System.Collections.Generic;
using Core.Log;
using Game.Battle.Component;

namespace GameHotUpdate.Battle.Summon
{
    /// <summary>
    /// �ٻ�������������ɫ���ٻ���������
    /// </summary>
    public class SummonComponent : BattleComponent, ISummonComponent
    {
        // ����ɫ�ɴ�������ٻ�������б�,�������һ���ֶα�ʾ(��ѡ)
        private readonly List<ISummon> _summons = new List<ISummon>();

        /// <summary>
        /// �����ٻ�������ͷ�ʱ���ã�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="initialActionTimes"></param>
        public void CreateSummon<T>() where T : ISummon, new()
        {
            T summon = new T();
            summon.Init(BattleEntity);
            //// ���丳ֵ��ʵ���ù��캯��ע�룬�˴��򻯣�
            //typeof(T).GetProperty(nameof(ISummon.Owner)).SetValue(summon, _owner);
            //typeof(T).GetProperty("_initialActionTimes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(summon, initialActionTimes);

            _summons.Add(summon);
            LogManager.Log($"{BattleEntity.GameObject.name}�ٻ��ˣ�{summon.GameObject.name}");
            // �㲥���ٻ��ﴴ���¼�����������ģ�������
            //BattleEventBus.Publish(new SummonCreatedEvent(_owner.GetBattleComponent<IBattleContext>(), summon, _owner));
        }

        // ��ȡ�����ٻ���
        public List<ISummon> GetAllSummons() => _summons;
    }
}
