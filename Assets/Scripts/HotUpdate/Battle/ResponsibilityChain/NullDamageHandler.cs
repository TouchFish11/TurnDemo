using Core.Log;
using HotUpdate.Core.Battle.Damage.Data;

namespace HotUpdate.Battle.ResponsibilityChain
{
    /// <summary>
    /// null伤害处理器
    /// </summary>
    public class NullDamageHandler : Handler<DamageResult>
    {
        public override void HandleRequest(DamageResult request)
        {
            LogManager.LogWarning($"{nameof(NullDamageHandler)}.{nameof(HandleRequest)}：该请求未被处理，{request}");
        }
    }
}
