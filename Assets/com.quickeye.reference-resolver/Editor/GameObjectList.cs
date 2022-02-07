using System;
using System.Collections.Generic;
using System.Linq;
using QuickEye.UIToolkit;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.ReferenceValidator
{
    // Add option to multiselect and delete all items with right clic context menu or delete button
    [Serializable]
    public class GameObjectList
    {
        public event Action<GameObject> SelectionChanged;

        [Q("gameObject--list")]
        private ListView list;

        public List<GameObject> data = new List<GameObject>();
        private DnDManipulator dragManipulator;

        public GameObject Selected => list.selectedItem as GameObject;

        private void InitDrag()
        {
            dragManipulator = new DnDManipulator("", "go--list");
            dragManipulator.DragPerformed += OnDragPerformed;
            list.AddManipulator(dragManipulator);
        }

        private void OnDragPerformed()
        {
            data.AddRange(DragAndDrop.objectReferences.OfType<GameObject>().ToList());
            list.Rebuild();
        }

        public void Clear()
        {
            data.Clear();
            list.Rebuild();
        }

        public void InitList()
        {
            InitDrag();
            list.itemsSource = data;
            list.makeItem = () => new IMGUIContainer(null);
            list.bindItem = (element, i) =>
            {
                var drawer = element as IMGUIContainer;
                drawer.onGUIHandler = () =>
                {
                    EditorGUIUtility.SetIconSize(new Vector2(30, 30));
                    GUILayout.Label(EditorGUIUtility.ObjectContent(data[i], typeof(GameObject)));
                };
            };
            list.RegisterToOnSelectionChange(objects => { SelectionChanged?.Invoke(list.selectedItem as GameObject); });
            list.Rebuild();
        }
    }
}