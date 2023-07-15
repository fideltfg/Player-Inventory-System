using UnityEngine;
using UnityEditor;
using System.Collections;

namespace PlayerInventorySystem.Editor
{

    /// <summary>
    /// Custom editor class to display the Recipe grid in the Item Catalog.
    /// </summary>
    [CustomPropertyDrawer(typeof(PlayerInventorySystem.Recipe))]
    public class CustomTileData : PropertyDrawer
    {
        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PrefixLabel(position, label);

            Rect newPosition = position;
            newPosition.y += 18f;
            SerializedProperty rows = property.FindPropertyRelative("rows");

            for (int i = 0; i < 3; i++)
            {
                SerializedProperty row = rows.GetArrayElementAtIndex(i).FindPropertyRelative("cells");
                newPosition.height = 20;

                if (row.arraySize != 3)
                    row.arraySize = 3;

                newPosition.width = 70;

                for (int j = 0; j < 3; j++)
                {
                    EditorGUI.PropertyField(newPosition, row.GetArrayElementAtIndex(j), GUIContent.none);
                    newPosition.x += newPosition.width;
                }

                newPosition.x = position.x;
                newPosition.y += 20;
            }
        }

        public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
        {
            return 80;
        }
    }
}
