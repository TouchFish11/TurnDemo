using HotUpdate.Game.Battle.Object.Monster;

namespace HotUpdate.Game.Battle.Object
{
    /// <summary>
    /// 怪物AI驱动：打开操作后立即决策并释放技能。
    /// </summary>
    public class MonsterAIDriver : ITurnActionDriver
    {
        private readonly IMonsterObject _monsterObject;

        public MonsterAIDriver(IMonsterObject monsterObject)
        {
            _monsterObject = monsterObject;
        }

        public void OnOperationOpened()
        {
            // TODO：可以封装随机选择的策略类，用于玩家/怪物AI
            var skillId = _monsterObject.SelectSkill();
            _monsterObject.CastSkill(skillId);
        }
    }
}