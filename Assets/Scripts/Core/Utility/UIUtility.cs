using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Logger = Core.Log.Logger;

namespace Core.Utility
{
    /// <summary>
    /// UI工具类
    /// </summary>
    public static class UIUtility
    {
        /// <summary>
        /// 添加自定义事件监听
        /// </summary>
        /// <param name="control">要监听的控件</param>
        /// <param name="type">事件类型</param>
        /// <param name="listener">监听函数</param>
        public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> listener)
        {
            if (!control.TryGetComponent<EventTrigger>(out var eventTrigger))
            {
                eventTrigger = control.AddComponent<EventTrigger>();
            }

            var entry = new EventTrigger.Entry { eventID = type };
            entry.callback.AddListener(listener);
            eventTrigger.triggers.Add(entry);
        }

        /// <summary>
        /// 世界转UI坐标
        /// </summary>
        /// <param name="world">世界摄像机</param>
        /// <param name="ui">UI摄像机</param>
        /// <param name="parent">父对象</param>
        /// <param name="uiObj">世界点</param>
        /// <param name="worldPoint">世界点</param>
        /// <param name="offset">UI坐标偏移</param>
        public static bool WorldToLocalPointInRectangle(Camera world, Camera ui, Transform parent, GameObject uiObj, Vector3 worldPoint, Vector2 offset = default)
        {
            // 世界转屏幕
            var screenPoint = RectTransformUtility.WorldToScreenPoint(world, worldPoint);
            // 屏幕转UI
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parent as RectTransform, screenPoint, ui, out var localPoint))
            {
                //设置父对象
                uiObj.transform.SetParent(parent, false);
                ((RectTransform)uiObj.transform).anchoredPosition = localPoint + offset;
                return true;
            }

            Logger.LogWarning("世界转UI坐标，转换失败");
            return false;
        }

    }
}
