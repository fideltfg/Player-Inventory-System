using UnityEngine;
using UnityEditor;
using System;
using PlayerInventorySystem;

public class ItemPickerWindow : EditorWindow
{
    public static Action<int> OnItemPicked;
    private SO_ItemCatalog itemList;
    private Vector2 scrollPosition;
    private const float itemButtonSize = 50f;
    private const float itemButtonPadding = 10f;
    private Texture2D defaultTexture;

    public static void ShowWindow(SO_ItemCatalog itemList, Action<int> onItemPicked)
    {
        var window = GetWindow<ItemPickerWindow>("Recipe Item Picker");
        window.itemList = itemList;
        OnItemPicked = onItemPicked;
        window.Show();
    }

    private void OnEnable()
    {
        // Create a default texture for items without sprites
        defaultTexture = new Texture2D(1, 1);
        defaultTexture.Apply();
    }

    private void OnGUI()
    {
        HandleWindowEvents();

        if (itemList == null) return;

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        GUILayout.Label("Select an Item", EditorStyles.boldLabel);

        float windowWidth = position.width;
        int itemsPerRow = Mathf.FloorToInt((windowWidth - itemButtonPadding) / (itemButtonSize + itemButtonPadding));
        int itemsInCurrentRow = 0;

        GUILayout.BeginHorizontal();
        foreach (var item in itemList.list)
        {
            if (itemsInCurrentRow >= itemsPerRow)
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                itemsInCurrentRow = 0;
            }

            GUILayout.BeginVertical();

            Texture2D texture = item.sprite != null ? ConvertSpriteToTexture2D(item.sprite) : defaultTexture;

            if (GUILayout.Button(texture, GUILayout.Width(itemButtonSize), GUILayout.Height(itemButtonSize)))
            {
                OnItemPicked?.Invoke(item.id);
                Close();
            }

            // Display name and ID overlay
            Rect lastRect = GUILayoutUtility.GetLastRect();
            GUI.Label(new Rect(lastRect.x, lastRect.yMax - 15, lastRect.width, 15), $"{item.id}", EditorStyles.whiteLabel);

            GUILayout.EndVertical();

            itemsInCurrentRow++;
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();
    }

    private void HandleWindowEvents()
    {
        Event e = Event.current;

        // Close window when pressing escape
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
        {
            Close();
        }
    }

    private void OnLostFocus()
    {
        Close();
    }

    private Texture2D ConvertSpriteToTexture2D(Sprite sprite)
    {
        if (sprite == null || sprite.texture == null)
        {
            return null;
        }

        int width = (int)sprite.rect.width;
        int height = (int)sprite.rect.height;

        if (width <= 0 || height <= 0)
        {
            Debug.LogWarning("Invalid sprite dimensions.");
            return null;
        }

        Texture2D texture = new Texture2D(width, height);

        if ((int)sprite.textureRect.x + width > sprite.texture.width || (int)sprite.textureRect.y + height > sprite.texture.height)
        {
            Debug.LogWarning("Texture2D.GetPixels: the size of data to be written to would result in writing outside the target buffer bounds.");
            return null;
        }

        Color[] pixels = sprite.texture.GetPixels(
            (int)sprite.textureRect.x,
            (int)sprite.textureRect.y,
            width,
            height
        );

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}
