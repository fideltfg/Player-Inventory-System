using PlayerInventorySystem;
using System.Collections;
using UnityEngine;

/// <summary>
/// This is a custom player controller for the TDS demo
/// </summary>

public class TDS_PlayerController : MonoBehaviour
{
    public float moveSpeed = 1;
    public float rotationsSpeed = 1f;
    public bool dead = true;

    public Transform throwSpawn;
    [HideInInspector]
    public AudioSource audioSource;
    [HideInInspector]
    public AudioClip[] HurtSounds;
    public AudioClip[] AudioList_0;
    public AudioClip[] PlaceSounds;
    public AudioClip[] DropSounds;
    public AudioClip[] DeathSounds;
    PlayerInventoryController playerInventoryController;

    private Vector3 movementVelocity = new Vector3();
    private Rigidbody rb;
    private bool running = false;

    public static TDS_PlayerController Instance { get; private set; }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        dead = false;


    }

    private void OnEnable()
    {
        Instance = this;


        // get a reference to the player inventory controller
        playerInventoryController = GetComponent<PlayerInventoryController>();

        /// register callback with inventory controller for when the player selects a new item
        //InventoryController.RegisterOnSlectedItemChangeCallback(playerWeaponController.HoldItem);

        rb = GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        // InventoryController.UnregisterOnSelectedItemChangeCallback(playerWeaponController.HoldItem);
        StopAllCoroutines();
    }

    float throwTimer = 0;
    float throwDelay = .25f;

    private void FixedUpdate()
    {
        if (running == false)
        {
            running = true;
        }

        LookAtMouse();
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Move(v, h);

        if (Input.GetMouseButton(0))
        {
            if (throwTimer <= 0)
            {
                throwTimer = throwDelay;
                ThrowSelectedItem();
            }
        }

        if (throwTimer > 0)
        {
            throwTimer -= Time.deltaTime;
        }


    }

    private void Move(float v, float h)
    {
        // limit the player to moving within the camera's frustum
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);



        movementVelocity.Set(h, 0, v);
        movementVelocity.Normalize();
        movementVelocity *= (movementVelocity.x == 1 && movementVelocity.z == 1) ? .707106f : 1f;
        movementVelocity *= (moveSpeed * Time.deltaTime);
        rb.velocity = movementVelocity;



    }

    private void LookAtMouse()
    {
        Quaternion targetRotation = Quaternion.LookRotation(MousePointer.cursorPos - (transform.position + new Vector3(-.5f, 0, -.5f)));
        targetRotation.x = 0;
        targetRotation.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationsSpeed);
    }

    public void ThrowSelectedItem()
    {
        // get the currently selected item from the itembar
        Item item = InventoryController.Instance.ItemBar.GetSelectedItem();

        if (item != null)
        {
            // spawn the item in the world
            // the item will be spawned at the throwSpawn position
            GameObject gameobject = InventoryController.Instance.SpawnItem(item.Data.id, throwSpawn.position, 1);
            
            // set the time to live for the item so it will be destroyed after a set amount of time
            gameobject.GetComponent<DroppedItem>().TimeToLive = Random.Range(5, 8);

            // consume the item from the selected slot in the itembar
            InventoryController.Instance.ItemBar.ConsumeSelectedItem();
            audioSource.PlayOneShot(AudioList_0[Random.Range(0, AudioList_0.Length - 1)]);

            // apply a force to the item in the direction the player is facing
            Rigidbody rb = gameobject.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 10 + transform.up, ForceMode.Impulse);
            rb.AddTorque(0, 0, Random.Range(-.25f, .25f), ForceMode.Impulse);



        }


    }

}