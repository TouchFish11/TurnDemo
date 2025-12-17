using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 遗器管理组件（角色的遗器容器，负责加载单件/套装效果）
    /// 战斗外存储仪器数据，战斗时通过数据初始化仪器组件，然后在动态添加仪器效果
    /// </summary>
    public class RelicComponent : BattleComponent, IRelicComponent
    {
        // 已装备仪器
        private readonly List<IRelic> _equippedRelics = new List<IRelic>();
        // 仪器套装映射
        private readonly Dictionary<int, IRelicSetEffect> _activeSetEffects = new Dictionary<int, IRelicSetEffect>();

        /// <summary>
        /// 装备遗器（新增遗器仅需调用此方法，无需改其他代码）
        /// </summary>
        /// <param name="relic"></param>
        public void EquipRelic(IRelic relic)
        {
            _equippedRelics.Add(relic);
            Console.WriteLine($"{BattleEntity.Name}装备遗器：{relic.Name}");

            // 触发单件属性加成
            foreach (var effect in relic.SingleEffects)
            {
                BattleEntity.AddRelicBonus(effect.RelicBoun, effect.BounValue);
            }

            // 检查套装效果（统计同套装件数，满足条件则激活）
            CheckAndActivateSetEffects();
        }

        /// <summary>
        /// 套装效果激活逻辑（独立封装，不依赖外部）
        /// </summary>
        private void CheckAndActivateSetEffects()
        {
            // 统计各套装的装备件数
            var setCount = _equippedRelics.GroupBy(r => r.RelicID)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var (setId, count) in setCount)
            {
                // 从配置表获取对应套装效果（示例：量子套对应QuantumRelicSetEffect）
                var setEffect = RelicSetEffectFactory.Create(setId);
                if (setEffect == null || count < setEffect.RequiredCount) continue;

                // 激活套装效果（注入所有者）
                setEffect.SetOwner(BattleEntity);
                setEffect.Activate(BattleEntity);
                _activeSetEffects.Add(setId, setEffect);
            }
        }
    }
}
