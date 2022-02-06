using QuickEye.UIToolkit;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.ReferenceValidator
{
    internal static class PackageResources
    {
        private const string BaseResourcesPath = "Assets/com.quickeye.reference-resolver/Editor/Resources/";
        private const string BaseDir = "com.quickeye.reference-resolver/";
        private const string CommonStyle = BaseDir + "Common";

        // In Unity 2019 Loading USS files form resources for some reason created unpredictable results
        // try returning Resources load here to compare 
        private static T LoadAsset<T>(string resourcesRelativePath) where T : Object
        {
            return Resources.Load<T>(resourcesRelativePath);
            var extension = typeof(T) == typeof(StyleSheet) ? ".uss" : ".uxml";
            return AssetDatabase.LoadAssetAtPath<T>(
                $"{BaseResourcesPath}{resourcesRelativePath}{extension}");
        }

        public static bool TryLoadTree<T>(out VisualTreeAsset tree)
        {
            tree = LoadAsset<VisualTreeAsset>(BaseDir + typeof(T).Name);
            return tree != null;
        }

        private static bool TryLoadThemeStyle<T>(out StyleSheet styleSheet) where T : VisualElement
        {
            var styleSuffix = EditorGUIUtility.isProSkin ? "Dark" : "Light";
            styleSheet = LoadAsset<StyleSheet>($"{BaseDir}{typeof(T).Name}-{styleSuffix}");
            return styleSheet != null;
        }

        private static bool TryLoadStyle<T>(out StyleSheet styleSheet) where T : VisualElement
        {
            styleSheet = LoadAsset<StyleSheet>($"{BaseDir}{typeof(T).Name}");
            if (typeof(T) == typeof(Tab) && styleSheet == null)
                Debug.Log($"TryLoad Style: {typeof(T).Name} / is null = {styleSheet == null}");
            return styleSheet != null;
        }

        public static void InitResources<T>(this T ve) where T : VisualElement
        {
            if (TryLoadTree<T>(out var tree))
            {
                tree.CloneTree(ve);
                ve.AssignQueryResults(ve);
            }

            var commonStyle = Resources.Load<StyleSheet>(CommonStyle);
            if (commonStyle)
                ve.styleSheets.Add(commonStyle);

            if (TryLoadThemeStyle<T>(out var themeStyleSheet))
                ve.styleSheets.Add(themeStyleSheet);

            if (TryLoadStyle<T>(out var styleSheet))
                ve.styleSheets.Add(styleSheet);
        }
    }
}