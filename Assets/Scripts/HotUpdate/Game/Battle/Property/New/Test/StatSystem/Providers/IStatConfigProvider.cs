namespace HotUpdate.Game.Battle.Property.New.Test.StatSystem.Providers
{
    /// <summary>
    /// 属性配置提供器接口
    /// </summary>
    public interface IStatConfigProvider
    {
        /// <summary>
        /// 通过指定类型获取对应配置属性值
        /// </summary>
        /// <param name="statType"></param>
        /// <returns></returns>
        float GetConfigValue(EStatType statType);
    }
}
