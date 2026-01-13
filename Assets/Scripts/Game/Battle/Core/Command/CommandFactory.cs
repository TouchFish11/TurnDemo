
using Framework;
using Game.Battle;

/// <summary>
/// 命令工厂
/// </summary>
public class CommandFactory : IFactory
{
    void IFactory.InitFactory()
    {

    }

    T IFactory.GetTypeInstance<T>()
    {
        return null;
    }

    /// <summary>
    /// 获取技能命令
    /// </summary>
    /// <param name="skill"></param>
    /// <returns></returns>
    public SkillCommand GetSkillCommand(ISkill skill)
    {
        SkillCommand skillCommand = PoolManager.Instance.GetData<SkillCommand>();
        skillCommand.Init(skill);
        return skillCommand; 
    }

    /// <summary>
    /// 获取技能命令
    /// </summary>
    /// <param name="skill"></param>
    /// <returns></returns>
    public ToughnessCommand GetToughnessCommand(ToughnessComponent component)
    {
        ToughnessCommand command = PoolManager.Instance.GetData<ToughnessCommand>();
        command.Init(component);
        return command;
    }

    // 获取其它命令
    // ...
}
