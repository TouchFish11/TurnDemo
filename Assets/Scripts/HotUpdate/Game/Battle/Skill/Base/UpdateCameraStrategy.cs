using System.Collections;
using Core.DI;
using HotUpdate.Game.Battle.Core;

namespace HotUpdate.Game.Battle.Skill.Base
{
    public abstract class UpdateCameraStrategy : IUpdateCameraStrategy
    {
        [Inject] protected BattleCoordinator battleCoordinator;
    
        public abstract IEnumerator UpdateCamera(SkillContext skillContext);
    }
}
