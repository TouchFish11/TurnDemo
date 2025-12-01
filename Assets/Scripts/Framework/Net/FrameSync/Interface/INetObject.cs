using Net.FrameSync.Command;
using UnityEngine;
using static Net.FrameSync.FSFrameHandler;

/// <summary>
/// 网络对象接口
/// </summary>
public interface INetObject
{
    /// <summary>
    /// 对象位置
    /// </summary>
    Transform Transform { get; }

    //ICharacter Character { get; }

    /// <summary>
    /// 收集收入
    /// </summary>
    /// <param name="clientFrameCommand"></param>
    void CollectInput(ClientFrameCommand clientFrameCommand);

    /// <summary>
    /// 同步逻辑状态
    /// </summary>
    /// <param name="clientFrameCommand"></param>
    void SyncLogic(ClientFrameCommand clientFrameCommand, CommandArg commandArg);

    /// <summary>
    /// 追帧
    /// </summary>
    /// <param name="clientFrameCommand"></param>
    /// <param name="commandArg"></param>
    void ChaseFrame(ClientFrameCommand clientFrameCommand, CommandArg commandArg);

    /// <summary>
    /// 同步帧
    /// </summary>
    /// <param name="clientFrameCommand"></param>
    void SyncFrame(ClientFrameCommand clientFrameCommand);
}
