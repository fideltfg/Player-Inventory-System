using UnityEngine;
using System.Collections;


namespace PlayerInventorySystem
{

    /// <summary>
    /// Component added to item prefabs dot teh inventroy system can identify them for saving and respawning
    /// </summary>
    public class PlacedItem : MonoBehaviour
    {
        public int ItemID = 0;
    }
}
