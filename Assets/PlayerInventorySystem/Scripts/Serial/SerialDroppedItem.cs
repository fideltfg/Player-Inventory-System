namespace PlayerInventorySystem
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class SerialDroppedItem
    {
        [SerializeField] private int itemID;
        [SerializeField] private int stackCount;
        [SerializeField] private float durability;
        [SerializeField] private float timeToLive;
        [SerializeField] private Vector3 position;
        [SerializeField] private float x;
        [SerializeField] private float y;
        [SerializeField] private float z;

        /// <summary>
        /// The ID of the dropped item.
        /// </summary>
        public int ItemID
        {
            get { return itemID; }
            private set { itemID = value; }
        }

        /// <summary>
        /// The number of items in the dropped item's stack.
        /// </summary>
        public int StackCount
        {
            get { return stackCount; }
            private set { stackCount = value; }
        }

        /// <summary>
        /// The durability of the dropped item.
        /// </summary>
        public float Durability
        {
            get { return durability; }
            private set { durability = value; }
        }

        /// <summary>
        /// The time to live (TTL) of the dropped item (how long it remains in the game world before disappearing).
        /// </summary>
        public float TimeToLive
        {
            get { return timeToLive; }
            private set { timeToLive = value; }
        }

        /// <summary>
        /// The x-coordinate of the dropped item's position in the game world.
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
            private set { position = value; }
        }

        /// <summary>
        /// Constructor to create a new SerialDroppedItem with the provided data.
        /// </summary>
        /// <param name="itemID">The ID of the dropped item.</param>
        /// <param name="stackCount">The number of items in the dropped item's stack.</param>
        /// <param name="durability">The durability of the dropped item.</param>
        /// <param name="timeToLive">The time to live (TTL) of the dropped item.</param>
        /// <param name="pos">The the dropped item's position in the game world.</param>
        public SerialDroppedItem(int itemID, int stackCount, float durability, float timeToLive, Vector3 pos)
        {
            ItemID = itemID;
            StackCount = stackCount;
            Durability = durability;
            TimeToLive = timeToLive;
            position = pos;
        }
    }

}