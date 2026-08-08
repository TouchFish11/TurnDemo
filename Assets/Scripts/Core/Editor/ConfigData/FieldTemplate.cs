using System;

namespace Core.Editor.ConfigData
{
    /// <summary>
    /// �ֶ�ģ��
    /// </summary>
    [Serializable]
    public class FieldTemplate
    {
        public E_FieldType fieldType; // �ֶ�����
        public string fieldName; // �ֶ�������taskId��
        public string fieldDescription;    // �ֶ�����
        public bool key;    // �Ƿ�Ϊ����

        public FieldTemplate(string fieldName, E_FieldType fieldType)
        {
            this.fieldName = fieldName;
            this.fieldType = fieldType;
        }
    }
}
