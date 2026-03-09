using HotUpdate.Core.Battle.Object;
using UnityEngine;

namespace HotUpdate.Core.Module
{
    public interface IBattleModule : IModule
    {
        IPlayerObject AddWarrior(GameObject warrior);
    }
}
