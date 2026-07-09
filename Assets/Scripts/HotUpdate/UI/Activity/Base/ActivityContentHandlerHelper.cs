using System.Reflection;
using Core.DI;
using HotUpdate.Base.Data;
using HotUpdate.Game.Activity.Core;

namespace HotUpdate.UI.Activity.Base
{
    public class ActivityContentHandlerHelper
    {
        public static IActivityContentHandler CreateHandler(ActivityData data)
        {
            var type = data.GetType();
            var attribute = type.GetCustomAttribute<ActivityIdAttribute>();
            if(attribute == null)
                return null;

            return DIContainer.Create(attribute.ActivityContentHandler) as IActivityContentHandler;
        }
    }
}
