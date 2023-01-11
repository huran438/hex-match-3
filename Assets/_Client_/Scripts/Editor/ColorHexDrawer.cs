using _Client_.Scripts.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace _Client_.Scripts.Editor
{
    [CustomPropertyDrawer(typeof(ColorHexAttribute))]
    public class ColorHexDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var value = property.stringValue;

            if (ColorUtility.TryParseHtmlString(value, out var color))
            {
                EditorGUI.BeginChangeCheck();
                color = EditorGUI.ColorField(position, label, color);
                if (EditorGUI.EndChangeCheck())
                {
                    value = "#" + ColorUtility.ToHtmlStringRGB(color);
                }
            }
            else
            {
                value = "#" + ColorUtility.ToHtmlStringRGB(Color.white);
            }

            property.stringValue = value;

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}