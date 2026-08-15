namespace HotUpdate.Game.Battle.Object.Monster
{
    public interface IMonsterObject : IBattleEntityObject
    {
        void MonsterBattleInit(MonsterBattleInitData initData);

        /// <summary>
        /// 怪物配置信息（从配置表加载）
        /// </summary>
        MonsterInfo MonsterInfo { get; }
    }
}
