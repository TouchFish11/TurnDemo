--定义面板基类，继承Object
Object:subClass("BasePanel")

--存储面板预设体
BasePanel.panelObj = nil
--存储控件 键-控件名 值-控件
BasePanel.controlsDic = {}

--按钮监听
function BasePanel:ButtonOnClick(btnName)
    
end

function BasePanel:ToggleValueChanged(togName, isOn)
    
end

function BasePanel:DropDownValueChanged(ddName, index)
    
end

function BasePanel:InputFieldValueChanged(inputName, strValue)
    
end

function BasePanel:SliderValueChanged(slilerName, fValue)
    
end

--初始化(自动寻找控件)
function BasePanel:Init()
    if not self.panelObj then
        return
    end
    --找控件
    local controls = self.panelObj:GetComponentsInChildren(typeof(UIBehaviour))
    --存储所有有用的UI控件
    for i = 0, controls.Length - 1 do
        --记录控件名
        local controlName = controls[i].name
        --只找满足命名规则的控件
        if string.find(controlName, "btn") ~= nil or --按钮
            string.find(controlName, "tog") ~= nil or --开关
            string.find(controlName, "img") ~= nil or --图片
            string.find(controlName, "sv") ~= nil or --滚动视图
            string.find(controlName, "txt") ~= nil or --文本
            string.find(controlName, "dd") ~= nil or --下拉列表
            string.find(controlName, "input") then --输入框
            --避免出现一个对象挂载多个UI控件出现覆盖的问题，可以被同时存到一个表中
            --反射获取控件类型名
            local typeName = controls[i]:GetType().Name
            if self.controlsDic[controlName] then
                --通过自定义索引的方式添加
                self.controlsDic[controlName][typeName] = controls[i]
            else
                self.controlsDic[controlName] =  {[typeName] = controls[i]}
            end
            --添加监听事件
            if typeName == "Button"  then
                controls[i].onClick:AddListener(function()
                    self:ButtonOnClick(controlName)
                end)
            elseif typeName == "Toggle" then
                controls[i].onValueChanged:AddListener(function(isOn)
                    self:ToggleValueChanged(controlName, isOn)
                end)
            elseif typeName == "DropDown" then
                controls[i].onValueChanged:AddListener(function(index)
                    self:DropDownValueChanged(controlName, index)
                end)
            elseif typeName == "InputField" then
                controls[i].onValueChanged:AddListener(function(strValue)
                    self:InputFieldValueChanged(controlName, strValue)
                end)
            elseif typeName == "Slider" then
                controls[i].onValueChanged:AddListener(function(fValue)
                    self:SliderValueChanged(controlName, fValue)
                end)
            end
        end
    end
end

--获取控件
--参数一：控件名 参数二：控件类型 
function BasePanel:GetControl(controlName, typeName)
    if self.controlsDic[controlName] then
        if self.controlsDic[controlName][typeName] then
            return self.controlsDic[controlName][typeName]
        end
    end
    return nil
end

--显示界面
function BasePanel:Show(panelName)
    if package.loaded[panelName] then
        require(panelName)
    end
end

--隐藏界面
function BasePanel:Hide(panelName)
    GameObject.Destroy(self.panelObj)
    package.loaded[panelName] = nil
end














