using System;

namespace Core.Inputs.ActionAsset
{
    /// <summary>
    /// ��Ƕ���·��ӳ���滻�ؼ���
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ActionMapReplaceKeyAttribute : Attribute
    {
        public string ReplaceKey { get; }

        public ActionMapReplaceKeyAttribute(string replaceKey)
        {
            ReplaceKey = replaceKey;
        }
    }
}
