using System;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerInventorySystem
{
    /// <summary>
    /// Component to handel the durability sliders on slot.
    /// </summary>
    public class dSlider : MonoBehaviour
    {
        Slider slider;

        public Image fillImage;
        // Use this for initialization
        void Start ()
        {
            slider = GetComponent<Slider>();
        }

        // Update is called once per frame
        void Update ()
        {
            CheckValue();
        }

        private void CheckValue ()
        {
            if ((slider.value / slider.maxValue) * 100 < 25)
            {
                fillImage.color = Color.red;
            }
            else if ((slider.value / slider.maxValue) * 100 < 50)
            {
                fillImage.color = Color.yellow;
            }
            else if ((slider.value / slider.maxValue) * 100 <= 100)
            {
                fillImage.color = Color.green;
            }
        }
    }
}
