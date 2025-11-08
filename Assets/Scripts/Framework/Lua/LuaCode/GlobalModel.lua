--定义全局模块类，继承Object
Object:subClass("GlobalModel")

--全局判空函数
function GlobalModel:IsNull(obj)
    if obj == nil or obj:Equals(nil) then
        return true
    end
    return false
end