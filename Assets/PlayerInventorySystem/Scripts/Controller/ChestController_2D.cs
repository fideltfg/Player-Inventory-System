using PlayerInventorySystem;
using UnityEngine;

public class ChestController_2D : ChestController
{
    public override void Update() { 
     Debug.Log("ChestController_2D Update");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Open = false;
            ClosePanel();
        }
    }
}
