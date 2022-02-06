using System.Linq;
using QuickEye.UIToolkit;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.ReferenceValidator
{
    public class ReferenceResolverWindow : EditorWindow
    {
        [MenuItem("Tools/Reference Helper")]
        public static void Open()
        {
            GetWindow<ReferenceResolverWindow>();
        }

        [Q("top--toolbar")]
        private Toolbar topToolbar;

        [Q("apply--button")]
        private ToolbarButton applyButton;

        [Q("revert--button")]
        private ToolbarButton revertButton;

        [Q("set--button")]
        private ToolbarButton setButton;

        [Q("query--field")]
        private ToolbarSearchField queryField;


        [Q("gameObject--list")]
        private ListView gameObjectList;

        [Q("prop--list")]
        private ListView propList;

        [Q("ComponentBrowser")]
        private QuickEye.ReferenceValidator.ComponentBrowser componentBrowser;

        [SerializeField]
        private string query;

        [SerializeField]
        private GameObjectList goList = new GameObjectList();

        private void CreateGUI()
        {
            PackageResources.TryLoadTree<ReferenceResolverWindow>(out var tree);
            tree.CloneTree(rootVisualElement);
            rootVisualElement.AssignQueryResults(this);
            rootVisualElement.AssignQueryResults(goList);
            InitView();
            queryField.value = query;

            applyButton.clicked += InitView;
            revertButton.clicked += goList.Clear;
            // setButton.clicked += () =>
            // {
            //     Debug.Log($"hello: {debugIndex}");
            //     propList.selectedIndex = debugIndex;
            //     propList.ScrollToItem(debugIndex);
            // };
            rootVisualElement.RegisterCallback<AttachToPanelEvent>(evt =>
            {
                // var field = new PropertyField()
                // topToolbar.Add(field);
            });
        }

        private void RefreshPropList()
        {
            var go = goList.Selected;
            if (!go)
                return;
            var query = queryField.value;
            var props = string.IsNullOrEmpty(query)
                ? GameObjectAnalyzer.GetNullProperties(go)
                : QueryInterpreter.GetProps(go, query).ToArray();
            propList.itemsSource = props;
            propList.Rebuild();
        }

        private void InitView()
        {
            goList.SelectionChanged += go => { RefreshPropList(); };
            goList.InitList();

            queryField.RegisterValueChangedCallback(evt =>
            {
                query = evt.newValue;
                RefreshPropList();
            });

            propList.makeItem = () => new PropertyField();
            propList.bindItem = (element, i) =>
            {
                var field = element as PropertyField;
                field.BindProperty(propList.itemsSource[i] as SerializedProperty);
            };

            propList.RegisterToOnSelectionChange( list =>
            {
                propList.ScrollToItem(propList.selectedIndex);
                var selectedProp = propList.selectedItem as SerializedProperty;
                var target = selectedProp.serializedObject.targetObject as Component;
                var root = target.gameObject;
                Selection.activeObject = root;
                if (typeof(Object).IsAssignableFrom(selectedProp.GetPropertyType()))
                {
                    var children = root.GetComponentsInChildren(selectedProp.GetPropertyType(), true);
                    componentBrowser.Setup(children);
                }
            });

            componentBrowser.ItemChosen += o =>
            {
                Debug.Log($"Componen CHosen: {o.name}");
                var selectedProp = propList.selectedItem as SerializedProperty;
                selectedProp.objectReferenceValue = o;
                selectedProp.serializedObject.ApplyModifiedProperties();
                if (propList.selectedIndex < propList.itemsSource.Count - 1)
                    propList.selectedIndex++;
                else
                {
                    //GameObjectList intex++
                }

                var nextProp = propList.selectedItem as SerializedProperty;
                var message = $"Next:\n{nextProp.displayName}";
                // It would be better to draw a small gui label in the corner of the scene view
                SceneView.lastActiveSceneView.ShowNotification(new GUIContent(message));
            };
        }
    }
}