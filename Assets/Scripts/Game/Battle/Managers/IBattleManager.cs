using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 战斗管理器接口
/// </summary>
public interface IBattleManager
{
    IBattleContext GetContext();
    Task StartBattle();
}
