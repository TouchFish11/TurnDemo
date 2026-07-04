using HotUpdate.Base.Factory;
using HotUpdate.Game.Battle.Skill.Handler;

namespace HotUpdate.Game.Battle.Skill.Factory
{
    public interface ISkillCastPostHandlerFactory : IFactory
    {
        /// <summary>
        /// 获取技能释放后处理器
        /// </summary>
        /// <typeparam name="THandler">技能释放后处理器类型</typeparam>
        /// <returns></returns>
        ISkillCastPostHandler GetSkillCastPostHandler<THandler>()where THandler : class, ISkillCastPostHandler;
    }
}
