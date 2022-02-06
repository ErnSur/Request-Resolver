using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QuickEye.ReferenceValidator
{
    [Serializable]
    public class SelectablePropertyFieldValue
    {
        public int Integer;
        public bool Boolean;
        public float Float;
        public string String;
        public Color Color;
        public Object ObjectReference;
        public LayerMask LayerMask;
        public Vector2 Vector2;
        public Vector2Int Vector2Int;
        public Vector3 Vector3;
        public Vector3Int Vector3Int;
        public Vector4 Vector4;
        public Rect Rect;
        public RectInt RectInt;
        public char Character;
        public AnimationCurve AnimationCurve;
        public Bounds Bounds;
        public BoundsInt BoundsInt;
        public Gradient Gradient;
        public Quaternion Quaternion;

        public object GetValue(SelectablePropType type)
        {
            switch (type)
            {
                case SelectablePropType.Integer:
                    return Integer;
                    break;
                case SelectablePropType.Boolean:
                    return Boolean;
                    break;
                case SelectablePropType.Float:
                    break;
                case SelectablePropType.String:
                    break;
                case SelectablePropType.Color:
                    break;
                case SelectablePropType.ObjectReference:
                    break;
                case SelectablePropType.LayerMask:
                    break;
                case SelectablePropType.Enum:
                    break;
                case SelectablePropType.Vector2:
                    break;
                case SelectablePropType.Vector3:
                    break;
                case SelectablePropType.Vector4:
                    break;
                case SelectablePropType.Rect:
                    break;
                case SelectablePropType.ArraySize:
                    break;
                case SelectablePropType.Character:
                    break;
                case SelectablePropType.AnimationCurve:
                    break;
                case SelectablePropType.Bounds:
                    break;
                case SelectablePropType.Gradient:
                    break;
                case SelectablePropType.Quaternion:
                    break;
                case SelectablePropType.Vector2Int:
                    break;
                case SelectablePropType.Vector3Int:
                    break;
                case SelectablePropType.RectInt:
                    break;
                case SelectablePropType.BoundsInt:
                    break;
                case SelectablePropType.Hash128:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return default;
        }
    }
}