using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Net.TCP.Message
{
    /// <summary>
    /// 消息基类
    /// </summary>
    public abstract class BaseMessage
    {
        /// <summary>
        /// 获取字节数组长度
        /// </summary>
        /// <returns></returns>
        public abstract int GetBytesLength();

        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns></returns>
        public abstract byte[] Serialize();

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="beginIndex"></param>
        /// <returns></returns>
        public abstract int Deserialize(byte[] bytes, int beginIndex = 0);

        /// <summary>
        /// 写入字段
        /// </summary>
        /// <typeparam name="T">支持byte、shout、int、long、float、double、char、bool、string、BaseMessage、Vector3</typeparam>
        /// <param name="bytes">字节数组</param>
        /// <param name="value">字段值</param>
        /// <param name="index">当前写入位置</param>
        protected void WriteField<T>(byte[] bytes, T value, ref int index)
        {
            switch (value)
            {
                case byte byteValue:
                    bytes[index] = byteValue;
                    index += 1;
                    break;
                case short shortValue:
                    BitConverter.GetBytes(shortValue).CopyTo(bytes, index);
                    index += 2;
                    break;
                case int intValue:
                    BitConverter.GetBytes(intValue).CopyTo(bytes, index);
                    index += 4;
                    break;
                case long longValue:
                    BitConverter.GetBytes(longValue).CopyTo(bytes, index);
                    index += 8;
                    break;
                case float floatValue:
                    BitConverter.GetBytes(floatValue).CopyTo(bytes, index);
                    index += 4;
                    break;
                case double doubleValue:
                    BitConverter.GetBytes(doubleValue).CopyTo(bytes, index);
                    index += 8;
                    break;
                case bool boolValue:
                    BitConverter.GetBytes(boolValue).CopyTo(bytes, index);
                    index += 1;
                    break;
                case char charValue:
                    BitConverter.GetBytes(charValue).CopyTo(bytes, index);
                    index += 2;
                    break;
                case string stringValue:
                    byte[] strBytes = Encoding.UTF8.GetBytes(stringValue);
                    int length = strBytes.Length;
                    BitConverter.GetBytes(length).CopyTo(bytes, index);
                    index += 4;
                    strBytes.CopyTo(bytes, index);
                    index += length;
                    break;
                case BaseMessage baseMessage:
                    baseMessage.Serialize().CopyTo(bytes, index);
                    index += baseMessage.GetBytesLength();
                    break;
                case List<int> listValue:
                    WriteField<int>(bytes, listValue.Count, ref index);
                    for (int i = 0; i < listValue.Count; i++)
                    {
                        WriteField<int>(bytes, listValue[i], ref index);
                    }
                    break;
                default:
                    Debug.LogError($"序列化消息字段失败，未定义该类型的序列化逻辑，{typeof(T)}");
                    break;
            }
        }

        /// <summary>
        /// 读取byte类型
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected byte ReadByte(byte[] bytes, ref int index)
        {
            byte byteValue = bytes[index];
            index += 1;
            return byteValue;
        }

        /// <summary>
        /// 读取short类型
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected short ReadShort(byte[] bytes, ref int index)
        {
            short shortValue = BitConverter.ToInt16(bytes, index);
            index += 2;
            return shortValue;
        }

        /// <summary>
        /// 读取int类型
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected int ReadInt(byte[] bytes, ref int index)
        {
            int intValue = BitConverter.ToInt32(bytes, index);
            index += 4;
            return intValue;
        }

        /// <summary>
        /// 读取long类型
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected long ReadLong(byte[] bytes, ref int index)
        {
            long longValue = BitConverter.ToInt64(bytes, index);
            index += 8;
            return longValue;
        }

        /// <summary>
        /// 读取float类型
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected float Readfloat(byte[] bytes, ref int index)
        {
            float floatValue = BitConverter.ToSingle(bytes, index);
            index += 4;
            return floatValue;
        }

        /// <summary>
        /// 读取double类型
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected double ReadDouble(byte[] bytes, ref int index)
        {
            double doubleValue = BitConverter.ToDouble(bytes, index);
            index += 8;
            return doubleValue;
        }

        /// <summary>
        /// 读取char类型
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected char ReadChar(byte[] bytes, ref int index)
        {
            char charValue = BitConverter.ToChar(bytes, index);
            index += 2;
            return charValue;
        }

        /// <summary>
        /// 读取bool类型
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected bool ReadBool(byte[] bytes, ref int index)
        {
            bool boolValue = BitConverter.ToBoolean(bytes, index);
            index += 1;
            return boolValue;
        }

        /// <summary>
        /// 读取string类型
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected string ReadString(byte[] bytes, ref int index)
        {
            int length = ReadInt(bytes, ref index);
            string strValue = Encoding.UTF8.GetString(bytes, index, length);
            index += length;
            return strValue;
        }

        /// <summary>
        /// 读取BaseMessage类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected T ReadMessage<T>(byte[] bytes, ref int index) where T : BaseMessage, new()
        {
            T message = new T();
            // 当执行该方法时，一定是嵌套消息。
            // 所以这里手动的解析消息消息头（即前12个字节：帧ID、消息ID、消息长度）
            _ = ReadInt(bytes, ref index);
            _ = ReadInt(bytes, ref index);
            _ = ReadInt(bytes, ref index);
            // 反序列化API不会解析消息头
            index += message.Deserialize(bytes, index);
            return message;
        }

        /// <summary>
        /// 读取int类型的列表
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected List<int> ReadListInt(byte[] bytes, ref int index)
        {
            List<int> ints = new List<int>();
            int count = ReadInt(bytes, ref index);
            for (int i = 0; i < count; i++)
            {
                ints.Add(ReadInt(bytes, ref index));
            }
            return ints;
        }
    }
}
