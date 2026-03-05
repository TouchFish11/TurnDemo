using Core.Reflection;

namespace HotUpdate.Battle.Skill.Conditions
{
    /// <summary>
    /// 释放技能条件工厂接口
    /// </summary>
    public interface ICastSkillConditionFactory : IFactory
    {
        /// <summary>
        /// 获取释放技能条件
        /// </summary>
        /// <typeparam name="TCondition">释放技能条件类型</typeparam>
        /// <returns></returns>
        ICastSkillCondition GetCastSkillCondition<TCondition>()where TCondition : class, ICastSkillCondition;
    }
}
