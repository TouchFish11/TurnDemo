using GameLogic.BattleMoudule.Entity;
using System.Collections;

namespace Game.Battle
{
    /// <summary>
    /// 战斗实体接口
    /// </summary>
    public interface IBattleEntityObject : IEntityObject
    {
        /// <summary>
        /// 临时名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 战斗上下文
        /// </summary>
        IBattleContext Context { get; }

        /// <summary>
        /// 行动值
        /// </summary>
        float ActionValue { get; }

        /// <summary>
        /// 设置行动值
        /// </summary>
        void SetActionValue(float actionValue);

        /// <summary>
        /// 战斗初始化
        /// </summary>
        /// <param name="id"></param>
        /// <param name="context"></param>
        void BattleInit(int id, IBattleContext context);

        /// <summary>
        /// 执行行动
        /// </summary>
        /// <returns></returns>
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
