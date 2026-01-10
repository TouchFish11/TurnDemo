using System;
using System.Text;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 文本工具类
    /// </summary>
    public static class TextUtility
    {
        //时间String
        private static readonly StringBuilder _timeBuilder = new StringBuilder();

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="str">只含单种分割符的字符串</param>
        /// <param name="type">拆分字符类型：1-; 2-, 3-% 4-: 5-空格 6-| 7-_ </param>
        /// <returns></returns>
        public static string[] Split(string str, int type)
        {
            if (str == "")
            {
                LogManager.LogWarning("进行分割的字符串长度为0");
                return new string[0];
            }

            //避免改变原字符串的内容
            string newStr = str;

            switch (type)
            {
                case 1:
                    //避免配置表出现中文符号，将中文符号转换为英文符号，再进行分割
                    while (newStr.IndexOf("；") != -1)
                        newStr = newStr.Replace("；", ";");
                    return newStr.Split(';');
                case 2:
                    //避免配置表出现中文符号，将中文符号转换为英文符号，再进行分割
                    while (newStr.IndexOf("，") != -1)
                        newStr = newStr.Replace("，", ",");
                    return newStr.Split(",");
                case 3:
                    return newStr.Split('%');
                case 4:
                    //避免配置表出现中文符号，将中文符号转换为英文符号，再进行分割
                    while (newStr.IndexOf("：") != -1)
                        newStr = newStr.Replace("：", ":");
                    return newStr.Split(":");
                case 5:
                    return newStr.Split(' ');
                case 6:
                    return newStr.Split("|");
                case 7:
                    return newStr.Split('_');
                default:
                    LogManager.LogError("没有提供该符号类型的分割方式");
                    return new string[0];
            }
        }

        /// <summary>
        /// 分割字符串后转int数组
        /// </summary>
        /// <param name="str">只含单种分割符的字符串</param>
        /// <param name="type">拆分字符类型：1-; 2-, 3-% 4-: 5-空格 6-| 7-_ </param>
        /// <returns></returns>
        public static int[] SplitToIntArr(string str, int type)
        {
            string[] newStr = Split(str, type);
            if (newStr.Length == 0)
            {
                LogManager.LogWarning("进行转换的字符串数组长度为0");
                return new int[0];
            }

            //把字符串数组转为int数组
            return Array.ConvertAll(newStr, (str) =>
            {
                if(int.TryParse(str, out int value))
                    return value;
                LogManager.LogWarning("字符串转换失败，已用默认值代替");
                return default;
            });
        }

        /// <summary>
        /// 分割字符串后转float数组
        /// </summary>
        /// <param name="str">只含单种分割符的字符串</param>
        /// <param name="type">拆分字符类型：1-; 2-, 3-% 4-: 5-空格 6-| 7-_ </param>
        /// <returns></returns>
        public static float[] SplitTofloatArr(string str, int type)
        {
            string[] newStr = Split(str, type);
            if (newStr.Length == 0)
            {
                LogManager.LogWarning("进行转换的字符串数组长度为0");
                return new float[0];
            }

            //把字符串数组转为int数组
            return Array.ConvertAll(newStr, (str) =>
            {
                if (float.TryParse(str, out float value))
                    return value;
                LogManager.LogWarning("字符串转换失败，已用默认值代替");
                return default;
            });
        }

        /// <summary>
        /// 分割多个分隔符的字符串后转int数组
        /// </summary>
        /// <param name="str">包含多个分割符的字符串</param>
        /// <param name="firstSplitCharType">第一种分隔符，类型：1-; 2-, 3-% 4-: 5-空格 6-| 7-_ </param>
        /// <param name="secondSplitCharType">第二种分割符，类型：1-; 2-, 3-% 4-: 5-空格 6-| 7-_ </param>
        /// <param name="callBack">每组分割后回调</param>
        public static void SplitMultiple(string str, int firstSplitCharType, int secondSplitCharType, UnityAction<int, int> callBack)
        {
            //先用第一个分隔符分割字符串
            string[] newStr = Split(str, firstSplitCharType);
            if (newStr.Length == 0)
            {
                LogManager.LogWarning("进行分割的字符串数组长度为0");
                return;
            }

            int[] ints;
            for (int i = 0; i < newStr.Length; i++)
            {
                ints = SplitToIntArr(newStr[i], secondSplitCharType);
                if (ints.Length == 0)
                    continue;
                callBack?.Invoke(ints[0], ints[1]);
            }
        }

        /// <summary>
        /// 分割多个分隔符的字符串后返回string数组
        /// </summary>
        /// <param name="str">包含多个分割符的字符串</param>
        /// <param name="firstSplitCharType">第一种分隔符，类型：1-; 2-, 3-% 4-: 5-空格 6-| 7-_ </param>
        /// <param name="secondSplitCharType">第二种分割符，类型：1-; 2-, 3-% 4-: 5-空格 6-| 7-_ </param>
        /// <param name="callBack">每组分割后回调</param>
        public static void SplitMultiple(string str, int firstSplitCharType, int secondSplitCharType, UnityAction<string, string> callBack)
        {
            //先用第一个分隔符分割字符串
            string[] newStr = Split(str, firstSplitCharType);
            if (newStr.Length == 0)
            {
                LogManager.LogWarning("进行分割的字符串数组长度为0");
                return;
            }

            string[] strs;
            for (int i = 0; i < newStr.Length; i++)
            {
                strs = Split(newStr[i], secondSplitCharType);
                if (strs.Length == 0)
                    continue;
                callBack?.Invoke(strs[0], strs[1]);
            }
        }

        /// <summary>
        /// 数字转指定长度的字符串
        /// </summary>
        /// <param name="num">数值</param>
        /// <param name="length">转换后的字符串长度，数值长度不足则在字符串前以0补全长度</param>
        /// <returns>字符串数字</returns>
        public static string NumToStr(long num, int length)
        {
            return num.ToString($"D{length}");
        }

        /// <summary>
        /// 浮点数转指定小数点位数的字符串
        /// </summary>
        /// <param name="value">浮点数</param>
        /// <param name="places">小数点后保留的位数，会四舍五入</param>
        /// <returns>字符串数字</returns>
        public static string FloatToStr(float value, int places)
        {
            return value.ToString($"F{places}");
        }

        /// <summary>
        /// 秒转时分秒
        /// </summary>
        /// <param name="scends">总秒数</param>
        /// <param name="dayStr">天单位</param>
        /// <param name="hourStr">时单位</param>
        /// <param name="minStr">分单位</param>
        /// <param name="scendStr">秒单位</param>
        /// <param name="isEgZero">是否忽略0</param>
        /// <param name="isKeepTwoPlaces">是否保留两位数</param>
        /// <returns>字符串时间</returns>
        public static string SecondToHMS(long scends, string dayStr, string hourStr, string minStr, string scendStr, bool isEgZero = false, bool isKeepTwoPlaces = true)
        {
            if(scends < 0)
                scends = 0;
            //计算天数
            long day = scends / (3600 * 24);
            //计算小时
            long hour = scends % (3600 * 24) / 3600;
            //计算分钟
            long min = scends % (3600 * 24) % 3600 / 60;
            //计算秒
            long second = scends % 60;

            _timeBuilder.Clear();

            //拼接天数
            if ((day > 0 || !isEgZero) && dayStr != "")
            {
                _timeBuilder.Append(isKeepTwoPlaces ? NumToStr(day, 2) : day);
                //天单位
                _timeBuilder.Append(dayStr);
            }
            //拼接小时：有小时或不忽略0时都要拼接
            if ((hour > 0 || !isEgZero) && hourStr != "")
            {
                _timeBuilder.Append(isKeepTwoPlaces ? NumToStr(hour, 2) : hour);
                //小时单位
                _timeBuilder.Append(hourStr);
            }
            //拼接分钟：有分钟或不忽略0或有小时时都要拼接
            if ((min > 0 || !isEgZero || hour > 0) && minStr != "")
            {
                _timeBuilder.Append(isKeepTwoPlaces ? NumToStr(min, 2) : min);
                //分钟单位
                _timeBuilder.Append(minStr);
            }
            //拼接秒：有秒或不忽略0或有分钟或有小时时都要拼接
            if ((second > 0 || !isEgZero || min > 0 || hour > 0) && scendStr != "")
            {
                _timeBuilder.Append(isKeepTwoPlaces ? NumToStr(second, 2) : second);
                //秒单位
                _timeBuilder.Append(scendStr);
            }
            return _timeBuilder.ToString();
        }

        /// <summary>
        /// 转不同字节单位
        /// </summary>
        /// <param name="size">字节数</param>
        /// <returns></returns>
        public static string ToByteUnit(ulong size)
        {
            // B / 1024 * 1024 * 1024 = GB
            if (size / (1024f * 1024 * 1024) > 1f)
            {
                return $"{size / (1024f * 1024 * 1024):F2}GB";
            }
            // B / 1024 * 1024 = MB
            else if (size / (1024f * 1024) > 1f)
            {
                return $"{size / (1024f * 1024):F2}MB";
            }
            // B / 1024 = KB
            else if (size / 1024f > 1f)
            {
                return $"{size / 1024}KB";
            }
            // B
            else
            {
                return $"{size}B";
            }
        }
    }
}
