using GameLogic.BattleMoudule.Entity;
using System.Collections;

namespace Game.Battle
{
    /// <summary>
    /// 战斗实体接口
    /// </summary>
    public interface IBattleEntityObject : IEntityObject
    {
        //[Obsolete("TODO：通过配置数据来获取")]
        string Name { get; }

        /// <summary>
        /// 战斗初始化
        /// </summary>
        /// <param name="id"></param>
        /// <param name="context"></param>
        void BattleInit(int id, IBattleContext context);

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
