using System;
using UnityEngine;

namespace PlayerInventorySystem.Serial
{

    [Serializable]
    internal class SerialPlacedItem
    {
        [SerializeField] private int itemID;
        [SerializeField] private SerialTransform transform;

        public SerialPlacedItem(int itemID, SerialTransform serialTransform)
        {
            Transform = serialTransform;
            ItemID = itemID;
        }

        /// <summary>
        /// The ID of the item catalog associated with the chest.
        /// </summary>
        internal int ItemID
        {
            get { return itemID; }
            set { itemID = value; }
        }

        /// <summary>
        /// The transform data (position, rotation, scale) of the chest.
        /// </summary>
        internal SerialTransform Transform
        {
            get { return transform; }
            set { transform = value; }
        }


    }
}