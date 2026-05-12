using System;
using System.Text;
using Core.Log;
using UnityEngine.Events;

namespace Core.Utility
{
    /// <summary>
    /// 文本处理工具类
    /// 提供字符串分割、类型转换、数值格式化、时间/文件大小单位转换等通用文本处理方法
    /// </summary>
    public static class TextUtility
    {
        // 时间字符串拼接专用StringBuilder，避免频繁创建销毁提升性能
        private static readonly StringBuilder _timeBuilder = new StringBuilder();

        /// <summary>
        /// 按指定分隔符类型分割字符串
        /// 自动处理中文全角分隔符转半角后再分割，保证分割逻辑统一
        /// </summary>
        /// <param name="str">待分割的字符串（空字符串会触发警告并返回空数组）</param>
        /// <param name="type">分隔符类型枚举：
        /// 1-分号(;) 2-逗号(,) 3-百分号(%) 4-冒号(:) 5-空格( ) 6-竖线(|) 7-下划线(_)
        /// </param>
        /// <returns>分割后的字符串数组，无有效内容时返回空数组</returns>
        public static string[] Split(string str, int type)
        {
            // 空字符串校验
            if (str == "")
            {
                Logger.LogWarning("待分割的字符串长度为0");
                return Array.Empty<string>();
            }

            // 临时变量存储处理后的字符串（全角转半角）
            var newStr = str;
            switch (type)
            {
                case 1:
                    // 中文全角分号转半角分号后分割
                    while (newStr.IndexOf("；") != -1)
                        newStr = newStr.Replace("；", ";");
                    return newStr.Split(';');
                case 2:
                    // 中文全角逗号转半角逗号后分割
                    while (newStr.IndexOf("，") != -1)
                        newStr = newStr.Replace("，", ",");
                    return newStr.Split(",");
                case 3:
                    // 百分号直接分割（无全角半角差异）
                    return newStr.Split('%');
                case 4:
                    // 中文全角冒号转半角冒号后分割
                    while (newStr.IndexOf("：") != -1)
                        newStr = newStr.Replace("：", ":");
                    return newStr.Split(":");
                case 5:
                    // 空格分割（无全角半角差异）
                    return newStr.Split(' ');
                case 6:
                    // 竖线分割（无全角半角差异）
                    return newStr.Split("|");
                case 7:
                    // 下划线分割（无全角半角差异）
                    return newStr.Split('_');
                default:
                    // 无效分隔符类型告警
                    Logger.LogError($"未提供该类型的分割方式，当前type值为，{type}。范围：1-7");
                    return Array.Empty<string>();
            }
        }

        /// <summary>
        /// 按指定分隔符分割字符串并转换为int数组
        /// 转换失败的元素会返回int默认值(0)并触发警告
        /// </summary>
        /// <param name="str">待分割的字符串（空字符串会触发警告并返回空数组）</param>
        /// <param name="type">分隔符类型枚举：
        /// 1-分号(;) 2-逗号(,) 3-百分号(%) 4-冒号(:) 5-空格( ) 6-竖线(|) 7-下划线(_)
        /// </param>
        /// <returns>转换后的int数组，无有效内容时返回空数组</returns>
        public static int[] SplitToIntArr(string str, int type)
        {
            // 先执行字符串分割
            var newStr = Split(str, type);
            if (newStr.Length == 0)
            {
                Logger.LogWarning("待转换的字符串数组长度为0，无法转为int数组");
                return Array.Empty<int>();
            }

            // 批量转换字符串数组为int数组
            return Array.ConvertAll(newStr, (singleStr) =>
            {
                // 安全转换，避免格式错误导致异常
                if (int.TryParse(singleStr, out var value))
                    return value;
                
                // 转换失败告警
                Logger.LogWarning($"字符串「{singleStr}」转换int失败，返回默认值0");
                return 0;
            });
        }

