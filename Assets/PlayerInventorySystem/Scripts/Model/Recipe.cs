using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace PlayerInventorySystem
{

    /// <summary>
    /// Class to define and hold a crafting recipe
    /// </summary>
    [System.Serializable]
    public class Recipe
    {
        /// <summary>
        /// Row struct for Recipe
        /// </summary>
        [System.Serializable]
        public struct Row
        {
            public int[] cells;
        }

        /// <summary>
        /// Array of Row objects
        /// </summary>
        public Row[] rows = new Row[3];

        /// <summary>
        /// contains the the recipe in a format that can be used by the crafting system to test if a recipe is met.
        /// </summary>
        public string Ingredients
        {
            get
            {
                string r = "";
                foreach (Row row in rows)
                {
                    foreach (int cv in row.cells)
                    {
                        string x = cv.ToString();

                        if (x == "0" || x == "00")
                        {
                            x = "X";
                        }
                        else
                        {
                            x = x.PadLeft(2, '0');
                        }
                        r += x;
                    }
                }
                char[] t = { 'X' };
                r = r.Trim(t);
                return r;
            }
        }


        /// <summary>
        /// method to check if an item is required for this recipe
        /// </summary>
        /// <param name="x">The item id to look for in this recipe</param>
        /// <returns>True if the item is required else false</returns>
        public bool InRecipe(int x)
        {
            foreach (Row row in rows)
            {
                foreach (int v in row.cells)
                {
                    if (v == x)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal int[] GetRequiredItemIds()
        {
            List<int> ids = new List<int>();
            foreach (Row row in rows)
            {
                foreach (int cv in row.cells)
                {
                    ids.Add(cv);
                }
            }
            return ids.ToArray();
        }
    }
}
