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
    /// <summary>
    /// 获取上下文
    /// </summary>
    /// <returns></returns>
    IBattleContext GetContext();

    /// <summary>
    /// 启动战斗
    /// 外部调用
    /// </summary>
    Task StartBattle();
}
