--定义UIMgr，继承Object
Object:subClass("L_UIMgr")

--创建一个UIMgr实例
L_UIMgr.Instance = L_UIMgr:new()
--存储界面容器
L_UIMgr.panelDic = {}

--根据不同面板名获取不同对象
function L_UIMgr:GetObject(panelName)
    if panelName == "ActivityPanel" then
        return ActivityPanel:new()
    else
        return ActivityDetailPanel:new()
    end
end

--异步显示界面
function L_UIMgr:ShowPanel(panelName, parent)
    if L_UIMgr.panelDic[panelName] then
        return L_UIMgr.panelDic[panelName]
    else
        --AB包同步加载
        local panelObj = ABMgr.Instance:LoadAsset("ui", panelName, typeof(GameObject))
        --实例化面板预设体、设置位置
        local panelInstance = GameObject.Instantiate(panelObj, parent, false)
        --创建面板对象
        local panel = self:GetObject(panelName)
        --记录实例化的预设体
        panel.panelObj = panelInstance
        --调用初始化方法
        panel:Init()
        --调用显示方法
        panel:Show(panelName)
        --存储面板对象
        L_UIMgr.panelDic[panelName] = panel
        --返回外部
        return panel
    end
end

--隐藏界面
function L_UIMgr:HidePanel(panelName)
    if L_UIMgr.panelDic[panelName] then
        L_UIMgr.panelDic[panelName]:Hide(panelName)
    end
end

--获取面板
function L_UIMgr:GetPanel(panelName)
    if L_UIMgr.panelDic[panelName] then
        return L_UIMgr.panelDic[panelName]
    end
    return nil
end

--清空所有面板
function L_UIMgr:ClearPanels()
    for key, _ in pairs(L_UIMgr.panelDic) do
        L_UIMgr.panelDic[key] = nil
    end
end








