using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillManager
{
    void AddSkillCommand(ISkill skill, IBattleEntityObject entityObject);
}
