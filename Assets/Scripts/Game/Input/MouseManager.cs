using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 鼠标管理器
/// </summary>
public class MouseManager : SingletonAutoMono<MouseManager>, IMouseManager
{
    // 默认锁定模式
    private readonly CursorLockMode defaultLockMode = CursorLockMode.Locked;
    // 默认是否显示
    private readonly bool defaultVisible = false;
    // 记录每个触发鼠标状态的对象标识
    private readonly Stack<string> mouseVisibleSources = new Stack<string>();

    private void Awake()
    {
        EventCenter.Instance.AddEventListener<IUIController>(E_EventType.E_OpenView, OnOpenView);
        EventCenter.Instance.AddEventListener<IUIController>(E_EventType.E_CloseView, OnCloseView);
    }

    /// <summary>
    /// 申请显示并解锁鼠标
    /// </summary>
    /// <param name="sorce"></param>
    public void RequestMouseVisible(string sorce)
    {
        if (!CanVisible(sorce))
        {
            return;
        }

        if (mouseVisibleSources.TryPeek(out string value))
        {
            if (value == sorce)
            {
                return;
            }
        }

        LogManager.Log($"{value}：请求鼠标可见");
        mouseVisibleSources.Push(sorce);
        UpdateMouseState();
    }

    /// <summary>
    /// 释放鼠标显示状态
    /// </summary>
    /// <param name="sorce"></param>
    public void ReleaseMouseVisible(string sorce)
    {
        if (mouseVisibleSources.TryPeek(out string value))
        {
            if (value != sorce)
            {
                return;
            }

            LogManager.Log($"{value}：释放鼠标可见");
            mouseVisibleSources.Pop();
            UpdateMouseState();
        }
    }

    /// <summary>
    /// 更新鼠标状态
    /// </summary>
    private void UpdateMouseState()
    {
        if (mouseVisibleSources.Count > 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = defaultLockMode;
            Cursor.visible = defaultVisible;
        }
    }

    private bool CanVisible(string sorce)
    {
        if (sorce == typeof(MainController).Name)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 打开界面事件回调
    /// </summary>
    /// <param name="controller"></param>
    private void OnOpenView(IUIController controller)
    {
        RequestMouseVisible(controller.ToString());
    }

    /// <summary>
    /// 关闭界面事件回调
    /// </summary>
    /// <param name="controller"></param>
    private void OnCloseView(IUIController controller)
    {
        ReleaseMouseVisible(controller.ToString());
    }

    protected override void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<IUIController>(E_EventType.E_OpenView, OnOpenView);
        EventCenter.Instance.RemoveEventListener<IUIController>(E_EventType.E_CloseView, OnCloseView);
        base.OnDestroy();
    }

    /// <summary>
    /// 鼠标可见状态
    /// </summary>
    public bool Visible => Cursor.visible;
}
