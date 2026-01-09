using Game.Battle;
using System.Collections.Generic;

/// <summary>
/// ×´Ì¬ÒÆ³ý²ßÂÔ
/// </summary>
public interface IStatusRemovalStrategy
{
    void HandleRemove(List<IStatus> statuses);
}
