using PlayerInventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdvancedInventoryPanel : InventorySystemPanel
{

    public InventoryPanel inventoryPanel;
    public CraftingPanel craftingPanel;
    public CharacterPanel characterPanel;
    public SalvagePanel salvagePanel;

    public override void Build(int InventoryIndex)
    {
       // inventoryPanel.Build(0);
        craftingPanel.Build(2);
        characterPanel.Build(3);
        salvagePanel.Build(6);
    }

    public override void OnEnable()
    {
        InventoryController.Instance.OnWindowOpenCallback(this);
        transform.SetAsLastSibling();
    }

}
