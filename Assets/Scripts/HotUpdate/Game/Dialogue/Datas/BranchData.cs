namespace HotUpdate.Game.Dialogue.Datas
{
    /// <summary>
    /// 运行时分支数据
    /// </summary>
    public class BranchData
    {
        public EBranchType BranchType { get; }
        
        public BranchInfo BranchInfo { get; }

        public BranchData(EBranchType branchType,  BranchInfo branchInfo)
        {
            BranchType = branchType;
            BranchInfo = branchInfo;
        }
    }
}