        /// <summary>
        /// 按指定分隔符分割字符串并转换为float数组
        /// 转换失败的元素会返回float默认值(0.0f)并触发警告
        /// </summary>
        /// <param name="str">待分割的字符串（空字符串会触发警告并返回空数组）</param>
        /// <param name="type">分隔符类型枚举：
        /// 1-分号(;) 2-逗号(,) 3-百分号(%) 4-冒号(:) 5-空格( ) 6-竖线(|) 7-下划线(_)
        /// </param>
        /// <returns>转换后的float数组，无有效内容时返回空数组</returns>
        public static float[] SplitTofloatArr(string str, int type)
        {
            // 先执行字符串分割
            var newStr = Split(str, type);
            if (newStr.Length == 0)
            {
                Logger.LogWarning("待转换的字符串数组长度为0，无法转为float数组");
                return Array.Empty<float>();
            }

            // 批量转换字符串数组为float数组
            return Array.ConvertAll(newStr, (singleStr) =>
            {
                // 安全转换，避免格式错误导致异常
                if (float.TryParse(singleStr, out var value))
                    return value;
                
                // 转换失败告警
                Logger.LogWarning($"字符串「{singleStr}」转换float失败，返回默认值0.0f");
                return 0;
            });
        }

        /// <summary>
        /// 双层分割字符串并转换为int键值对，通过回调返回每一组结果
        /// 示例："1,2;3,4" 先按分号(类型1)分割为["1,2","3,4"]，再按逗号(类型2)分割为[1,2]和[3,4]
        /// 1-分号(;) 2-逗号(,) 3-百分号(%) 4-冒号(:) 5-空格( ) 6-竖线(|) 7-下划线(_)
        /// </summary>
        /// <param name="str">待双层分割的字符串（空字符串会触发警告并终止执行）</param>
        /// <param name="firstSplitCharType">第一层分隔符类型（枚举值1-7）</param>
        /// <param name="secondSplitCharType">第二层分隔符类型（枚举值1-7）</param>
        /// <param name="callBack">回调函数，返回每一组分割转换后的int键值对</param>
        public static void SplitMultiple(string str, int firstSplitCharType, int secondSplitCharType, UnityAction<int, int> callBack)
        {
            // 第一层分割
            var newStr = Split(str, firstSplitCharType);
            if (newStr.Length == 0)
            {
                Logger.LogWarning("第一层分割后的字符串数组长度为0，终止双层分割");
                return;
            }

            // 遍历第一层分割结果，执行第二层分割
            foreach (var s in newStr)
            {
                // 第二层分割并转换为int数组
                var ints = SplitToIntArr(s, secondSplitCharType);
                // 跳过无效的第二层分割结果（长度不足2时无键值对）
                if (ints.Length < 2)
                    continue;
                // 回调返回当前组的键值对
                callBack?.Invoke(ints[0], ints[1]);
            }
        }

        /// <summary>
        /// 双层分割字符串并转换为string键值对，通过回调返回每一组结果
        /// 示例："a,b;c,d" 先按分号(类型1)分割为["a,b","c,d"]，再按逗号(类型2)分割为[a,b]和[c,d]
        /// </summary>
        /// <param name="str">待双层分割的字符串（空字符串会触发警告并终止执行）</param>
        /// <param name="firstSplitCharType">第一层分隔符类型（枚举值1-7）</param>
        /// <param name="secondSplitCharType">第二层分隔符类型（枚举值1-7）</param>
        /// <param name="callBack">回调函数，返回每一组分割后的string键值对</param>
        public static void SplitMultiple(string str, int firstSplitCharType, int secondSplitCharType, UnityAction<string, string> callBack)
        {
            // 第一层分割
            var newStr = Split(str, firstSplitCharType);
            if (newStr.Length == 0)
            {
                Logger.LogWarning("第一层分割后的字符串数组长度为0，终止双层分割");
                return;
            }

            // 遍历第一层分割结果，执行第二层分割
            foreach (var s in newStr)
            {
                // 第二层分割
                var strs = Split(s, secondSplitCharType);
                // 跳过无效的第二层分割结果（长度不足2时无键值对）
                if (strs.Length < 2)
                    continue;
                // 回调返回当前组的键值对
                callBack?.Invoke(strs[0], strs[1]);
            }
        }

        /// <summary>
        /// 数字转换为指定长度的字符串，长度不足时左侧补0
        /// 示例：NumToStr(5, 3) → "005"；NumToStr(123, 2) → "123"（长度超过时不截断）
        /// </summary>
        /// <param name="num">待转换的长整型数字</param>
        /// <param name="length">目标字符串长度</param>
        /// <returns>补0后的固定长度字符串</returns>
        public static string NumToStr(long num, int length)
        {
            return num.ToString($"D{length}");
        }

        /// <summary>
        /// 浮点数转换为指定小数位数的字符串，自动四舍五入
        /// 示例：FloatToStr(3.1415f, 2) → "3.14"；FloatToStr(2.5f, 0) → "3"
        /// </summary>
        /// <param name="value">待转换的浮点数</param>
        /// <param name="places">保留的小数位数（非负整数）</param>
        /// <returns>指定小数位数的字符串</returns>
        public static string FloatToStr(float value, int places)
        {
            return value.ToString($"F{places}");
        }

