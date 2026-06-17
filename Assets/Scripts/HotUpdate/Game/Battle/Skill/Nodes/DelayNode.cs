using System.Collections;
using HotUpdate.Game.Battle.Skill.Base;
using UnityEngine;

namespace HotUpdate.Game.Battle.Skill.Nodes
{
    /// <summary>
    /// 延迟节点
    /// </summary>
    public class DelayNode : SkillNode
    {
        private WaitForSeconds _waitForSeconds;
        private float _delay;
        
        public DelayNode(ISkill skill) : base(skill)
        {
        
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override IEnumerator Execute()
        {
            yield return _waitForSeconds;
        }
        
        public void SetDelay(float delay)
        {
            _waitForSeconds = new WaitForSeconds(delay);
            _delay = delay;
        }
    }
}
