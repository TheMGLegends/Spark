using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DisplayNameAttribute))]
public class DisplayNameDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DisplayNameAttribute displayNameAttribute = attribute as DisplayNameAttribute;
        label.text = displayNameAttribute.DisplayName;
        EditorGUI.PropertyField(position, property, label, true);
    }
}
