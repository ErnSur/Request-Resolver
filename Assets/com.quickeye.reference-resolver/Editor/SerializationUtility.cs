using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QuickEye.ReferenceValidator
{
    internal static class SerializationUtility
    {
        private const string ContextActionName = "CONTEXT/Component/Suggest References";

        public static SerializedProperty[] GetComponentSerializedFields(Object target)
        {
            var serObj = new SerializedObject(target);
            var prop = serObj.GetIterator();
            var result = new List<SerializedProperty>();
            while (prop.NextVisible(true))
            {
                if (prop.name == "m_Script")
                    continue;
                result.Add(prop.Copy());
            }
            return result.ToArray();
        }

        private static bool IsObjectProp(SerializedProperty prop) => prop.name != "m_Script"
                                                                     && prop.propertyType == SerializedPropertyType.ObjectReference;

        [MenuItem(ContextActionName)]
        private static void ContextAction(MenuCommand command)
        {
            var props = GetComponentSerializedFields(command.context);
        }
    }
}