using Game.Battle.Enum;
using Game.Battle.Objects;
using Game.Battle.Skill.Enum;

namespace Game.Battle.Damage
{
    /// <summary>
    /// 伤害结果结构体
    /// 用于封装单次战斗中产生的伤害相关的所有核心信息
    /// </summary>
    public readonly struct DamageResult
    {
        /// <summary>
        /// 伤害来源实体
        /// </summary>
        public IBattleEntityObject Source { get; }

        /// <summary>
        /// 伤害目标实体
        /// </summary>
        public IBattleEntityObject Target { get; }

        /// <summary>
        /// 最终结算伤害值
        /// 经过减伤、增伤、抗性等所有计算后实际生效的伤害数值
        /// </summary>
        public int FinalDamage { get; }

        /// <summary>
        /// 伤害元素类型
        /// </summary>
        public E_ElementType ElementType { get; }

        /// <summary>
        /// 伤害类型
        /// </summary>
        public E_DamageType DamageType { get; }

        /// <summary>
        /// 是否为暴击伤害
        /// 标记本次伤害是否触发了暴击判定
        /// </summary>
        public bool IsCrit { get; }

        /// <summary>
        /// 技能信息
        /// 关联本次伤害的技能元数据（如技能ID、技能等级、技能类型等）
        /// </summary>
        public SkillInfo SkillInfo { get; }

        /// <summary>
        /// 伤害结果结构体构造函数
        /// </summary>
        /// <param name="source">伤害来源实体</param>
        /// <param name="target">伤害目标实体</param>
        /// <param name="finalDamage">最终结算伤害值</param>
        /// <param name="elementType">伤害元素类型</param>
        /// <param name="damageType">伤害类型</param>
        /// <param name="isCrit">是否为暴击伤害</param>
        /// <param name="skillInfo">技能信息</param>
        public DamageResult(IBattleEntityObject source, IBattleEntityObject target, int finalDamage, E_ElementType elementType, E_DamageType damageType, bool isCrit, SkillInfo skillInfo)
        {
            Source = source;
            Target = target;
            FinalDamage = finalDamage;
            ElementType = elementType;
            DamageType = damageType;
            IsCrit = isCrit;
            SkillInfo = skillInfo;
        }
    }
}