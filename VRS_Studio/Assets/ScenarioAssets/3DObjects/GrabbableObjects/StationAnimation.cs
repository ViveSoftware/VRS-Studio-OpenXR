using UnityEngine;

namespace HTC.ViveSoftware.ExpLab.HandInteractionDemo
{
    public class StationAnimation : MonoBehaviour
    {
        public float virticalOffset = 0.025f;
        public float virticalSpeed = 20f; // degree per sec
        public float rotateSpeed = 15f;

        private Vector3 startPos;
        private Quaternion startRot;

        private void Start()
        {
            startPos = transform.localPosition;
            startRot = transform.localRotation;
        }

        private void Update()
        {
            transform.localPosition = new Vector3(startPos.x, startPos.y + Mathf.Sin(Mathf.Repeat(Time.time * virticalSpeed, 360f) * Mathf.Deg2Rad) * virticalOffset, startPos.z);
            transform.localRotation = Quaternion.AngleAxis(Mathf.Repeat(Time.time * rotateSpeed, 360f), Vector3.up) * startRot;
        }

    }
}