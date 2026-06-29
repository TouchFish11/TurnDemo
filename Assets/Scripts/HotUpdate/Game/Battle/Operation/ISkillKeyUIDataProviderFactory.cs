namespace HotUpdate.Game.Battle.Operation
{
    /// <summary>
    /// 技能按键UI数据提供器工厂接口
    /// </summary>
    public interface ISkillKeyUIDataProviderFactory
    {
        /// <summary>
        /// 获取提供器
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <returns></returns>
        ISkillKeyUIDataProvider GetProvider<TProvider>()where TProvider : class, ISkillKeyUIDataProvider;
    }
}