        /// <summary>
        /// 将秒数转换为「天/时/分/秒」格式的时间字符串
        /// 支持自定义单位文本、是否显示0值、是否保留两位数字（不足补0）
        /// </summary>
        /// <param name="scends">总秒数（负数会强制转为0）</param>
        /// <param name="dayStr">天的单位文本（如"天"、"d"）</param>
        /// <param name="hourStr">时的单位文本（如"时"、"h"）</param>
        /// <param name="minStr">分的单位文本（如"分"、"m"）</param>
        /// <param name="scendStr">秒的单位文本（如"秒"、"s"）</param>
        /// <param name="isEgZero">是否显示0值（true=显示，false=仅显示非0部分）</param>
        /// <param name="isKeepTwoPlaces">是否保留两位数字（true=补0，false=原始数字）</param>
        /// <returns>格式化后的时间字符串</returns>
        public static string SecondToHMS(long scends, string dayStr, string hourStr, string minStr, string scendStr, bool isEgZero = false, bool isKeepTwoPlaces = true)
        {
            // 处理负数秒数
            if (scends < 0)
                scends = 0;
            
            // 计算天、时、分、秒
            var day = scends / (3600 * 24);          // 总天数 = 总秒数 / 一天总秒数(86400)
            var hour = scends % (3600 * 24) / 3600;  // 剩余小时 = 总秒数取模一天秒数 / 一小时秒数(3600)
            var min = scends % 3600 / 60;            // 剩余分钟 = 总秒数取模一小时秒数 / 一分钟秒数(60)
            var second = scends % 60;                // 剩余秒数 = 总秒数取模一分钟秒数

            // 清空复用的StringBuilder，避免残留旧数据
            _timeBuilder.Clear();

            // 拼接天（仅当有值/显示0值 且 单位文本非空时）
            if ((day > 0 || !isEgZero) && !string.IsNullOrEmpty(dayStr))
            {
                _timeBuilder.Append(isKeepTwoPlaces ? NumToStr(day, 2) : day);
                _timeBuilder.Append(dayStr);
            }

            // 拼接时（仅当有值/显示0值 且 单位文本非空时）
            if ((hour > 0 || !isEgZero) && !string.IsNullOrEmpty(hourStr))
            {
                _timeBuilder.Append(isKeepTwoPlaces ? NumToStr(hour, 2) : hour);
                _timeBuilder.Append(hourStr);
            }

            // 拼接分（有值/显示0值/有小时 且 单位文本非空时）
            if ((min > 0 || !isEgZero || hour > 0) && !string.IsNullOrEmpty(minStr))
            {
                _timeBuilder.Append(isKeepTwoPlaces ? NumToStr(min, 2) : min);
                _timeBuilder.Append(minStr);
            }

            // 拼接秒（有值/显示0值/有分/有小时 且 单位文本非空时）
            if ((second > 0 || !isEgZero || min > 0 || hour > 0) && !string.IsNullOrEmpty(scendStr))
            {
                _timeBuilder.Append(isKeepTwoPlaces ? NumToStr(second, 2) : second);
                _timeBuilder.Append(scendStr);
            }

            return _timeBuilder.ToString();
        }

        /// <summary>
        /// 将字节数转换为易读的单位字符串（B/KB/MB/GB）
        /// 转换规则：≥1GB显示GB（保留2位小数），≥1MB显示MB（保留2位小数），≥1KB显示KB（整数），否则显示B
        /// </summary>
        /// <param name="size">字节数（无符号长整型）</param>
        /// <returns>带单位的文件大小字符串</returns>
        public static string ToByteUnit(ulong size)
        {
            // 转换为GB（1GB = 1024*1024*1024 B）
            if (size / (1024f * 1024 * 1024) > 1f)
            {
                return $"{size / (1024f * 1024 * 1024):F2}GB";
            }
            // 转换为MB（1MB = 1024*1024 B）
            else if (size / (1024f * 1024) > 1f)
            {
                return $"{size / (1024f * 1024):F2}MB";
            }
            // 转换为KB（1KB = 1024 B）
            else if (size / 1024f > 1f)
            {
                return $"{size / 1024}KB";
            }
            // 保留为B
            else
            {
                return $"{size}B";
            }
        }
    }
}