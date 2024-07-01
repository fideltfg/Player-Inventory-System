using PlayerInventorySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameArea : MonoBehaviour
{
    public BoxCollider areaCollider;
    public int playerScore = 0;


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            ItemAction(other.gameObject);
        }
    }

    private void ItemAction(GameObject item)
    {
        // destroy the item
        Destroy(item);
    }
}
