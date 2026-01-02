
using System.Collections.Generic;
using Game;
using Game.Battle;

public enum E_CharacterType
{
    /// <summary>
    /// 玩家角色
    /// </summary>
    PlayerCharacter,
    /// <summary>
    /// 怪物角色
    /// </summary>
    MonsterCharacter,
}

/// <summary>
/// 战斗处理工具
/// </summary>
public class BattleUtil
{
    /// <summary>
    /// 获取技能范围所有目标
    /// </summary>
    /// <param name="characterType"></param>
    /// <param name="mainTarget"></param>
    /// <param name="rangeType"></param>
    /// <returns></returns>
    public static List<IBattleEntityObject> GetRangeTargets(E_CharacterType characterType, IBattleEntityObject mainTarget, int rangeType)
    {
        List<IBattleEntityObject> allTargets = new List<IBattleEntityObject>();
        List<IBattleEntityObject> finalTargets = new List<IBattleEntityObject>();

        // 判断选择者
        switch (characterType)
        {
            case E_CharacterType.PlayerCharacter:
                // 主目标是玩家角色
                if (mainTarget is PlayerObject)
                    allTargets.AddRange(mainTarget.Context.GetPlayerObjects());
                // 主目标是怪物角色
                else
                    allTargets.AddRange(mainTarget.Context.GetMonsterObjects());
                break;
            case E_CharacterType.MonsterCharacter:
                // 主目标是怪物角色
                if (mainTarget is MonsterObject)
                    allTargets.AddRange(mainTarget.Context.GetMonsterObjects());
                // 主目标是玩家角色
                else
                    allTargets.AddRange(mainTarget.Context.GetPlayerObjects());
                break;
        }

        switch ((E_SkillRangeType)rangeType)
        {
            case E_SkillRangeType.Single:
                // 只包括主目标
                finalTargets.Add(mainTarget);
                break;
            case E_SkillRangeType.Diffusion:
                // 包括主目标和相邻目标
                finalTargets.Add(mainTarget);
                if (allTargets.Count > 1)
                {
                    int mainIndex = allTargets.IndexOf(mainTarget);
                    // 最左端
                    if (mainIndex == 0)
                        finalTargets.Add(allTargets[mainIndex + 1]);
                    // 最右端
                    else if (mainIndex == allTargets.Count - 1)
                        finalTargets.Add(allTargets[mainIndex - 1]);
                    // 不是最左/右
                    else
                    {
                        finalTargets.Add(allTargets[mainIndex - 1]);
                        finalTargets.Add(allTargets[mainIndex + 1]);
                    }
                }
                break;
            case E_SkillRangeType.All:
                //包括全体目标
                finalTargets = allTargets;
                break;
        }

        return finalTargets;
    }

    /// <summary>
    /// 获取主目标
    /// </summary>
    /// <param name="skillInfo"></param>
    /// <param name="caster"></param>
    /// <returns></returns>
    public static IBattleEntityObject GetMainTarget(SkillInfo skillInfo, IBattleContext context, IBattleEntityObject caster)
    {
        // 获取技能目标类型
        E_SkillTargetType targetType = (E_SkillTargetType)skillInfo.f_targetType;
        // 根据技能目标类型获取所有敌方/友方实体

        List<IBattleEntityObject> targets = null;
        if (caster is PlayerObject)
        {
            targets = new List<IBattleEntityObject>(targetType == E_SkillTargetType.Enemy ? context.GetMonsterObjects() : context.GetPlayerObjects());
        }
        else if(caster is MonsterObject)
        {
            targets = new List<IBattleEntityObject>(targetType == E_SkillTargetType.Enemy ? context.GetPlayerObjects() : context.GetMonsterObjects());
        }

        IBattleEntityObject currentMainTarget = null;
        // 若当前目标为空且当前选中的目标已经死亡，则需要重新选择目标；否则就默认选中上次选中的目标
        while (currentMainTarget == null || currentMainTarget.GetComponent<PropertyComponent>().IsDeath)
        {
            int targetNum = targets.Count;
            // 若没有目标，则不用选择，返回空
            if (targetNum == 0)
            {
                return null;
            }
            // 若只有一个目标，则默认选择该目标
            else if (targetNum == 1)
            {
                currentMainTarget = targets[0];
            }
            // 若有多个目标，则默认选择靠近中间的目标
            else
            {
                // 奇数数量，选中中间目标；偶数数量，选中右边目标
                currentMainTarget = targets[targetNum / 2];
            }
        }
        // 返回主目标
        return currentMainTarget;
    }

