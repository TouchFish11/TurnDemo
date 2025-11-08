using GameLogic.BattleMoudule.Entity;
using GameLogic.BattleMoudule.Relic;
using System.Collections;

namespace GameLogic.BattleMoudule
{
    /// <summary>
    /// 战斗实体接口
    /// </summary>
    public interface IBattleEntity
    {
        string Name { get; }

        /// <summary>
        /// 获取组件
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        bool GetBattleComponent<TComponent>(out TComponent component) where TComponent : IComponent;

        IEnumerator ExecuteAction();

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        int GetField(E_FieldType propertyType);

        /// <summary>
        /// 添加遗器属性加成
        /// </summary>
        /// <param name="propertyType"></param>
        void AddRelicBonus(E_RelicBoun relicBoun, float value);

        /// <summary>
        /// 获取速度
        /// </summary>
        /// <returns></returns>
        int GetSpeed();

        /// <summary>
        /// 回血
        /// </summary>
        /// <param name="value"></param>
        void Heal(int value);

        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="value"></param>
        void TakeDamage(int damage, E_PropertyType propertyType);
    }
}
