using System;

namespace Core.Inputs
{
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
