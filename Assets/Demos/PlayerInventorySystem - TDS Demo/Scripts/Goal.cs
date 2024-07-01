using PlayerInventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public int goalID = 1; // the item ID the goal will accept

    public int goalCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            if(other.GetComponent<DroppedItem>().ItemID == goalID)
            {
                goalCount++;
                // destroy the item
                Destroy(other.gameObject);
            }
            else
            {
                goalCount--;
            }   

        }
    }

}
