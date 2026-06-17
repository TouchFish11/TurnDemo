using System.Collections;

namespace HotUpdate.Game.Battle.Skill.Base
{
    /// <summary>
    /// 技能效果接口
    /// </summary>
    public interface ISkillNode
    {
        bool CanExecute();
        
        IEnumerator Execute();
    }
}
