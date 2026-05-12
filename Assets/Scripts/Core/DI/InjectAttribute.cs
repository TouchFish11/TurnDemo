using System;

namespace Core.DI
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class InjectAttribute : Attribute
    {
        
    }
}
