using Framework;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.BattleMoudule.Summon
{
    /// <summary>
    /// 召唤物管理组件（角色的召唤物容器）
    /// </summary>
    public class SummonComponent : MonoBehaviour, ISummonComponent
    {
        // 若角色可创建多个召唤物可用列表,否则可以一个字段表示(可选)
        private List<ISummon> _summons = new();

        public IEntityObject EntityObject { get; private set; }

        public void Init(IEntityObject entityObject)
        {
            EntityObject = entityObject;
        }

        /// <summary>
        /// 创建召唤物（技能释放时调用）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="initialActionTimes"></param>
        public void CreateSummon<T>() where T : ISummon, new()
        {
            T summon = new T();
            summon.Init(EntityObject as IBattleEntityObject);
            //// 反射赋值（实际用构造函数注入，此处简化）
            //typeof(T).GetProperty(nameof(ISummon.Owner)).SetValue(summon, _owner);
            //typeof(T).GetField("_initialActionTimes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(summon, initialActionTimes);

            _summons.Add(summon);
            LogMgr.Log($"{(EntityObject as IBattleEntityObject).Name}召唤了：{summon.Name}");
            // 广播“召唤物创建事件”（供连击模块监听）
            //BattleEventBus.Publish(new SummonCreatedEvent(_owner.GetBattleComponent<IBattleContext>(), summon, _owner));
        }

        // 获取所有召唤物
        public List<ISummon> GetAllSummons() => _summons;
    }
}
