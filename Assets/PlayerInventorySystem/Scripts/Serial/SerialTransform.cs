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
        [SerializeField] public float px;
        [SerializeField] public float py;
        [SerializeField] public float pz;

        [SerializeField] public float rx;
        [SerializeField] public float ry;
        [SerializeField] public float rz;

        [SerializeField] public float sx;
        [SerializeField] public float sy;
        [SerializeField] public float sz;

        public Vector3 Position => new(px, py, pz);
        public Vector3 Rotation => new(rx, ry, rz);
        public Vector3 Scale => new(sx, sy, sz);

        /// <summary>
        /// Construst. Pass in the transform you wish to serailize
        /// </summary>
        /// <param name="t"></param>
        public SerialTransform(Transform t)
        {
            px = t.position.x;
            py = t.position.y;
            pz = t.position.z;

            rx = t.localRotation.eulerAngles.x;
            ry = t.localRotation.eulerAngles.y;
            rz = t.localRotation.eulerAngles.z;

            sx = t.localScale.x;
            sy = t.localScale.y;
            sz = t.localScale.z;
        }

        public SerialTransform() { }
    }
}