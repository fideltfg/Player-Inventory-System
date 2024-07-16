using UnityEditor;
using UnityEngine;
using PlayerInventorySystem;

[CustomEditor(typeof(SO_ItemCatalog))]
public class SO_ItemListEditor : Editor
{
    private SerializedProperty catalogID;
    private SerializedProperty list;
    private SO_ItemCatalog itemList;

    private void OnEnable()
    {
        // Ensure the target is correctly assigned
        if (target != null)
        {
            itemList = (SO_ItemCatalog)target;
            catalogID = serializedObject.FindProperty("catalogID");
            list = serializedObject.FindProperty("list");
        }
    }

    public override void OnInspectorGUI()
    {
        // Ensure serializedObject is properly updated
        serializedObject.Update();

        // Display the catalogID property field
        EditorGUILayout.PropertyField(catalogID);

        // Display the list of items
        EditorGUILayout.LabelField("Items", EditorStyles.boldLabel);

        if (list != null)
        {
            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty itemData = list.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(itemData);
            }

            // Add a button to add new items to the list
            if (GUILayout.Button("Add Item"))
            {
                list.arraySize++;
                SerializedProperty newItem = list.GetArrayElementAtIndex(list.arraySize - 1);
                newItem.FindPropertyRelative("name").stringValue = "New Item";

                // Find the maximum ID in the existing items
                int maxID = 0;
                for (int i = 0; i < list.arraySize - 1; i++)
                {
                    SerializedProperty currentItem = list.GetArrayElementAtIndex(i);
                    int currentID = currentItem.FindPropertyRelative("id").intValue;
                    if (currentID > maxID)
                    {
                        maxID = currentID;
                    }
                }

                // Set the new item's ID to maxID + 1
                newItem.FindPropertyRelative("id").intValue = maxID + 1;
            }
        }
        else
        {
            EditorGUILayout.HelpBox("List property is null. Check if the property name is correct.", MessageType.Error);
        }

        // Apply modified properties
        serializedObject.ApplyModifiedProperties();
    }
}
