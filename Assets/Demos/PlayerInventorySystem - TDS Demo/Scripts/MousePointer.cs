using System;
using System.Collections.Generic;
using UnityEngine;
using PlayerInventorySystem;
using System.Collections;

public class MousePointer : MonoBehaviour
{
    // public Image image;
    public static bool blocked = false; // indicates if the mouse pointer is over an item
    public static float maxReach = 5f; // the range the player can interact with items
    public static Vector3 cursorPos;
    public static Vector3 placementVector;
    Camera cm;
    public bool InRange;
    private bool running = false;

    private void Awake ()
    {
        cm = Camera.main;
 
    }

    private void FixedUpdate ()
    {
        if (running == false)
        {
            running = true;
            StartCoroutine("Run");
        }
    }


    IEnumerator Run ()
    {
        while (true)
        {
            Vector3 mousePos = Input.mousePosition;
            cursorPos = cm.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cm.transform.position.y - transform.position.y));
            transform.localPosition = new Vector3(Mathf.Round(cursorPos.x), 0, Mathf.Round(cursorPos.z));
            placementVector = transform.localPosition;
          //  InRange = Vector3.Distance(cursorPos, PlayerController.Instance.transform.position) <= maxReach;

            
            yield return null;
        }
    }
    void OnTriggerEnter (Collider other)
    {
        blocked = true;

    }

    void OnTriggerExit (Collider other)
    {
        blocked = false;
    }
}
