using System.Collections.Generic;
using Core.Log;

namespace Test
{
    /// <summary>
    /// 属性加成修改器
    /// </summary>
    public class StatsModifier
    {
        private readonly Dictionary<EStatType, BonusData> _totalBonusDatas = new();
        private readonly Dictionary<IStatsModifierSource, Dictionary<EStatType, BonusData>> bonusDatas = new();
        private readonly StatsComponent _statsComponent;

        public StatsModifier(StatsComponent statsComponent)
        {
            _statsComponent = statsComponent;
        }
        
        /// <summary>
        /// 继承IStatsModifierSource的对象需要调用该方法注册
        /// </summary>
        /// <param name="source">继承IStatsModifierSource的对象</param>
        public void RegisterSource(IStatsModifierSource source) 
        {
            bonusDatas.Add(source, new Dictionary<EStatType, BonusData>());
            // 注册变化回调
            source.OnModifiersChanged += () => OnSourceChanged(source);
        }
    
        private void OnSourceChanged(IStatsModifierSource source)
        {
            if (!bonusDatas.TryGetValue(source, out var data))
            {
                Logger.Log($"No stats modifier found for {source}");
                return;
            }
            
            // 先移除原来的加成
            foreach (var kvp in data)
            {
                if (!_totalBonusDatas.TryGetValue(kvp.Key, out var bonusData)) continue;
                bonusData.BuildValue -= kvp.Value.BuildValue;
                bonusData.PercentValue -= kvp.Value.PercentValue;
            }
                
            // 重新计算当前来源的加成集合
            data.Clear();
            source.GetModifier(data);
                
            // 添加到总加成中
            foreach (var kvp in data)
            {
                if (_totalBonusDatas.TryGetValue(kvp.Key, out var bonusData))
                {
                    bonusData.BuildValue += kvp.Value.BuildValue;
                    bonusData.PercentValue += kvp.Value.PercentValue;
                }
                else
                {
                    _totalBonusDatas.Add(kvp.Key, kvp.Value);
                }
            }
                
            // 更新变化的属性
            foreach (var type in data.Keys) 
            {
                // 通知容器该属性及其依赖需要更新
                _statsComponent.OnModifiersChanged(type);
            }
        }
    
        /// <summary>
        /// 获取属性的总固定加成
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public float GetBuildBonus(EStatType type) => _totalBonusDatas.TryGetValue(type, out var data) ? data.BuildValue : 0;
        
        /// <summary>
        /// 获取属性的总百分比加成
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public float GetPercentBonus(EStatType type) => _totalBonusDatas.TryGetValue(type, out var data) ? data.PercentValue : 0;
    }
}
