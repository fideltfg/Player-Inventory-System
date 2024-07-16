using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PlayerInventorySystem.Editor
{
    [CustomPropertyDrawer(typeof(Recipe))]
    public class RecipeDrawer : PropertyDrawer
    {
        private SO_ItemCatalog itemList;
        private List<GUIContent> itemContents;
        private SerializedProperty currentProperty;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(position, label);
            position.y += EditorGUIUtility.singleLineHeight;

            EnsureItemListLoaded();
            if (itemList == null)
            {
                ShowItemListError(position);
                return;
            }

            EnsureItemContentsLoaded();

            SerializedProperty rows = property.FindPropertyRelative("rows");
            EnsureRowsInitialized(rows);

            // Add message above the recipe grid
            Rect messageRect = new Rect(position.x + EditorGUIUtility.labelWidth - 20, position.y - EditorGUIUtility.singleLineHeight, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(messageRect, "Click to select or Shift+Click to clear");

            DrawGrid(position, rows);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 11; // Increased height to account for the additional label
        }

        private void EnsureItemListLoaded()
        {
            if (itemList == null)
            {
                GameObject inventoryController = GameObject.FindGameObjectWithTag("InventoryController");
                if (inventoryController != null)
                {
                    InventoryController controller = inventoryController.GetComponent<InventoryController>();
                    if (controller != null)
                    {
                        itemList = controller.ItemCatalog;
                    }
                }
                else
                {
                    Debug.LogWarning("InventoryController not found in the scene.");
                }
            }
        }

        private void ShowItemListError(Rect position)
        {
            EditorGUI.HelpBox(position, "Item list not found. Ensue you have placed this item list in your inventory controller.", MessageType.Error);
        }

        private void EnsureItemContentsLoaded()
        {
            if (itemList != null && itemContents == null)
            {
                itemContents = new List<GUIContent>();
                foreach (var item in itemList.list)
                {
                    string contentText = $"{item.name} : {item.id}";
                    itemContents.Add(new GUIContent(contentText));
                }
            }
        }

        private void EnsureRowsInitialized(SerializedProperty rows)
        {
            if (rows.arraySize != 3)
            {
                rows.arraySize = 3;
                for (int i = 0; i < 3; i++)
                {
                    SerializedProperty row = rows.GetArrayElementAtIndex(i).FindPropertyRelative("cells");
                    if (row.arraySize != 3)
                    {
                        row.arraySize = 3;
                        for (int j = 0; j < 3; j++)
                        {
                            row.GetArrayElementAtIndex(j).intValue = 0; // Default value 0 for empty cell
                        }
                    }
                }
            }
        }

        private void DrawGrid(Rect position, SerializedProperty rows)
        {
            float cellWidth = 50f;
            float cellHeight = 50f;
            float marginx = 5f;
            float marginy = 5f;
            float totalWidth = (cellWidth + marginx * 2) * 3;
            float totalHeight = (cellHeight * 3.5f) - 5;

            Rect gridRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, totalWidth, totalHeight);
            EditorGUI.DrawRect(gridRect, Color.clear);
            DrawGridBorder(gridRect);

            Rect cellRect = new Rect(position.x + EditorGUIUtility.labelWidth + marginx, position.y + marginy, cellWidth, cellHeight);
            DrawCells(rows, cellRect, cellWidth, cellHeight, marginx, marginy);
        }

        private void DrawGridBorder(Rect gridRect)
        {
            Handles.color = Color.black;
            Handles.DrawLine(new Vector3(gridRect.x - 1, gridRect.y - 1), new Vector3(gridRect.x + gridRect.width + 1, gridRect.y - 1));
            Handles.DrawLine(new Vector3(gridRect.x - 1, gridRect.y - 1), new Vector3(gridRect.x - 1, gridRect.y + gridRect.height + 1));
            Handles.DrawLine(new Vector3(gridRect.x + gridRect.width + 1, gridRect.y - 1), new Vector3(gridRect.x + gridRect.width + 1, gridRect.y + gridRect.height + 1));
            Handles.DrawLine(new Vector3(gridRect.x - 1, gridRect.y + gridRect.height + 1), new Vector3(gridRect.x + gridRect.width + 1, gridRect.y + gridRect.height + 1));
        }

        private void DrawCells(SerializedProperty rows, Rect cellRect, float cellWidth, float cellHeight, float marginx, float marginy)
        {
            for (int i = 0; i < 3; i++)
            {
                SerializedProperty row = rows.GetArrayElementAtIndex(i).FindPropertyRelative("cells");

                for (int j = 0; j < 3; j++)
                {
                    int V = row.GetArrayElementAtIndex(j).intValue;
                    int itemID = V;
                    int selectedIndex = GetItemIndexByID(itemID);

                    if (selectedIndex == -1)
                        selectedIndex = 0;

                    // Draw cell background color
                    EditorGUI.DrawRect(cellRect, new Color(.31f, .31f, .31f, 1.0f));

                    DrawCellBorder(cellRect);
                    DrawCellContent(cellRect, selectedIndex, cellWidth, cellHeight);

                    if (Event.current.type == EventType.MouseDown && cellRect.Contains(Event.current.mousePosition))
                    {
                        if (Event.current.shift)
                        {
                            row.GetArrayElementAtIndex(j).intValue = 0;
                            Event.current.Use(); // Consume the event to prevent context menu
                        }
                        else
                        {
                            currentProperty = row.GetArrayElementAtIndex(j);
                            ItemPickerWindow.ShowWindow(itemList, OnItemPicked);
                        }
                    }

                    cellRect.x += cellWidth + marginx * 2;
                }

                cellRect.x = cellRect.x - (cellWidth + marginx * 2) * 3;
                cellRect.y += cellHeight + marginy;
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

        private void DrawCellContent(Rect cellRect, int selectedIndex, float cellWidth, float cellHeight)
        {
            if (itemList.list[selectedIndex].sprite != null)
            {
                Sprite sprite = itemList.list[selectedIndex].sprite;
                Rect spriteRect = new Rect(sprite.rect.x / sprite.texture.width, sprite.rect.y / sprite.texture.height, sprite.rect.width / sprite.texture.width, sprite.rect.height / sprite.texture.height);
                GUI.DrawTextureWithTexCoords(new Rect(cellRect.x, cellRect.y, cellWidth, cellHeight), sprite.texture, spriteRect, true);
            }
        }

        private int GetItemIndexByID(int itemID)
        {
            for (int i = 0; i < itemList.list.Count; i++)
            {
                if (itemList.list[i].id == itemID)
                {
                    return i;
                }
            }

            return -1;
        }

        private void OnItemPicked(int itemID)
        {
            currentProperty.intValue = itemID;
            currentProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}
