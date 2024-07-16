using PlayerInventorySystem;
using UnityEngine;

/// <summary>
/// This class takes care of player movement and jumping
/// </summary>


public class PlayerController_2D_Demo : MonoBehaviour
{
    Rigidbody2D rB;
    public float moveSpeed = 5;
    public float jumpForce = 5f;
    public LayerMask groundLayer;
    public Vector2 castSize = new Vector2(.5f, .5f);
    public float castDistance = 1f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public SpriteRenderer itemSprite;
    public static bool lastLookedLeft = false;

    [Tooltip("The audio source to play the sounds")]
    public AudioSource audioSource;

    [Tooltip("The audio clips to play when the player is hurt")]
    public AudioClip[] HurtSounds;

    [Tooltip("The audio clips to play when the player places something down")]
    public AudioClip[] PlaceSounds;

    [Tooltip("The audio clips to play when the player dropps an item")]
    public AudioClip[] DropSounds;

    [Tooltip("The audio clips to play when the player dies")]
    public AudioClip[] DeathSounds;

    [Tooltip("The audio clips to play when the player is mining")]
    public AudioClip[] MiningSounds;

    [Tooltip("The audio clips to play when the player is jumping")]
    public AudioClip[] JumpSounds;

    [Tooltip("The audio clips to play when the player is walking")]
    public AudioClip[] FootstepSounds;

    [Tooltip("The audio clips to play when the player is spawning")]
    public AudioClip[] SpawnSounds;

    [Tooltip("The audio clips to play when the player picks up an item")]
    public AudioClip[] PickupSounds;

    [Tooltip("The audio clips to play when the player attacks")]
    public AudioClip[] AttackSounds;

    private void OnEnable()
    {
        rB = GetComponent<Rigidbody2D>();

        // register for the callbacks from the player and inventory controller
        PlayerInventoryController_2D p = GetComponent<PlayerInventoryController_2D>();
        p.RegisterOnMineCallback(OnMine);
        p.RegisterOnAttackCallback(OnUseItem);
        p.RegisterOnPickupCallback(OnPickupItem);
        p.RegisterOnDropItemCallback(OnDropItem);

        InventoryController.RegisterOnSelectedItemChangeCallback(OnSelectedItemChange);

    }


    private void OnDisable()
    {
        // unregister for the callbacks from the player inventory controller
        PlayerInventoryController_2D p = GetComponent<PlayerInventoryController_2D>();
        p.UnRegisterOnMineCallback(OnMine);
        p.UnRegisterOnUseItemCallback(OnUseItem);
        p.UnRegisterOnPickupCallback(OnPickupItem);
        p.UnRegisterOnDropItemCallback(OnDropItem);
    }

    /// <summary>
    /// Called when the player attempts to mine
    /// </summary>
    private void OnMine(Mineable minable)
    {
        // play mining sound
        if (MiningSounds.Length > 0)
        {
            Debug.Log("Mining sound");
            audioSource.clip = MiningSounds[Random.Range(0, MiningSounds.Length)];
            audioSource.Play();
        }
    }

    /// <summary>
    /// Called when the player selects an item in the item bar
    /// 
    /// In this demo it is used to swap the icon above the player based on what is selected int he itembar.
    /// </summary>
    /// <param name="item"></param>
    public void OnSelectedItemChange(Item item)
    {
        if (item == null || item.ItemID == 0)
        {
            itemSprite.sprite = null;
            itemSprite.gameObject.SetActive(false);
        }
        else
        {
            // get the sprite from the item

            itemSprite.sprite = item.Data.sprite;
            itemSprite.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Called when the player attempts to attack
    /// </summary>
    private void OnUseItem(Item item)
    {
        // play mining sound
        if (AttackSounds.Length > 0)
        {
            Debug.Log("Attack sound");
            audioSource.clip = AttackSounds[Random.Range(0, AttackSounds.Length)];
            audioSource.Play();
        }
    }

    private void OnPickupItem(Item item)
    {
        // play mining sound
        if (PickupSounds.Length > 0)
        {
            Debug.Log("Pickup sound");
            audioSource.clip = PickupSounds[Random.Range(0, PickupSounds.Length)];
            audioSource.Play();
        }
    }

    private void OnDropItem(Item item)
    {
        // play mining sound
        if (DropSounds.Length > 0)
        {
            Debug.Log("Drop sound");
            audioSource.clip = DropSounds[Random.Range(0, DropSounds.Length)];
            audioSource.Play();
        }
    }

    private void Update()
    {
        Move();
    }

    private void FixedUpdate()
    {

        if (rB.velocity.y < 0)
        {
            rB.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rB.velocity.y > 0 && !InputController_2D.jump)
        {
            rB.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
    void Move()
    {
        float v = InputController_2D.V;

        if (v != 0)
        {
            // if the player is moving, play footstep sound
            // if the audio source is not playing
            if (!audioSource.isPlaying)
            {
                if (FootstepSounds.Length > 0)
                {
                    audioSource.clip = FootstepSounds[Random.Range(0, FootstepSounds.Length)];
                    //   audioSource.Play();
                }
            }
        }


        rB.velocity = new Vector2(v * moveSpeed, rB.velocity.y);
        GetComponent<Animator>().SetBool("Run", v != 0);
        GetComponent<SpriteRenderer>().flipX = v < 0;

        lastLookedLeft = v < 0;



    }

    public void Jump()
    {
        Debug.Log("Jump");
        if (IsGrounded())
        {
            rB.velocity = new Vector2(rB.velocity.x, jumpForce);
            // play jump sound
            if (JumpSounds.Length > 0)
            {
                audioSource.clip = JumpSounds[Random.Range(0, JumpSounds.Length)];
            }
        }
    }



    internal void OnPause()
    {
        // throw new NotImplementedException();
    }

    private bool IsGrounded()
    {
        bool grounded = Physics2D.BoxCast(transform.position, castSize, 0f, Vector2.down, castDistance, groundLayer);
        Debug.DrawLine(transform.position, transform.position + Vector3.down * castDistance, Color.red);
        Debug.Log($"IsGrounded: {grounded}");
        return grounded;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, castSize);
    }

}
