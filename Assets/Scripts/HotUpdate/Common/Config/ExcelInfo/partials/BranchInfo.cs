using HotUpdate.Common.Config;

public partial class BranchInfo : IReviewInfo
{
    public IReviewInfo.EReviewType ReviewType => IReviewInfo.EReviewType.Branch;
    
    public string GetViewText()
    {
        return f_optText;
    }
}
