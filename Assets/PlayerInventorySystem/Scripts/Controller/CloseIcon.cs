using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace PlayerInventorySystem
{
    /// <summary>
    /// Helper component to provide close function to windows.
    /// Place this component on object you wish to be able to close/hide its parent
    /// </summary>
    public class CloseIcon : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick (PointerEventData eventData)
        {
            transform.parent.gameObject.SetActive(false);
        }
    }

}
