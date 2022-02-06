using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.ReferenceValidator
{
    public class DnDManipulator : Manipulator
    {
        public string dragOverClass;
        public string DragGenericDataType = nameof(DnDManipulator);
        public event Action DragPerformed;

        public DnDManipulator(string dragGenericDataName, string dragOverClassName)
        {
            dragOverClass = dragOverClassName;
            DragGenericDataType = dragGenericDataName;
        }

        private void OnDragExitedEvent(DragExitedEvent evt)
        {
            target.RemoveFromClassList(dragOverClass);
        }

        private void OnDragPerformEvent(DragPerformEvent evt)
        {
            DragAndDrop.AcceptDrag();
            // Get data like:
            // DragAndDrop.objectReferences
            // DragAndDrop.GetGenericData(DragGenericDataType)
            DragPerformed?.Invoke();
        }

        private void OnDragUpdatedEvent(DragUpdatedEvent evt)
        {
            target.EnableInClassList(dragOverClass, true);

            var draggedLabel = DragAndDrop.GetGenericData(DragGenericDataType);
            if (draggedLabel != null)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
        }

        private void OnDragLeaveEvent(DragLeaveEvent evt)
        {
            target.RemoveFromClassList(dragOverClass);
        }

        private void OnDragEnterEvent(DragEnterEvent evt)
        {
            target.AddToClassList(dragOverClass);
        }

        void OnAttach(AttachToPanelEvent e)
        {
            e.destinationPanel.visualTree.RegisterCallback<DragExitedEvent>(OnDragExitedEvent);
        }

        void OnDetach(DetachFromPanelEvent e)
        {
            e.originPanel.visualTree.UnregisterCallback<DragExitedEvent>(OnDragExitedEvent);
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<DragEnterEvent>(OnDragEnterEvent);
            target.RegisterCallback<DragLeaveEvent>(OnDragLeaveEvent);
            target.RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
            target.RegisterCallback<DragPerformEvent>(OnDragPerformEvent);
            target.RegisterCallback<DragExitedEvent>(OnDragExitedEvent);

            // If the mouse move quickly, DragExitedEvent will only be sent to panel.visualTree.
            // Register a callback there to get notified.
            target.panel?.visualTree.RegisterCallback<DragExitedEvent>(OnDragExitedEvent);

            // When opening the window, root.panel is not set yet. Use these callbacks to make
            // sure we register a DragExitedEvent callback on root.panel.visualTree.
            target.RegisterCallback<AttachToPanelEvent>(OnAttach);
            target.RegisterCallback<DetachFromPanelEvent>(OnDetach);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<DragEnterEvent>(OnDragEnterEvent);
            target.UnregisterCallback<DragLeaveEvent>(OnDragLeaveEvent);
            target.UnregisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
            target.UnregisterCallback<DragPerformEvent>(OnDragPerformEvent);
            target.UnregisterCallback<DragExitedEvent>(OnDragExitedEvent);
        }
    }
}