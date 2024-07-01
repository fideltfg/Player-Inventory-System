using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerInventorySystem;

public class GridController : MonoBehaviour
{
    public Grid grid;
    Dictionary<Vector3, Item> gridMap =new();
    public int width = 5;
    public int height = 5;

    public void Start()
    {
        // build play area grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new(x,0,y);
                gridMap.Add(pos, null);
            }
        }
    }

}
