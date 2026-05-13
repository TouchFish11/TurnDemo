using Core.Log;
using HotUpdate.Base.Battle.Damage.Data;

namespace HotUpdate.Game.Battle.ResponsibilityChain
{
    /// <summary>
    /// null伤害处理器
    /// </summary>
    public class NullDamageHandler : Handler<DamageResult>
    {
        public override void HandleRequest(DamageResult request)
        {
            Logger.LogWarning($"{nameof(NullDamageHandler)}.{nameof(HandleRequest)}：该请求未被处理，{request}");
        }
    }
}
