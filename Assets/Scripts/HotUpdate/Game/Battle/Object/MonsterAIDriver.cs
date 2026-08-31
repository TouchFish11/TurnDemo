using System.Threading.Tasks;
using HotUpdate.Game.Battle.Object.Monster;

namespace HotUpdate.Game.Battle.Object
{
    /// <summary>
    /// 怪物AI驱动
    /// </summary>
    public class MonsterAIDriver : ITurnActionDriver
    {
        private readonly IMonsterObject _monsterObject;

        public MonsterAIDriver(IMonsterObject monsterObject)
        {
            _monsterObject = monsterObject;
        }
        
        public Task WaitForOperation()
        {
            // TODO：可以封装随机选择的策略类，用于玩家/怪物AI
            var skillId = _monsterObject.SelectSkill();
            // 释放选中的技能
            _monsterObject.CastSkill(skillId);
            return Task.CompletedTask;
        }
    }
}
