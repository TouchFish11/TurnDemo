using Core.Components;
using Core.DI;
using HotUpdate.Base;

namespace HotUpdate.Game.Battle.Property
{
    /// <summary>
    /// 怪物属性组件
    /// </summary>
    [ComponentId(typeof(MonsterPropertyComponent))]
    public class MonsterPropertyComponent : PropertyComponent
    {
        public override void BattleInit(IBattleEntityObject battleEntity)
        {
            base.BattleInit(battleEntity);

            battleProperty = DIContainer.Create<MonsterProperty>();
            battleProperty.InitProperty(battleEntity.BattleEntityId);
        }
    }
}
