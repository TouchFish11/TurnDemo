using System;
using System.Collections.Generic;
using Core.DI;
using Core.HotUpdate;

namespace HotUpdate.Game.Dialogue
{
    /// <summary>
    /// 分支处理器收集器
    /// </summary>
    public class BranchHandlerCollecor
    {
        // 对话分支处理器缓存
        private readonly Dictionary<EBranchType, IBranchHandler> s_branchHandlers = new();

        private BranchHandlerCollecor(IHotUpdateManager hotUpdateManager)
        {
            foreach (var hotAssembly in hotUpdateManager.GetHotAssemblies())
            {
                foreach (var exportedType in hotAssembly.GetExportedTypes())
                {
                    if (typeof(IBranchHandler).IsAssignableFrom(exportedType) && !exportedType.IsAbstract && !exportedType.IsInterface)
                    {
                        var branchHandler = (IBranchHandler)DIContainer.Create(exportedType);
                        s_branchHandlers.Add(branchHandler.BranchType, branchHandler);
                    }
                }
            }
        }

        /// <summary>
        /// 获取分支处理器
        /// </summary>
        /// <param name="branchType"></param>
        /// <param name="branchHandler"></param>
        /// <returns></returns>
        public bool TryGetHandler(EBranchType branchType, out IBranchHandler branchHandler)
        {
            return s_branchHandlers.TryGetValue(branchType, out branchHandler);
        }
    }
}
