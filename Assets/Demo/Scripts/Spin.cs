using UnityEngine;
using System.Collections;

namespace PlayerInventorySystem
{
    /// <summary>
    /// Helper component to demo objects in game.
    /// </summary>
    public class Spin : MonoBehaviour
    {
        public float XrotationsPerMinute = 10.0f;
        public float YrotationsPerMinute = 0f;
        public float ZrotationsPerMinute = 0f;
        void Update ()
        {
            transform.Rotate(6.0f * XrotationsPerMinute * Time.deltaTime, 6.0f * YrotationsPerMinute * Time.deltaTime, 6.0f * ZrotationsPerMinute * Time.deltaTime);
        }
    }
}