using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInventorySystem;


/// <summary>
/// Controller for the player input
/// This class acts as a bridge between the player input component and rest of the game.
/// </summary>

public class InputController : MonoBehaviour
{
    [HideInInspector]
    public float XRotation = 0;
    [HideInInspector]
    public float YRotation = 0;
    [HideInInspector]
    public bool modify = false; // akin to holding shift

    public void OnEscape(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            DemoGameController.instance.OnPause();
        }
    }

    public void OnForward(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            DemoPlayerController.instance.OnForward(1);
        }
        else
        {
            DemoPlayerController.instance.OnForward(0);
        }
    }

    public void OnBack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            DemoPlayerController.instance.OnBack(-1);
        }
        else
        {
            DemoPlayerController.instance.OnBack(0);
        }
    }

    public void OnLeft(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            DemoPlayerController.instance.OnLeft(-1);
        }
        else
        {
            DemoPlayerController.instance.OnLeft(0);
        }
    }

    public void OnRight(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            DemoPlayerController.instance.OnRight(1);
        }
        else
        {
            DemoPlayerController.instance.OnRight(0);
        }
    }

    public void OnLeftBumper(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            InventoryController.Instance.ItemBar.SelectPreviousSlot();
        }
    }

    public void OnRightBumper(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            InventoryController.Instance.ItemBar.SelectNextSlot();
        }
    }

    public void OnInteract(InputAction.CallbackContext ctx) // right click
    {
        if (ctx.performed)
        {
                InventoryController.Instance.PlayerInventoryControler.Interact();
        }
    }

    public void OnAltInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            InventoryController.Instance.ItemBar.DropSelectedItem();
        }
    }

    public void OnModifier(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            modify = true;
            InventoryController.InputModifier = true;
        }
        else if (ctx.canceled)
        {
            modify = false;
            InventoryController.InputModifier = false;
        }
    }


    public void OnUse(InputAction.CallbackContext ctx) // left click
    {
        if (ctx.performed)
        {
            if (InventoryController.HeldItem == null)
            {
                InventoryController.Instance.PlayerInventoryControler.UseCurrentItem();
            }
        }
        
    }

    public void OnAimSwitch(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            DemoPlayerController.instance.OnAimSwitch();
        }
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            DemoPlayerController.instance.OnJump();
        }
    }

    public void OnInventoryPanel(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("Inventory Panel");
            InventoryController.Instance.ToggleInventoryPanel();
        }
    }

    public void OnCharacterPanel(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            InventoryController.Instance.ToggleCharacterPanel();
        }
    }

    public void OnRotationX(InputAction.CallbackContext ctx)
    {
        XRotation = ctx.ReadValue<float>();
    }

    public void OnRotationY(InputAction.CallbackContext ctx)
    {
        YRotation = ctx.ReadValue<float>();
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
}
