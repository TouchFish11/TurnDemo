using System.Collections.Generic;
using Core.Log;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Event.General;

namespace HotUpdate.Game.Battle.Property
{
    /// <summary>
    /// 属性组件抽象类
    /// 封装战斗实体的核心属性管理逻辑，包括属性读写、加成配置、属性变更事件触发等
    /// 所有具体的战斗属性组件需继承此类实现
    /// </summary>
    public abstract class StatComponent : BattleComponent, IPropertyComponent
    {
        // 存储属性加成类型与对应数值的映射字典（key：加成类型，value：加成数值）
        private readonly Dictionary<E_PropertyBonusType, int> _bonusToValueMap = new();
        // 当前战斗实体的核心属性容器
        protected BattleProperty battleProperty;
        
        /// <summary>
        /// 战斗实体是否死亡标识
        /// </summary>
        public bool IsDeath => battleProperty.CurrentHp == 0;
    }
}