using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.StatSystem;

namespace HotUpdate.Game.Battle.Damage
{
    /// <summary>
    /// 持续伤害计算数据
    /// 包含计算持续伤害的所有属性
    /// </summary>
    public class DotDamageCalcData
    {
        public IBattleEntityObject source { get; set; }
        
        public IBattleEntityObject target  { get; set; }
        
        public EElementType ElementType { get; set; }
        
        // 测试
        public int Damage { get; set; }
    }
}
