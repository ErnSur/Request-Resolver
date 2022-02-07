using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace QuickEye.ReferenceValidator
{
    public static class SerializedPropertyUtility
    {
        public static FieldInfo GetFieldInfo(this SerializedProperty property)
        {
            var parentType = property.serializedObject.targetObject.GetType();
            return GetFieldInfo(parentType, property.propertyPath);
        }

        public static FieldInfo GetFieldInfo(Type serializedObjectType, string path)
        {
            var pathSegments = path.Split('.');
            var fieldInfo = GetSerializedField(serializedObjectType, pathSegments[0]);
            for (var i = 1; i < pathSegments.Length; i++)
            {
                if (pathSegments[i] == "Array")
                {
                    i += 2;
                    if (i >= pathSegments.Length)
                        continue;

                    var type = fieldInfo.FieldType.IsArray
                        ? fieldInfo.FieldType.GetElementType()
                        : fieldInfo.FieldType.GetGenericArguments()[0];

                    fieldInfo = GetSerializedField(type, pathSegments[i]);
                }
                else
                {
                    if (i + 1 < pathSegments.Length && pathSegments[i + 1] == "Array")
                    {
                        fieldInfo = GetSerializedField(serializedObjectType, pathSegments[i]);
                    }
                    else
                    {
                        fieldInfo = GetSerializedField(fieldInfo.FieldType, pathSegments[i]);
                    }
                    // fieldInfo = i + 1 < splits.Length && splits[i + 1] == "Array"
                    //     ? GetField(parentType, splits[i])
                    //     : GetField(fieldInfo.FieldType, splits[i]);
                }

                if (fieldInfo == null)
                    throw new Exception("Invalid FieldInfo. ");

                serializedObjectType = fieldInfo.FieldType;
            }

            return fieldInfo;

            FieldInfo GetSerializedField(Type t, string fieldName)
            {
                var field = t.GetField(fieldName,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                if (field == null && t.BaseType != null)
                    return GetSerializedField(t.BaseType, fieldName);
                return field;
            }
        }

        /// <summary>
        /// SerializedProperty から Field の Type を取得する 
        /// </summary>
        /// <param name="property">SerializedProperty</param>
        /// <param name="isArrayListType">array や List の場合要素の Type を取得するか</param>
        public static Type GetPropertyType(this SerializedProperty property, bool isArrayListType = false)
        {
            var fieldInfo = property.GetFieldInfo();

            // 配列の場合は配列のTypeを返す
            if (isArrayListType && property.isArray && property.propertyType != SerializedPropertyType.String)
                return fieldInfo.FieldType.IsArray
                    ? fieldInfo.FieldType.GetElementType()
                    : fieldInfo.FieldType.GetGenericArguments()[0];
            return fieldInfo.FieldType;
        }
    }
}