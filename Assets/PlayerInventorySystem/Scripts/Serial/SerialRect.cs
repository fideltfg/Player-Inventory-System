namespace PlayerInventorySystem.Serial
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class SerialRect : SerialTransform
    {
        [SerializeField] private float h;
        [SerializeField] private float w;

        public Vector2 Size => new(w, h);

        /// <summary>
        /// Constructor. Pass in the RectTransform you wish to serialize.
        /// </summary>
        /// <param name="rt">The RectTransform object to serialize.</param>
        public SerialRect(RectTransform rt)
        {
            if (rt == null)
            {
                Debug.LogError("Cannot create SerialRect: RectTransform object is null.");
                return;
            }

            w = rt.sizeDelta.x;
            h = rt.sizeDelta.y;

            px = rt.position.x;
            py = rt.position.y;
            pz = rt.position.z;

            rx = rt.localRotation.eulerAngles.x;
            ry = rt.localRotation.eulerAngles.y;
            rz = rt.localRotation.eulerAngles.z;

            sx = rt.localScale.x;
            sy = rt.localScale.y;
            sz = rt.localScale.z;
        }
    }

}