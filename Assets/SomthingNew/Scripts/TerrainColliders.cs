using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainColliders : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<TerrainCollider>().enabled = false;
        GetComponent<TerrainCollider>().enabled = true;
    }

}
