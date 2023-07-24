namespace PlayerInventorySystem
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class SerialRect
    {
        [SerializeField] private float height;
        [SerializeField] private float width;

        [SerializeField] private float positionX;
        [SerializeField] private float positionY;
        [SerializeField] private float positionZ;

        [SerializeField] private float rotationX;
        [SerializeField] private float rotationY;
        [SerializeField] private float rotationZ;

        [SerializeField] private float scaleX;
        [SerializeField] private float scaleY;
        [SerializeField] private float scaleZ;

        public Vector2 Size => new Vector2(width, height);
        public Vector3 Position => new Vector3(positionX, positionY, positionZ);
        public Vector3 Rotation => new Vector3(rotationX, rotationY, rotationZ);
        public Vector3 Scale => new Vector3(scaleX, scaleY, scaleZ);

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

            width = rt.sizeDelta.x;
            height = rt.sizeDelta.y;

            positionX = rt.position.x;
            positionY = rt.position.y;
            positionZ = rt.position.z;

            rotationX = rt.localRotation.eulerAngles.x;
            rotationY = rt.localRotation.eulerAngles.y;
            rotationZ = rt.localRotation.eulerAngles.z;

            scaleX = rt.localScale.x;
            scaleY = rt.localScale.y;
            scaleZ = rt.localScale.z;
        }
    }

}