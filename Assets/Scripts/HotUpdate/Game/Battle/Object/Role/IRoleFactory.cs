using System.Threading.Tasks;
using HotUpdate.Game.Battle.Context;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role
{
    public interface IRoleFactory
    {
        Task<IRoleObject> CreateRole(int roleId, int entityIndex, IBattleContext context, Transform parent,
            bool stay = false);
    }
}
