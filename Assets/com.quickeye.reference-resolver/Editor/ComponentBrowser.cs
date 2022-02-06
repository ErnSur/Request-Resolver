using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using QuickEye.UIToolkit;
using Object = UnityEngine.Object;

namespace QuickEye.ReferenceValidator
{
    // On Item GUI:
    // if component is text element draw its text value
    // if it is image draw its texture/sprite
    // OR maybe just draw component editor as sort of AssetPreview at the bottom of the listview
    internal class ComponentBrowser : VisualElement
    {
        public event Action<Object> ItemChosen;

        [Q("children--tab")]
        private QuickEye.ReferenceValidator.Tab childrenTab;

        [Q("self--tab")]
        private QuickEye.ReferenceValidator.Tab selfTab;

        [Q("ancestors--tab")]
        private QuickEye.ReferenceValidator.Tab ancestorsTab;

        [Q("children--list")]
        private ListView childrenList;

        [Q("self--list")]
        private ListView selfList;

        [Q("ancestors--list")]
        private ListView ancestorsList;

        public ComponentBrowser()
        {
            this.InitResources();
            childrenList.RegisterToOnItemChosen(o => ItemChosen?.Invoke((Object)o));
            childrenList.RegisterToOnSelectionChange(list =>
            {
                Selection.activeObject = list.First() as Object;
                SceneView.FrameLastActiveSceneView();
            });
        }

        public void Setup(Component[] components)
        {
            SetupList(childrenList, components);
        }

        private void SetupList(ListView list, Component[] components)
        {
            list.itemsSource = components;
            list.makeItem = () => new IMGUIContainer(null);
            list.bindItem = (element, i) =>
            {
                var drawer = element as IMGUIContainer;
                drawer.onGUIHandler = () =>
                {
                    EditorGUIUtility.SetIconSize(new Vector2(30, 30));
                    var e = list.itemsSource[i];
                    GUILayout.Label(EditorGUIUtility.ObjectContent(e as Object, e.GetType()));
                };
            };

            list.Rebuild();
        }

        public class UxmlFactory : UxmlFactory<ComponentBrowser>
        {
        }
    }
}