    /// <summary>
    /// 获取当前技能作用的目标角色(包含主目标)
    /// </summary>
    /// <param name="characterType">选择者类型</param>
    /// <param name="mainTarget">主目标</param>
    /// <param name="rangeType">技能范围类型</param>
    /// <returns>技能作用范围的目标</returns>
    //public static list<ibattletarget> getrangetargets(e_charactertype charactertype, ibattletarget maintarget, int rangetype)
    //{
    //    list<ibattletarget> alltargets = new list<ibattletarget>();
    //    list<ibattletarget> finaltargets = new list<ibattletarget>();

    //    //判断选择者
    //    switch (charactertype)
    //    {
    //        case e_charactertype.playercharacter:
    //            //主目标是玩家角色
    //            if (maintarget is playercharacter)
    //                alltargets.addrange(battlemanager.instance.charactersmanager.getallactplayercharacter());
    //            //主目标是怪物角色
    //            else
    //                alltargets.addrange(battlemanager.instance.charactersmanager.getallactmonstercharacter());
    //            break;
    //        case e_charactertype.monstercharacter:
    //            //主目标是怪物角色
    //            if (maintarget is monstercharacter)
    //                alltargets.addrange(battlemanager.instance.charactersmanager.getallactmonstercharacter());
    //            //主目标是玩家角色
    //            else
    //                alltargets.addrange(battlemanager.instance.charactersmanager.getallactplayercharacter());
    //            break;
    //    }

    //    switch ((e_skillrangetype)rangetype)
    //    {
    //        case e_skillrangetype.single:
    //            //只包括主目标
    //            finaltargets.add(maintarget);
    //            break;
    //        case e_skillrangetype.diffusion:
    //            //包括主目标和相邻目标
    //            finaltargets.add(maintarget);
    //            if (alltargets.count > 1)
    //            {
    //                int mainindex = alltargets.indexof(maintarget);
    //                //最左端
    //                if (mainindex == 0)
    //                    finaltargets.add(alltargets[mainindex + 1]);
    //                //最右端
    //                else if (mainindex == alltargets.count - 1)
    //                    finaltargets.add(alltargets[mainindex - 1]);
    //                //不是最左/右
    //                else
    //                {
    //                    finaltargets.add(alltargets[mainindex - 1]);
    //                    finaltargets.add(alltargets[mainindex + 1]);
    //                }
    //            }
    //            break;
    //        case e_skillrangetype.all:
    //            //包括全体目标
    //            finaltargets = alltargets;
    //            break;
    //    }

    //    //设置为选中
    //    for (int i = 0; i < finaltargets.count; i++)
    //    {
    //        (finaltargets[i] as iactionable).setselectflag(true);
    //    }

    //    return finaltargets;
    //}

    /// <summary>
    /// 选择主目标
    /// </summary>
    /// <returns></returns>
    //public static IBattleTarget SelectMainTarget(IBattleTarget battleTarget)
    //{
    //    IBattleTarget currentMainTarget = null;
    //    List<IBattleTarget> targets = new List<IBattleTarget>();

    //    E_TargetType targetType = (E_TargetType)battleTarget..f_target_type;
    //    targets.AddRange(targetType == E_TargetType.Enemy ? BattleManager.Instance.CharactersManager.GetAllActMonsterCharacter() :
    //                        BattleManager.Instance.CharactersManager.GetAllActPlayerCharacter());
    //    //若当前目标为空且当前选中的目标已经死亡，则需要重新选择目标；否则就默认选中上次选中的目标
    //    while (currentMainTarget == null || currentMainTarget.IsDeath)
    //    {
    //        int targetNum = targets.Count;
    //        //若没有目标，则不用选择，返回空
    //        if (targetNum == 0)
    //            return null;
    //        //若只有一个目标，则默认选择该目标
    //        else if (targetNum == 1)
    //            currentMainTarget = targets[0];
    //        //若有多个目标，则默认选择靠近中间的目标
    //        else
    //            //奇数数量，选中中间目标；偶数数量，选中右边目标
    //            currentMainTarget = targets[targetNum / 2];
    //    }
    //    //返回主目标
    //    return currentMainTarget;
    //}

    /// <summary>
    /// 获取当前角色的技能信息
    /// </summary>
    /// <returns></returns>
    //public static T_SkillInfo GetCurrentCharacterSkillInfo()
    //{
    //    return BinaryDataManager.Instance.GetTable<T_SkillInfoContainer>().dataDic[BattleManager.Instance.BattleFlowController.GetCurrentActCharacter().GetCurrentSkillID()];
    //}

    /// <summary>
    /// 技能范围类型转文本内容
    /// </summary>
    /// <param name="skillRangeType">技能范围类型</param>
    /// <returns></returns>
    //public static string SkillRangeTypeToText(int skillRangeType)
    //{
    //    return (E_SkillRangeType)skillRangeType switch
    //    {
    //        E_SkillRangeType.Single => "单攻",
    //        E_SkillRangeType.Diffusion => "扩散",
    //        E_SkillRangeType.All => "群体",
    //        _ => "无",
    //    };
    //}
}
