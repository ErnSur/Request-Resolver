using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QuickEye.ReferenceValidator
{
    internal static class GameObjectAnalyzer
    {
        /*public static SerializedProperty[] GetNullProperties(GameObject gameObject)
        {
            var references = GetPrefabReferences(gameObject);
            var prefabsOutsideSdkFolder = (from @ref in references
                let assetPath = AssetDatabase.GetAssetPath(@ref.prefabAsset)
                where !assetPath.StartsWith(validPrefabDirectory)
                select @ref).ToArray();

            var dic = new Dictionary<GameObject, HashSet<string>>();
            foreach (var @ref in prefabsOutsideSdkFolder)
            {
                if (!dic.TryGetValue(@ref.prefabAsset, out var paths))
                {
                    dic[@ref.prefabAsset] = paths = new HashSet<string>();
                }

                paths.Add(GetComponentPath(@ref.component));
            }

            foreach (var kvp in dic)
            {
                var paths = new StringBuilder();
                foreach (var path in kvp.Value)
                {
                    paths.AppendLine(path);
                }

                Debug.LogError(
                    $"Referenced prefab outside of the SDK folder: \"{kvp.Key.name}\" in {gameObject.name}:\n{paths}",
                    kvp.Key);
            }
        }*/

        private static string GetComponentPath(Component component)
        {
            var go = component.gameObject;
            var path = $"/{go.name}";
            while (go.transform.parent != null)
            {
                go = go.transform.parent.gameObject;
                path = $"/{go.name}{path}";
            }

            path = $"{go.scene.name}{path}";
            path += $"/<color='red'>{component.name}</color>";

            return path;
        }

        public static SerializedProperty[] GetNullProperties(GameObject gameObject)
        {
            var components = gameObject.GetComponentsInChildren<MonoBehaviour>(true);

            var filteredComponents = from component in components
                let type = component.GetType()
                where type.Namespace != null && !type.Namespace.StartsWith("Unity")
                select component;

            return filteredComponents.SelectMany(GetNullProperties).ToArray();
        }
        
        public static IEnumerable<SerializedProperty> GetProperties(GameObject gameObject)
        {
            var components = gameObject.GetComponentsInChildren<MonoBehaviour>(true);

            var filteredComponents = from component in components
                let type = component.GetType()
                where type.Namespace != null && !type.Namespace.StartsWith("Unity")
                select component;

            return filteredComponents.SelectMany(SerializationUtility.GetComponentSerializedFields);
        }

        private static SerializedProperty[] GetNullProperties(Component comp)
        {
            var props = SerializationUtility.GetComponentSerializedFields(comp);
            return props
                .Where(p =>
                    p.propertyType == SerializedPropertyType.ObjectReference
                    && p.objectReferenceValue == null)
                .ToArray();
        }

        private static bool IsPrefab(GameObject gameObject)
        {
            return gameObject != null
                   && PrefabUtility.IsPartOfPrefabAsset(gameObject)
                   && gameObject.transform.root.gameObject == gameObject;
        }

        private static GameObject ToGameObject(Object obj)
        {
            switch (obj)
            {
                case Component component:
                    return component.gameObject;
                case GameObject go:
                    return go;
                default:
                    return null;
            }
        }
    }
}