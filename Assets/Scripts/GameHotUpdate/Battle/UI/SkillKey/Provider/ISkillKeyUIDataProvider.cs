using GameHotUpdate.Battle.Object;

namespace GameHotUpdate.Battle.UI.SkillKey.Provider
{
    /// <summary>
    /// 技能按键UI数据提供器接口
    /// 定义获取技能按键UI数据的统一契约
    /// </summary>
    public interface ISkillKeyUIDataProvider
    {
        /// <summary>
        /// 获取技能按键UI数据
        /// </summary>
        /// <param name="provider">数据提供方战斗实体</param>
        /// <returns>封装好的技能按键UI数据结构体</returns>
        SkillKeyUIData GetData(IBattleEntityObject provider);
    }
}