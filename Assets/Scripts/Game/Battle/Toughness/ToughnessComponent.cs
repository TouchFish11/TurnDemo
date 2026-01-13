using Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 韧性组件
    /// 管理目标的韧性系统
    /// </summary>
    [ComponentId(nameof(ToughnessComponent))]
    public class ToughnessComponent : BattleComponent, IToughnessComponent
    {
        // 当前韧性状态
        private ToughnessState _toughness;
        // 韧性削减条件策略列表
        private readonly List<IToughnessReduceStrategy> _toughnessReduceStrategies = new List<IToughnessReduceStrategy>();
        // 韧性削减条件策略列表
        private readonly List<IToughnessCalcStrategy> _toughnessCalcStrategies = new List<IToughnessCalcStrategy>();

        void IToughnessComponent.Init(IBattleEntityObject owner, int[] elementTypes , int initialToughness)
        {
            List<E_ElementType> weakPropertys = new List<E_ElementType>(elementTypes.Length);
            foreach (var type in elementTypes)
            {
                weakPropertys.Add(type.ToElementType());
            }
            _toughness = new ToughnessState(weakPropertys, initialToughness);
        }

        public override void BattleInit(IBattleEntityObject battleEntity)
        {
            base.BattleInit(battleEntity);

            MonsterInfo monsterInfo = battleEntity.GetComponent<MonsterObject>().MonsterInfo;
            (this as IToughnessComponent).Init(battleEntity, TextUtility.SplitToIntArr(monsterInfo.f_weaknesses, 2), monsterInfo.f_baseToughness);

            // 添加默认韧性削减策略
            _toughnessReduceStrategies.Add(ToughnessStrategyFactory.GetReduceStrategy<DefaultToughnessReduceStrategy>());
            // 添加默认削减韧性计算策略
            _toughnessCalcStrategies.Add(ToughnessStrategyFactory.GetCalcStrategy<DefaultToughnessCalcStrategy>());
        }

        /// <summary>
        /// 添加削减策略
        /// </summary>
        /// <param name="reduceStrategy"></param>
        public void AddToughnessReduceStrategy(IToughnessReduceStrategy reduceStrategy)
        {
            if (!_toughnessReduceStrategies.Contains(reduceStrategy))
            {
                _toughnessReduceStrategies.Add(reduceStrategy);
                // 按优先级排序
                SortReduceStrategy();
            }
        }

        /// <summary>
        /// 移除削减策略
        /// </summary>
        /// <param name="reduceStrategy"></param>
        public void RemoveToughnessReduceStrategy(IToughnessReduceStrategy reduceStrategy)
        {
            if (_toughnessReduceStrategies.Remove(reduceStrategy))
            {
                // 按优先级排序
                SortReduceStrategy();
            }
        }

        /// <summary>
        /// 排序削减策略
        /// </summary>
        private void SortReduceStrategy()
        {
            _toughnessReduceStrategies.Sort((s1, s2) =>
            {
                if (s1.Priority > s2.Priority)
                {
                    return -1;
                }
                else if (s1.Priority < s2.Priority)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            });
        }

        /// <summary>
        /// 添加计算策略
        /// </summary>
        /// <param name="reduceStrategy"></param>
        public void AddToughnessCalcStrategy(IToughnessCalcStrategy calcStrategy)
        {
            if (!_toughnessCalcStrategies.Contains(calcStrategy))
            {
                _toughnessCalcStrategies.Add(calcStrategy);
                // 按优先级排序
                SortReduceStrategy();
            }
        }

        /// <summary>
        /// 移除计算策略
        /// </summary>
        /// <param name="reduceStrategy"></param>
        public void RemoveToughnessCalcStrategy(IToughnessCalcStrategy calcStrategy)
        {
            if (_toughnessCalcStrategies.Remove(calcStrategy))
            {
                // 按优先级排序
                SortCalcStrategy();
            }
        }

        /// <summary>
        /// 排序计算策略
        /// </summary>
        private void SortCalcStrategy()
        {
            _toughnessCalcStrategies.Sort((s1, s2) =>
            {
                if (s1.Priority > s2.Priority)
                {
                    return -1;
                }
                else if (s1.Priority < s2.Priority)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            });
        }

        /// <summary>
        /// 削减韧性
        /// </summary>
        /// <param name="reducer"></param>
        /// <param name="propertyType"></param>
        /// <param name="value"></param>
        public void ReduceToughness(IBattleEntityObject reducer, E_ElementType propertyType, SkillInfo skillInfo)
        {
            // 能否削减韧性
            if (!CanReduceToughness(reducer, propertyType, skillInfo.f_toughenValue))
            {
                return;
            }

            // 计算最终削韧值
            int finalReduceValue = CalcToughness(reducer, propertyType, skillInfo.f_toughenValue);
            // 计算剩余韧性值
            int current = Mathf.Max(0, _toughness.CurrentToughnessValue - finalReduceValue);
            // 更新韧性
            SetToughnessValue(current, _toughness.MaxToughnessVaue);

            // 触发韧性变化事件
            this.BattleEntity.Context.GetEventBus().TriggerEvent(new ToughnessChangedEvent(this.BattleEntity.Context, this.BattleEntity, _toughness.CurrentToughnessValue, _toughness.MaxToughnessVaue));
            // 判断是否被击破
            if (IsToughnessBroken())
            {
                this.BattleEntity.Context.GetEventBus().TriggerEvent(new ToughnessBrokenEvent(this.BattleEntity.Context, reducer, this.BattleEntity, skillInfo));
            }
        }

        /// <summary>
        /// 设置韧性值
        /// </summary>
        /// <param name="current"></param>
        /// <param name="max"></param>
        public void SetToughnessValue(int current, int max)
        {
            _toughness.SetToughnessValue(current, max);
            // 触发韧性变化事件
            this.BattleEntity.Context.GetEventBus().TriggerEvent(new ToughnessChangedEvent(this.BattleEntity.Context, this.BattleEntity, _toughness.CurrentToughnessValue, _toughness.MaxToughnessVaue));
        }

        /// <summary>
        /// 能否削减韧性
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        private bool CanReduceToughness(IBattleEntityObject reducer, E_ElementType propertyType, int value)
        {
            if (_toughness.IsBroken)
            {
                return false;
            }

            foreach (IToughnessReduceStrategy reduceStrategy in _toughnessReduceStrategies)
            {
                if (reduceStrategy.CanReduceToughness(reducer, this.BattleEntity, propertyType, value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 计算削韧值
        /// </summary>
        /// <param name="reducer"></param>
        /// <param name="propertyType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private int CalcToughness(IBattleEntityObject reducer, E_ElementType propertyType, int value)
        {
            int totalValue = 0;
            foreach (IToughnessCalcStrategy calcStrategy in _toughnessCalcStrategies)
            {
                totalValue += calcStrategy.CalcReduceToughness(reducer, this.BattleEntity, propertyType, value);
            }

            return totalValue;
        }

        /// <summary>
        /// 是否处于击破状态
        /// </summary>
        /// <returns></returns>
        public bool IsToughnessBroken() => _toughness.IsBroken;

        /// <summary>
        /// 最大韧性值
        /// </summary>
        public int CurrentToughnessValue => _toughness.CurrentToughnessValue;

        /// <summary>
        /// 当前韧性值
        /// </summary>
        public int MaxToughnessVaue => _toughness.MaxToughnessVaue;

        /// <summary>
        /// 弱点属性列表
        /// </summary>
        public List<E_ElementType> WeakPropertys => _toughness.WeakPropertys;

    }
}
