using System;

namespace Game.Module
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleExportAttribute : Attribute
    {
        public Type ModuleType { get; }
        
        public ModuleExportAttribute(Type moduleType)
        {
            ModuleType = moduleType;
        }
    }
}
