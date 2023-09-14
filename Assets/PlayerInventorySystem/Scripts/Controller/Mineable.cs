using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace PlayerInventorySystem
{

    public class Mineable : PlacedItem
    {

        // the max  number of items that could be mined here
        [Tooltip("The max  number of items that could be mined here")]
        public int maxResourceCount = 10;


        [Tooltip("The rate are which items will be mined")]
        public float hitRate = 10; //hits to mine an item

        int hitCounter = 0; // the current number of hits on this item since the last refresh

        // the item that will be mined from this mine by id
        [Tooltip("The item that will be mined from this mine by id")]
        public int minedItemID = 0;

        // the amount for time it takes to reset the mineing on this item and allow another hit
        [Tooltip("The amount for time it takes to reset the mineing on this item and allow another hit")]
        public float refreshRate = 5;

        float refreshCounter = 0;

        /// <summary>
        /// Method to mine this item. a hit is the act of the player striking the minable once
        /// this is time limited by the refreshRate
        /// </summary>
        /// <param name="hitPower"> how hard the player hits the minable</param>
        /// <returns>the item that was mined if any else null</returns>
        internal Item Mine(float hitPower)
        {

            Debug.Log("Mine! " + hitPower.ToString());

            hitCounter++; // add a hit
            Item minedItem = null;
            // if the hit counter is greater than the hit rate then mine the item
            if (hitCounter > (hitRate / hitPower))
            {
                refreshCounter = 0;
                hitCounter = 0;
                // give the player the mined item or spawn it in the world
                Debug.Log("Mined! ");

                minedItem = Item.New(minedItemID);
            }
            else
            {
                Debug.Log("Not Mined! ");
            }

            return minedItem;
        }

        private void Update()
        {
            if (hitCounter > 0)
            {
                refreshCounter += Time.deltaTime;
                if (refreshCounter >= refreshRate)
                {
                    hitCounter--;
                    refreshCounter = 0;
                }
            }
            else
            {
                refreshCounter = 0;
            }

        }

    }
}