using System.Collections.Generic;
using Game.Battle.Component;
using Game.Battle.Enum;
using Game.Battle.Objects;

namespace Game.Battle.Toughness
{
    /// <summary>
    /// 韧性组件接口
    /// 定义了战斗实体韧性系统的核心行为，包括韧性初始化、增减、策略管理、破韧状态判断等
    /// </summary>
    public interface IToughnessComponent : IBattleComponent
    {
        /// <summary>
        /// 初始化韧性组件
        /// </summary>
        /// <param name="owner">所属战斗实体对象</param>
        /// <param name="elementTypes">弱点元素类型数组</param>
        /// <param name="initialToughness">初始韧性值（同时作为最大韧性值）</param>
        void Init(IBattleEntityObject owner, int[] elementTypes, int initialToughness);

        /// <summary>
        /// 添加韧性减免策略
        /// 策略会影响韧性扣除的计算逻辑
        /// </summary>
        /// <param name="reduceStrategy">待添加的韧性减免策略实例</param>
        void AddToughnessReduceStrategy(IToughnessReduceStrategy reduceStrategy);

        /// <summary>
        /// 移除韧性减免策略
        /// </summary>
        /// <param name="reduceStrategy">待移除的韧性减免策略实例</param>
        void RemoveToughnessReduceStrategy(IToughnessReduceStrategy reduceStrategy);

        /// <summary>
        /// 添加韧性计算策略
        /// 策略会影响韧性最大值/当前值的计算逻辑
        /// </summary>
        /// <param name="calcStrategy">待添加的韧性计算策略实例</param>
        void AddToughnessCalcStrategy(IToughnessCalcStrategy calcStrategy);

        /// <summary>
        /// 移除韧性计算策略
        /// </summary>
        /// <param name="calcStrategy">待移除的韧性计算策略实例</param>
        void RemoveToughnessCalcStrategy(IToughnessCalcStrategy calcStrategy);

        /// <summary>
        /// 扣除韧性值
        /// 根据攻击者、元素类型、技能信息，结合已挂载的策略计算并扣除韧性
        /// </summary>
        /// <param name="reducer">发起韧性扣除的战斗实体（攻击者）</param>
        /// <param name="propertyType">触发韧性扣除的元素类型</param>
        /// <param name="skillInfo">触发韧性扣除的技能信息</param>
        void ReduceToughness(IBattleEntityObject reducer, E_ElementType propertyType, SkillInfo skillInfo);

        /// <summary>
        /// 直接设置韧性值
        /// 用于强制修改当前韧性和最大韧性（如buff/道具效果）
        /// </summary>
        /// <param name="current">要设置的当前韧性值</param>
        /// <param name="max">要设置的最大韧性值</param>
        void SetToughnessValue(int current, int max);

        /// <summary>
        /// 判断是否处于破韧状态
        /// 破韧状态通常指当前韧性值≤0的状态
        /// </summary>
        /// <returns>true=已破韧，false=未破韧</returns>
        bool IsToughnessBroken();

        /// <summary>
        /// 获取当前韧性值
        /// </summary>
        int CurrentToughnessValue { get; }

        /// <summary>
        /// 获取最大韧性值
        /// </summary>
        int MaxToughnessVaue { get; }

        /// <summary>
        /// 获取弱点元素类型列表
        /// 对弱点元素的攻击会造成更多韧性扣除
        /// </summary>
        List<E_ElementType> WeakPropertys { get; }
    }
}