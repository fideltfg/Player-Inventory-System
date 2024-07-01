using UnityEngine;
namespace PlayerInventorySystem
{
    public class TorchController : Interactive
    {
        public override void Interact(PlayerInventoryController playerInventoryController)
        {
            Debug.Log("Interacting with torch");
            float dur = InventoryController.Instance.ItemCatalog.GetItemByID(ItemID).Data.maxDurability;

            InventoryController.Instance.SpawnItem(ItemID, transform.position + transform.up, 1, 30, dur);

            Destroy(gameObject);
        }

    }

}