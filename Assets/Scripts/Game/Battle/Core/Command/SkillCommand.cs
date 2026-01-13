using Framework;
using Game.Battle;
using System.Collections;

/// <summary>
/// 技能命令
/// 封装技能的调用
/// </summary>
public class SkillCommand : ICommand
{
    /// <summary>
    /// 技能对象
    /// </summary>
    public ISkill Skill { get; private set; }

    public int Priority { get; private set; }

    /// <summary>
    /// 执行技能
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public IEnumerator Excute(IBattleContext context)
    {
        return Skill.Cast(context);
    }

    /// <summary>
    /// 初始化技能命令
    /// </summary>
    /// <param name="skill"></param>
    public void Init(ISkill skill)
    {
        this.Skill = skill;
    }

    void IPoolData.ResetData()
    {
        Skill = null;
    }
}
