using HotUpdate.Common.Config;

public partial class DialogueInfo : IReviewInfo
{
    public IReviewInfo.EReviewType ReviewType => IReviewInfo.EReviewType.Dialogue;
        
    public string GetViewText()
    {
        return f_dialgueText;
    }
}