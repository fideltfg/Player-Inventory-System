using System;
using UnityEngine;

namespace PlayerInventorySystem.Serial
{
    /// <summary>
    /// Class to hold the details of a transform when saving.
    /// </summary>
    [Serializable]
    internal class SerialTransform
    {
        [SerializeField]
        public float positionX;
        public float positionY;
        public float positionZ;
        public float rotationX;
        public float rotationY;
        public float rotationZ;
        public float scaleX;
        public float scaleY;
        public float scaleZ;

        public Vector3 Position
        {
            get { return new Vector3(positionX, positionY, positionZ); }
        }

        public Vector3 Rotation
        {
            get { return new Vector3(rotationX, rotationY, rotationZ); }
        }

        public Vector3 Scale
        {
            get { return new Vector3(scaleX, scaleY, scaleZ); }
        }

        /// <summary>
        /// Construst. Pass in the transform you wish to serailize
        /// </summary>
        /// <param name="t"></param>
        public SerialTransform(Transform t)
        {
            positionX = t.position.x;
            positionY = t.position.y;
            positionZ = t.position.z;

            rotationX = t.localRotation.eulerAngles.x;
            rotationY = t.localRotation.eulerAngles.y;
            rotationZ = t.localRotation.eulerAngles.z;

            scaleX = t.localScale.x;
            scaleY = t.localScale.y;
            scaleZ = t.localScale.z;
        }

    }
}