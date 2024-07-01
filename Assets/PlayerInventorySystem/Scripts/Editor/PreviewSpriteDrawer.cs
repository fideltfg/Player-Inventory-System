using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Classes and methods to display ItemCatalog correctly in the unity editor.
/// </summary>
namespace PlayerInventorySystem.Editor
{
    /// <summary>
    /// Custom editor script to display sprite thumbnails in the item catalog
    /// </summary>
    [CustomPropertyDrawer(typeof(PlayerInventorySystem.PreviewSpriteAttribute))]
    public class PreviewSpriteDrawer : PropertyDrawer
    {
        const float cellWidth = 50;
        const float cellHeight = 50;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference &&
                (property.objectReferenceValue as Sprite) != null)
            {
                return EditorGUI.GetPropertyHeight(property, label, true) + cellHeight + 10;
            }
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        static string GetPath(SerializedProperty property)
        {
            string path = property.propertyPath;
            int index = path.LastIndexOf(".");
            return path.Substring(0, index + 1);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Draw the normal property field
            EditorGUI.PropertyField(position, property, label, true);

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                var sprite = property.objectReferenceValue as Sprite;
                if (sprite != null)
                {
                    position.x += EditorGUIUtility.labelWidth + 4;
                    position.y += EditorGUI.GetPropertyHeight(property, label, true) + 5;
                    position.height = cellHeight;
                    position.width = 50;

                    // Draw the cell background color
                    EditorGUI.DrawRect(position, new Color(0.31f, 0.31f, 0.31f, 1.0f));

                    // Draw the cell border
                    DrawCellBorder(position);

                    // Draw the sprite texture preview
                    DrawTexturePreview(position, sprite);

                    // Handle mouse click event to open the default sprite picker
                    if (Event.current.type == EventType.MouseDown && position.Contains(Event.current.mousePosition))
                    {
                        EditorGUIUtility.ShowObjectPicker<Sprite>(sprite, false, "", property.propertyPath.GetHashCode());
                        Event.current.Use();
                    }

                    // Handle object picker selection
                    if (Event.current.commandName == "ObjectSelectorUpdated" &&
                        EditorGUIUtility.GetObjectPickerControlID() == property.propertyPath.GetHashCode())
                    {
                        property.objectReferenceValue = EditorGUIUtility.GetObjectPickerObject() as Sprite;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }

        private void DrawCellBorder(Rect cellRect)
        {
            Handles.color = Color.black;
            Handles.DrawLine(new Vector3(cellRect.x - 1, cellRect.y - 1), new Vector3(cellRect.x + cellRect.width + 1, cellRect.y - 1));
            Handles.DrawLine(new Vector3(cellRect.x - 1, cellRect.y - 1), new Vector3(cellRect.x - 1, cellRect.y + cellRect.height + 1));
            Handles.DrawLine(new Vector3(cellRect.x + cellRect.width + 1, cellRect.y - 1), new Vector3(cellRect.x + cellRect.width + 1, cellRect.y + cellRect.height + 1));
            Handles.DrawLine(new Vector3(cellRect.x - 1, cellRect.y + cellRect.height + 1), new Vector3(cellRect.x + cellRect.width + 1, cellRect.y + cellRect.height + 1));
        }

        private void DrawTexturePreview(Rect position, Sprite sprite)
        {
            Vector2 fullSize = new Vector2(sprite.texture.width, sprite.texture.height);
            Vector2 size = new Vector2(sprite.textureRect.width, sprite.textureRect.height);

            Rect coords = sprite.textureRect;
            coords.x /= fullSize.x;
            coords.width /= fullSize.x;
            coords.y /= fullSize.y;
            coords.height /= fullSize.y;

            Vector2 ratio;
            ratio.x = position.width / size.x;
            ratio.y = position.height / size.y;
            float minRatio = Mathf.Min(ratio.x, ratio.y);

            Vector2 center = position.center;
            position.width = size.x * minRatio;
            position.height = size.y * minRatio;
            position.center = center;

            GUI.DrawTextureWithTexCoords(position, sprite.texture, coords);
        }
    }
}
