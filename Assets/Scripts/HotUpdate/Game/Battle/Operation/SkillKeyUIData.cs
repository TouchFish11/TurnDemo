using System.Collections.Generic;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Operation
{
    /// <summary>
    /// 技能按键UI数据结构体
    /// 封装技能按键UI展示所需的技能信息列表和数据提供方实体
    /// </summary>
    public readonly struct SkillKeyUIData
    {
        /// <summary>
        /// 技能信息列表
        /// 包含需要在UI上展示的所有技能信息
        /// </summary>
        public List<SkillInfo> SkillInfos { get; }

        /// <summary>
        /// 数据提供方战斗实体
        /// 关联的战斗实体对象（如角色、怪物等），作为技能数据的归属方
        /// </summary>
        public IBattleEntityObject Provider { get; }

        /// <summary>
        /// 技能按键UI数据构造函数
        /// </summary>
        /// <param name="skillInfos">技能信息列表</param>
        /// <param name="provider">数据提供方战斗实体</param>
        public SkillKeyUIData(List<SkillInfo> skillInfos, IBattleEntityObject provider)
        {
            SkillInfos = skillInfos;
            Provider = provider;
        }
    }
}
