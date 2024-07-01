using PlayerInventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemExplosion : MonoBehaviour
{
    public float TimeToLive = 3;

    public float Timer = 0;

    private void OnEnable()
    {
        // find the main camera
        Camera mainCamera = Camera.main;
        // get the canvas from this object
        Canvas canvas = GetComponent<Canvas>();
        // set the canvas render mode to world space void Update()
        {
            if (mainCamera != null)
            {
                Vector3 direction = mainCamera.transform.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(direction);
                canvas.transform.rotation = rotation;
            }
        }
    }


    // Update is called once per frame
    private void Update()
        {
            if (Timer >= TimeToLive)
            {
                Destroy(gameObject);
            }
            else
            {
                Timer += Time.deltaTime;
            }
        }
}
