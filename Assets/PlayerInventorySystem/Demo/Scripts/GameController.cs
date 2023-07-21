using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public InputController InputController;

    public PlayerController PlayerController;


    private void Awake()
    {
        instance = this;
        PlayerController.gameObject.SetActive(true);
    }


    public void OnPause() { }



}
