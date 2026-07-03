using System.Threading.Tasks;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role
{
    public interface IRoleFactory
    {
        Task<IPlayerObject> CreateRole(int roleId, Transform parent, bool stay = false);
    }
}
