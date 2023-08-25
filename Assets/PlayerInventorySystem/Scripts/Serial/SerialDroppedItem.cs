namespace PlayerInventorySystem.Serial
{
    using System;
    using UnityEngine;

    [Serializable]
    public class SerialDroppedItem
    {
        [SerializeField] private int itemID;
        [SerializeField] private int stackCount;
        [SerializeField] private float timeToLive;
        [SerializeField] private SerialTransform transform;

        /// <summary>
        /// Constructor to create a new SerialDroppedItem with the provided data.
        /// </summary>
        /// <param name="itemID">The ID of the dropped item.</param>
        /// <param name="stackCount">The number of items in the dropped item's stack.</param>
        /// <param name="timeToLive">The time to live (TTL) of the dropped item.</param>
        /// <param name="pos">The the dropped item's position in the game world.</param>
        internal SerialDroppedItem(int itemID, SerialTransform serialTransform, int stackCount, float timeToLive)
        {
            ItemID = itemID;
            StackCount = stackCount;
            TimeToLive = timeToLive;
            transform = serialTransform;
        }

        /// <summary>
        /// The ID of the dropped item.
        /// </summary>
        public int ItemID
        {
            get { return itemID; }
            set { itemID = value; }
        }

        /// <summary>
        /// The number of items in the dropped item's stack.
        /// </summary>
        public int StackCount
        {
            get { return stackCount; }
            set { stackCount = value; }
        }

        /// <summary>
        /// The time to live (TTL) of the dropped item (how long it remains in the game world before disappearing).
        /// </summary>
        public float TimeToLive
        {
            get { return timeToLive; }
            set { timeToLive = value; }
        }

        /// <summary>
        /// The x-coordinate of the dropped item's position in the game world.
        /// </summary>
        internal SerialTransform Transform
        {
            get { return transform; }
            set { transform = value; }
        }


    }

}