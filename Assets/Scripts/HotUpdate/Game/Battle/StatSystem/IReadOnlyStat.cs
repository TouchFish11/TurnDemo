namespace HotUpdate.Game.Battle.StatSystem
{
    /// <summary>
    /// 只读属性接口
    /// </summary>
    public interface IReadOnlyStat
    {
        EStatType StatType { get; }
        
        float BaseValue { get; }
        
        float FinalValue { get; }
    }
}
