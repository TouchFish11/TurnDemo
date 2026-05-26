using HotUpdate.Base.Factory;

namespace HotUpdate.Game.Battle.Skill
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
