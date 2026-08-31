namespace HotUpdate.Game.Battle.Object.Monster
{
    public interface IMonsterObject : IBattleEntityObject
    {
        void MonsterBattleInit(BattleParameterObject parameter);

        /// <summary>
        /// 怪物配置信息（从配置表加载）
        /// </summary>
        MonsterInfo MonsterInfo { get; }

        /// <summary>
        /// 选择技能
        /// </summary>
        /// <returns></returns>
        int SelectSkill();
    }
}
