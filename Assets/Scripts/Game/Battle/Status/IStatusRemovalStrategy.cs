using System.Collections.Generic;

namespace Game.Battle.Status
{
    /// <summary>
    /// ״̬�Ƴ�����
    /// </summary>
    public interface IStatusRemovalStrategy
    {
        void HandleRemove(List<IStatus> statuses);
    }
}
