using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This is a simple game controller for the demo scene.
/// </summary>
public class DemoGameController : MonoBehaviour
{
    public static DemoGameController instance;

    public InputController InputController;

    public DemoPlayerController PlayerController;

    public GameObject starterItems;

    private void Awake()
    {
        instance = this;
        PlayerController.gameObject.SetActive(true);
    }


    public void OnPause() { }



}
