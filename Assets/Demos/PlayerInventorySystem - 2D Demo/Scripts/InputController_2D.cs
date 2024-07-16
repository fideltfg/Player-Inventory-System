using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInventorySystem;


/// <summary>
/// Controller for 2D player input
/// </summary>
[RequireComponent(typeof(PlayerController_2D_Demo))]
[RequireComponent(typeof(PlayerInventoryController_2D))]
public class InputController_2D : MonoBehaviour
{
    public PlayerController_2D_Demo playerController;
    public PlayerInventoryController_2D playerInventoryController;

    public static float left = 0;
    public static float right = 0;
    public static bool jump = false;

    // V is the sum of left and right and is used to determine if the player should be moving
    // and in which direction
    public static float V
    {
        get { return left + right; }
    }

    public void OnEscape(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            playerController.OnPause();
        }
    }

    public void OnLeft(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            left = -1;
        }
        else
        {
            left = 0;
        }
    }

    public void OnRight(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            right = 1;
        }
        else
        {
            right = 0;
        }
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            playerController.Jump();
            jump = true;
        }
        else if (ctx.canceled)
        {
            jump = false;
        }
    }

    public void OnUse(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // can only use the item in the selected slot if there is no held item
            if (InventoryController.HeldItem == null)
            {
            
                    playerInventoryController.UseCurrentItem();
                
            }
        }
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
           
            if (InventoryController.HeldItem == null)
            {
                
                    //Debug.Log("Interact 1");
                    playerInventoryController.Interact();
              
            }
        }
    }

    public void OnDrop(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (InventoryController.HeldItem == null)
            {
                // drop the item in the selected slot if there is one
                if (InventoryController.Instance != null && InventoryController.Instance.ItemBar != null)
                {
                    InventoryController.Instance.ItemBar.DropSelectedItem();
                }
            }
        }
    }

    public void OnNumberSelect(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            string s = ctx.action.activeControl.ToString();
            // get the number from the string
            // int slotNum = int.Parse(s.Substring(s.Length - 1));
            int slotNum = int.Parse(s[^1..]);
            if (slotNum == 0)
            {
                slotNum = 9;
            }
            else { slotNum--; }

            InventoryController.Instance.ItemBar.SelectedSlotID = slotNum;
        }
    }

    public void OnOpenInventory(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (InventoryController.Instance != null)
            {
                InventoryController.Instance.ToggleInventoryPanel();
            }
        }
    }

}
