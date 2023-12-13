using UnityEditor;
using UnityEngine;

namespace VoidexSoft.Inventory.Editor
{
    [CustomPropertyDrawer(typeof(IconPreviewAttribute))]
    public class SpritePropertyDrawer : PropertyDrawer
    {
        private const float ICON_SIZE = 55f;

        public override float GetPropertyHeight(SerializedProperty p, GUIContent label)
        {
            return p.objectReferenceValue != null 
                ? ICON_SIZE 
                : base.GetPropertyHeight(p, label);
        }

        public override void OnGUI(Rect pos, SerializedProperty p, GUIContent label)
        {
            EditorGUI.BeginProperty(pos, label, p);

            if (p.objectReferenceValue != null)
            {
                pos.width = EditorGUIUtility.labelWidth;
                GUI.Label(pos, p.displayName);
                pos.x += pos.width;
                pos.width = ICON_SIZE;
                pos.height = ICON_SIZE;
                p.objectReferenceValue = EditorGUI.ObjectField(pos, p.objectReferenceValue, typeof(Sprite), false);
            }
            else
            {
                GUI.Label(pos, p.displayName);
                EditorGUI.PropertyField(pos, p, true);
            }
            EditorGUI.EndProperty();
        }
    }
}