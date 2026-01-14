using Framework;
using Game.Battle;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 战斗界面数据
/// </summary>
public class BattleModel : UIModel
{
    // 行动条格子UI列表
    private readonly List<ActionGridUI> actions = new List<ActionGridUI>();
    // 技能按键UI列表
    private readonly List<SkillKeyUI> skillKeyUIs = new List<SkillKeyUI>();
    // 角色状态UI列表
    private readonly List<RoleStateUI> roleStateUIs = new List<RoleStateUI>();
    // 普通怪物状态UI列表
    private readonly List<NormalMonsterStateUI> normalMonsterStateUIs = new List<NormalMonsterStateUI>();
    // 战技点UI列表
    private readonly List<BattlePointUI> battlePointUIs = new List<BattlePointUI>();
    // 选择标记UI列表
    private readonly List<SelectMarkerUI> selectMarkerUIs = new List<SelectMarkerUI>();
    // 等待行动对象列表
    private readonly List<WaitingActUI> waitingActUIs = new List<WaitingActUI>();
    // 当前累计伤害
    private long currentCalcDamage;

    /// <summary>
    /// 隐藏普通怪物状态UI
    /// </summary>
    /// <param name="deadMonster"></param>
    public void HideNormalMonsterStateUI(IBattleEntityObject deadMonster)
    {
        NormalMonsterStateUI normalMonsterStateUI = normalMonsterStateUIs.Find((m) => m.BattleEntity == deadMonster);
        normalMonsterStateUIs.Remove(normalMonsterStateUI);
        ServiceLocator.Get<IPoolManager>().PushObj(normalMonsterStateUI.gameObject);
    }

    /// <summary>
    /// 通过ID获取角色状态UI
    /// 使用Linq查询
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns>未找到返回null</returns>
    public RoleStateUI GetRoleStateUIById(int roleId)
    {
        return roleStateUIs.FirstOrDefault(r => r.RoleId == roleId);
    }

    /// <summary>
    /// 更新等待命令UI
    /// </summary>
    /// <param name="waitingActUIs"></param>
    public void UpdateWaitingCommmand(List<WaitingActUI> waitingActUIs)
    {
        foreach (WaitingActUI waitingActUI in this.waitingActUIs)
        {
            ServiceLocator.Get<IPoolManager>().PushObj(waitingActUI.gameObject);
        }
        this.waitingActUIs.Clear();
        this.waitingActUIs.AddRange(waitingActUIs);
    }

    /// <summary>
    /// 更新普通怪物状态UI
    /// </summary>
    /// <param name="normalMonsterStateUIs"></param>
    public void UpdateNormalMonsterState(IEnumerable<NormalMonsterStateUI> normalMonsterStateUIs)
    {
        foreach (NormalMonsterStateUI monsterStateUI in this.normalMonsterStateUIs)
        {
            ServiceLocator.Get<IPoolManager>().PushObj(monsterStateUI.gameObject);
        }
        this.normalMonsterStateUIs.Clear();

        this.normalMonsterStateUIs.AddRange(normalMonsterStateUIs);
    }

    /// <summary>
    /// 更新行动栏
    /// </summary>
    /// <param name="actionGridUIs"></param>
    public void UpdateAcitonbar(IEnumerable<ActionGridUI> actionGridUIs)
    {
        foreach (ActionGridUI actionGridUI in actions)
        {
            ServiceLocator.Get<IPoolManager>().PushObj(actionGridUI.gameObject);
        }
        actions.Clear();
        actions.AddRange(actionGridUIs);
    }

    /// <summary>
    /// 获取所有的行动格子
    /// </summary>
    /// <returns></returns>
    public List<ActionGridUI> GetActionGridUIs()
    {
        return actions;
    }

    /// <summary>
    /// 设置操作UI
    /// </summary>
    /// <param name="skillKeyUIs"></param>
    public void SetOperator(List<SkillKeyUI> skillKeyUIs)
    {
        foreach (SkillKeyUI skillKeyUI in this.skillKeyUIs)
        {
            ServiceLocator.Get<IPoolManager>().PushObj(skillKeyUI.gameObject);
        }

        this.skillKeyUIs.Clear();
        this.skillKeyUIs.AddRange(skillKeyUIs);
    }

    /// <summary>
    /// 清除操作UI
    /// </summary>
    public void ClearOperator()
    {
        foreach (SkillKeyUI skillKeyUI in this.skillKeyUIs)
        {
            ServiceLocator.Get<IPoolManager>().PushObj(skillKeyUI.gameObject);
        }
        this.skillKeyUIs.Clear();
    }

    /// <summary>
    /// 更新战技点数
    /// </summary>
    /// <param name="current"></param>
    /// <param name="battlePointUIs"></param>
    public void UpdateBattlePointCount(int current, IEnumerable<BattlePointUI> battlePointUIs)
    {
        foreach (BattlePointUI battlePointUI in this.battlePointUIs)
        {
            ServiceLocator.Get<IPoolManager>().PushObj(battlePointUI.gameObject);
        }
        this.battlePointUIs.Clear();
        this.battlePointUIs.AddRange(battlePointUIs);
    }

    /// <summary>
    /// 更新选择标记
    /// </summary>
    /// <param name="selectMarkerUIs"></param>
    public void UpdateSelectMarker(List<SelectMarkerUI> selectMarkerUIs)
    {
        ClearSelectMarker();
        this.selectMarkerUIs.AddRange(selectMarkerUIs);
    }

    /// <summary>
    /// 清除标记
    /// </summary>
    public void ClearSelectMarker()
    {
        foreach (SelectMarkerUI selectMarkerUI in this.selectMarkerUIs)
        {
            ServiceLocator.Get<IPoolManager>().PushObj(selectMarkerUI.gameObject);
        }
        this.selectMarkerUIs.Clear();
    }

    /// <summary>
    /// 初始化角色状态UI
    /// </summary>
    /// <param name="roleStateUIs"></param>
    public void InitRoleStateUI(IEnumerable<RoleStateUI> roleStateUIs)
    {
        this.roleStateUIs.AddRange(roleStateUIs);
    }

    /// <summary>
    /// 设置累计伤害文本
    /// </summary>
    /// <param name="dmg"></param>
    /// <param name="isClear"></param>
    /// <returns></returns>
    public long SetCumulativeDamage(int dmg, bool isClear)
    {
        if (!isClear)
        {
            currentCalcDamage += dmg;
        }
        else
        {
            currentCalcDamage = 0;
        }

        return currentCalcDamage;
    }
}
