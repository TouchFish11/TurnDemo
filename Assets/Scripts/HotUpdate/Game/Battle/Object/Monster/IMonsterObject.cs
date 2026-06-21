using HotUpdate.Common.Config.ExcelInfo.Info;
using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Object.Monster
{
    public interface IMonsterObject : IBattleEntityObject
    {
        void MonsterBattleInit(MonsterInfo info, IBattleContext context, Commandfactory factory, IDeathHandler handler);

        /// <summary>
        /// 怪物配置信息（从配置表加载）
        /// </summary>
        MonsterInfo MonsterInfo { get; }
    }
}
