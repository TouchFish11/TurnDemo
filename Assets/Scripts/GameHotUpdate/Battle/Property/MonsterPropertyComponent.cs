using Core.Components;
using Game.Battle.Objects;
using Game.Battle.Property;

namespace GameHotUpdate.Battle.Property
{
    /// <summary>
    /// �����������
    /// </summary>
    [ComponentId(typeof(MonsterPropertyComponent))]
    public class MonsterPropertyComponent : PropertyComponent
    {
        public override void BattleInit(IBattleEntityObject battleEntity)
        {
            base.BattleInit(battleEntity);

            battleProperty = new MonsterProperty();
            battleProperty.InitProperty(battleEntity.BattleEntityId);
        }
    }
}
