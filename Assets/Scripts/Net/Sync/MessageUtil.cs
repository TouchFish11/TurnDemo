using System;
using System.Text;
using Net.Sync.Msg;

namespace Net.Sync
{
    /// <summary>
    /// 消息工具类
    /// </summary>
    public static class MessageUtil
    {
        /// <summary>
        /// д���ֶ�
        /// </summary>
        /// <typeparam name="T">֧��byte��shout��int��long��float��double��char��bool��string��BaseMessage</typeparam>
        /// <param name="bytes">�ֽ�����</param>
        /// <param name="value">�ֶ�ֵ</param>
        /// <param name="index">��ǰд��λ��</param>
        public static void WriteField<T>(byte[] bytes, T value, ref int index)
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
                    var strBytes = Encoding.UTF8.GetBytes(stringValue);
                    var length = strBytes.Length;
                    BitConverter.GetBytes(length).CopyTo(bytes, index);
                    index += 4;
                    strBytes.CopyTo(bytes, index);
                    index += length;
                    break;
                case FrameMessage frameMessage:
                    frameMessage.Serialize().CopyTo(bytes, index);
                    index += frameMessage.GetMsgLength();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

        /// <summary>
        /// ��ȡbyte����
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static  byte ReadByte(byte[] bytes, ref int index)
        {
            var byteValue = bytes[index];
            index += 1;
            return byteValue;
        }

        /// <summary>
        /// ��ȡshort����
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static  short ReadShort(byte[] bytes, ref int index)
        {
            var shortValue = BitConverter.ToInt16(bytes, index);
            index += 2;
            return shortValue;
        }

        /// <summary>
        /// ��ȡint����
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static  int ReadInt(byte[] bytes, ref int index)
        {
            var intValue = BitConverter.ToInt32(bytes, index);
            index += 4;
            return intValue;
        }

        /// <summary>
        /// ��ȡlong����
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static  long ReadLong(byte[] bytes, ref int index)
        {
            var longValue = BitConverter.ToInt64(bytes, index);
            index += 8;
            return longValue;
        }

        /// <summary>
        /// ��ȡfloat����
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static  float Readfloat(byte[] bytes, ref int index)
        {
            var floatValue = BitConverter.ToSingle(bytes, index);
            index += 4;
            return floatValue;
        }

        /// <summary>
        /// ��ȡdouble����
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static  double ReadDouble(byte[] bytes, ref int index)
        {
            var doubleValue = BitConverter.ToDouble(bytes, index);
            index += 8;
            return doubleValue;
        }

        /// <summary>
        /// ��ȡchar����
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static  char ReadChar(byte[] bytes, ref int index)
        {
            var charValue = BitConverter.ToChar(bytes, index);
            index += 2;
            return charValue;
        }

        /// <summary>
        /// ��ȡbool����
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static  bool ReadBool(byte[] bytes, ref int index)
        {
            var boolValue = BitConverter.ToBoolean(bytes, index);
            index += 1;
            return boolValue;
        }

        /// <summary>
        /// ��ȡstring����
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static  string ReadString(byte[] bytes, ref int index)
        {
            var length = ReadInt(bytes, ref index);
            var strValue = Encoding.UTF8.GetString(bytes, index, length);
            index += length;
            return strValue;
        }

        /// <summary>
        /// ��ȡFrameCommand����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T ReadFrameMessage<T>(byte[] bytes, ref int index) where T : FrameMessage, new()
        {
            var frameCommand = new T();
            index += frameCommand.Deserialize(bytes, index);
            return frameCommand;
        }
    }
}
