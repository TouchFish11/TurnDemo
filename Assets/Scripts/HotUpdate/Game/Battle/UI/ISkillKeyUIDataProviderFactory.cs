using HotUpdate.Base.Factory;

namespace HotUpdate.Game.Battle.UI
{
    /// <summary>
    /// 技能按键UI数据提供器工厂接口
    /// </summary>
    public interface ISkillKeyUIDataProviderFactory : IFactory
    {
        /// <summary>
        /// 获取释放技能条件
        /// </summary>
        /// <typeparam name="TProvider">释放技能条件类型</typeparam>
        /// <returns></returns>
        ISkillKeyUIDataProvider GetCastSkillCondition<TProvider>()where TProvider : class, ISkillKeyUIDataProvider;
    }
}
