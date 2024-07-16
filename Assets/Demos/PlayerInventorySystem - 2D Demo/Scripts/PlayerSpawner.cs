using PlayerInventorySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{

   public float spawnDelay = 5f;
    float counter = 0f;
    bool isPlayerSpawned = false;
    // Start is called before the first frame update
    void OnEnable()
    {
        counter = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerSpawned)
        {
            //disable this component
            this.enabled = false;
        }
        counter += Time.deltaTime;
        if (counter >= spawnDelay)
        {
            counter = 0f;
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        if(InventoryController.Instance == null)
        {
            Debug.Log("Player prefab is not set in InventoryController");
            //disable this component
            this.enabled = false;
            return;
        }
        InventoryController.Instance.Player.SetActive(true);
        isPlayerSpawned = true;
    }
}
