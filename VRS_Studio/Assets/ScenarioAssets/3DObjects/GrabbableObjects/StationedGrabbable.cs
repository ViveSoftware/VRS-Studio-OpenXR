using HTC.UnityPlugin.LiteCoroutineSystem;
using HTC.UnityPlugin.Vive;
using System.Collections;
using UnityEngine;

namespace HTC.ViveSoftware.ExpLab.HandInteractionDemo
{
    public class StationedGrabbable : BasicGrabbable
    {
        [SerializeField]
        private Transform station;

        private LiteCoroutine stayStationedCoroutine;

        private bool moveByVelocity { get { return !unblockableGrab && grabRigidbody != null && !grabRigidbody.isKinematic; } }

        protected override void Awake()
        {
            base.Awake();
            onGrabberDrop += TryBackToStation;
            TryBackToStation();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            LiteCoroutine.StopCoroutine(stayStationedCoroutine);
        }

        private void TryBackToStation()
        {
            if (allGrabbers.Count == 0 && stayStationedCoroutine.IsNullOrDone())
            {
                if (moveByVelocity)
                {
                    LiteCoroutine.StartCoroutine(ref stayStationedCoroutine, RigidbodyStayStationedCoroutine());
                }
                else
                {
                    LiteCoroutine.StartCoroutine(ref stayStationedCoroutine, TransformStayStationedCoroutine());
                }
            }
        }

        private static WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        private IEnumerator RigidbodyStayStationedCoroutine()
        {
            yield return waitForFixedUpdate;
            GrabRigidbodyToPose(new UnityPlugin.Utility.RigidPose(transform));
            yield return waitForFixedUpdate;
            while (allGrabbers.Count == 0)
            {
                GrabRigidbodyToPose(new UnityPlugin.Utility.RigidPose(station));
                yield return waitForFixedUpdate;
            }
        }

        private IEnumerator TransformStayStationedCoroutine()
        {
            yield return null;
            GrabTransformToPose(new UnityPlugin.Utility.RigidPose(transform));
            yield return null;
            while (allGrabbers.Count == 0)
            {
                GrabTransformToPose(new UnityPlugin.Utility.RigidPose(station));
                yield return null;
            }
        }
    }
}