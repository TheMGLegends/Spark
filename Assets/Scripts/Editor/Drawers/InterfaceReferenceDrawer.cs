using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InterfaceReference<>), true)]
public class InterfaceReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty underlyingObjectProp = property.FindPropertyRelative("underlyingObject");

        if (underlyingObjectProp != null)
        {
            EditorGUI.BeginProperty(position, label, property);

            underlyingObjectProp.objectReferenceValue = EditorGUI.ObjectField(
                position,
                label,
                underlyingObjectProp.objectReferenceValue,
                typeof(UnityEngine.Object),
                true
            );

            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}
