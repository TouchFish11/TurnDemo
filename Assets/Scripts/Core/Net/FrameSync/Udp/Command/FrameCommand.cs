using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Net.FrameSync.Udp.Command
{
    /// <summary>
    /// ָ֡��
    /// </summary>
    public abstract class FrameCommand
    {
        /// <summary>
        /// ��ȡ�ֽ����鳤��
        /// </summary>
        /// <returns></returns>
        public abstract int GetBytesLength();

        /// <summary>
        /// ���л�
        /// </summary>
        /// <returns></returns>
        public abstract byte[] Serialize();

        /// <summary>
        /// �����л�
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="beginIndex"></param>
        /// <returns></returns>
        public abstract int Deserialize(byte[] bytes, int beginIndex = 0);

        /// <summary>
        /// д���ֶ�
        /// </summary>
        /// <typeparam name="T">֧��byte��shout��int��long��float��double��char��bool��string��BaseMessage</typeparam>
        /// <param name="bytes">�ֽ�����</param>
        /// <param name="value">�ֶ�ֵ</param>
        /// <param name="index">��ǰд��λ��</param>
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
                case FrameCommand frameCommand:
                    frameCommand.Serialize().CopyTo(bytes, index);
                    index += frameCommand.GetBytesLength();
                    break;
                case List<ClientFrameCommand> clientFrameCommands:
                    WriteField<int>(bytes, clientFrameCommands.Count, ref index);
                    for (int i = 0; i < clientFrameCommands.Count; i++)
                    {
                        WriteField<ClientFrameCommand>(bytes, clientFrameCommands[i], ref index);
                    }
                    break;
                case List<OneFrameCommand> oneFrameCommands:
                    WriteField<int>(bytes, oneFrameCommands.Count, ref index);
                    for (int i = 0; i < oneFrameCommands.Count; i++)
                    {
                        WriteField<OneFrameCommand>(bytes, oneFrameCommands[i], ref index);
                    }
                    break;
                default:
                    Console.WriteLine($"���л���Ϣ�ֶ�ʧ�ܣ�δ��������͵����л��߼���{typeof(T)}");
                    break;
            }
        }

        /// <summary>
        /// ��ȡbyte����
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
        /// ��ȡshort����
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
        /// ��ȡint����
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
        /// ��ȡlong����
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
        /// ��ȡfloat����
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
        /// ��ȡdouble����
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
        /// ��ȡchar����
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
        /// ��ȡbool����
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
        /// ��ȡstring����
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
        /// ��ȡFrameCommand����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected T ReadFrameCommand<T>(byte[] bytes, ref int index) where T : FrameCommand, new()
        {
            T frameCommand = new T();
            index += frameCommand.Deserialize(bytes, index);
            return frameCommand;
        }

        /// <summary>
        /// ��ȡָ��List����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected List<T> ReadCommandList<T>(byte[] bytes, ref int index) where T : FrameCommand, new()
        {
            List<T> list = new List<T>();
            int count = ReadInt(bytes, ref index);
            for (int i = 0; i < count; i++)
            {
                list.Add(ReadFrameCommand<T>(bytes, ref index));
            }
            return list;
        }
    }
}
