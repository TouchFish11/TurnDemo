namespace HotUpdate.Game.Battle.Turn
{
    /// <summary>
    /// 每波次胜利条件类型
    /// </summary>
    public enum EWaveVictoryConditionType : byte
    {
        EliminateAllEnemies,    // 全灭敌人
        SurviveForTurns,        // 存活指定回合数
        EliminateSpecificTarget // 击杀特定目标
    }
}
