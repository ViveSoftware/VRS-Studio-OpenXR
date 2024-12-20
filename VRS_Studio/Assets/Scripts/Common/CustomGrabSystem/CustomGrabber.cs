using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRSStudio.Common.Input;

namespace VRSStudio.Input.CustomGrab
{
    // OnTriggerEnter/Exit need Rigidbody
    [RequireComponent(typeof(Rigidbody))]
    public class CustomGrabber : MonoBehaviour
    {
        private static string TAG = "Grabber";

        // Can be null
        public CustomGrabberIndicatorBehaviour behaviour;

        // grabAnchor's position and rotation will be changed according to grabbed object's grabPoseRef.  Public here is only for show in Inspector.
        public Transform grabAnchor;

        [SerializeField]
        bool isLeftHand = false;

        public enum GrabButton { None, Trigger, Grip };

        [SerializeField]
        private List<GrabButton> grabButtons = new List<GrabButton>();

        void Start()
        {
            var obj = new GameObject("GrabeAnchor");
            grabAnchor = obj.transform;
            grabAnchor.SetParent(transform, false);

            grabButtons.Distinct().ToList();
            if (grabButtons.Contains(GrabButton.None))
                grabButtons.Remove(GrabButton.None);

        }

        private CustomGrabbable grabbable = null;
        private CustomGrabbable grabbed = null;

        private Button btnActivated = null;

        // Last entered will be the grabbable
        List<Collider> collidersEntered = new List<Collider>();

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("grabbable"))
            {
                // I am not sure if one collider will be entered twice.
                if (collidersEntered.Contains(other)) return;
                var qg = other.GetComponent<CustomGrabbable>();
                if (qg == null)
                    return;
                collidersEntered.Add(other);
                grabbable = qg;
                behaviour?.OnAvailable(this, true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (collidersEntered.Count == 0)
            {
                grabbable = null;
                behaviour?.OnAvailable(this, false);
                return;
            }
            var id = collidersEntered.IndexOf(other);
            if (id < 0) return;

            collidersEntered.Remove(other);
            Collider next = null;
            if (collidersEntered.Count > 0)
                next = collidersEntered[collidersEntered.Count - 1];
            grabbable = next != null ? next.GetComponent<CustomGrabbable>() : null;
            behaviour?.OnAvailable(this, false);
        }

        internal void Grab()
        {
            if (grabbable == null) return;
            if (grabbable.IsGrabbed)
                grabbable.Grabber.Release(false);
            grabbed = grabbable;
            grabbed.IsGrabbed = true;
            grabbed.Grabber = this;
            var grabbedRb = grabbed.GetRigidbody();

            grabbedRb.isKinematic = true;
            behaviour?.OnGrab(this);
            if (grabbed.useFixedPose)
            {
                var grabPoseRef = isLeftHand ? CustomGrabbable.GrabPoseType.Left : CustomGrabbable.GrabPoseType.Right;
                var gposeRef = grabbed.GetGrabPoseRef(grabPoseRef);
                var grotInv = Quaternion.Inverse(gposeRef.localRotation);
                grabAnchor.localPosition = Vector3.Scale(grotInv * -gposeRef.localPosition, gposeRef.lossyScale);
                grabAnchor.localRotation = grotInv;
            }
            else
            {
                grabAnchor.position = grabbed.transform.position;
                grabAnchor.rotation = grabbed.transform.rotation;
            }
            UpdateGrabbedObjectPose();
            try { grabbed.OnGrabbed.Invoke(); }
            catch (System.Exception) { }
        }

        // For hand switch, releaseKinematic should be false
        internal void Release(bool releaseKinematic)
        {
            btnActivated = null;
            if (grabbed == null) return;
            var grabbedRb = grabbed.GetRigidbody();
            if (releaseKinematic)
            {
                if (grabbed.keepObjectKinematic)
                    grabbedRb.isKinematic = grabbed.IsKinematic;
                else
                    grabbedRb.isKinematic = false;
                grabbedRb.ResetInertiaTensor();
                var toMassCenter = grabbedRb.worldCenterOfMass - transform.position;
                var ctrl = GetController();
                if (ctrl != null)
                {
                    grabbedRb.velocity = Vector3.Cross(ctrl.angularVelocityW, toMassCenter) + ctrl.velocityW;
                    grabbedRb.angularVelocity = ctrl.angularVelocityW;
                }
            }
            grabbed.IsGrabbed = false;
            grabbed.Grabber = null;
            behaviour?.OnRelease(this);
            try { grabbed.OnReleased.Invoke(); }
            catch (System.Exception) { }

            grabbed = null;
        }

        private void UpdateGrabbedObjectPose()
        {
            var qg = CustomGrab.Instance;
            if (grabbed == null || qg == null) return;
            var grabbedRb = grabbed.GetRigidbody();

            grabbedRb.position = grabAnchor.position;
            grabbedRb.rotation = grabAnchor.rotation;
        }

        private Button GetButton(GrabButton gbtn)
        {
            var qg = CustomGrab.Instance;
            if (qg == null) return null;
            switch (gbtn)
            {
                case GrabButton.Trigger:
                    return isLeftHand ? qg.ctrlL.btnTrigger : qg.ctrlR.btnTrigger;
                case GrabButton.Grip:
                    return isLeftHand ? qg.ctrlL.btnGrip : qg.ctrlR.btnGrip;
                default:
                    return null;
            }
        }

        private CustomGrab.GrabController GetController()
        {
            var qg = CustomGrab.Instance;
            if (qg == null) return null;
            return isLeftHand ? qg.ctrlL : qg.ctrlR;
        }

        private void CheckGrab()
        {
            bool hasPressed = false;
            foreach (var button in grabButtons)
            {
                Button btn = GetButton(button);
                if (btn == null) continue;
                if (btn.IsDown && grabbable != null && btnActivated == null)
                {
                    btnActivated = btn;
                    Grab();
                }

                if (btn.IsUp && grabbed != null && btnActivated == btn)
                {
                    Release(true);
                    btnActivated = null;
                }

                if (btn.IsPressed)
                    hasPressed = true;
            }

            if (!hasPressed)
                btnActivated = null;
        }

        void Update()
        {
            var qg = CustomGrab.Instance;
            if (qg == null) return;

            qg.UpdateInput();

            UpdateGrabbedObjectPose();
            CheckGrab();
        }

        private void OnValidate()
        {
            if (grabAnchor != null) grabAnchor = null;
            if (grabButtons.Count == 0)
            {
                grabButtons.Add(GrabButton.Grip);
            }
            // In Editor, this will let list hard to edit
            //else
            //{
            //    // Remove duplicate
            //    grabButtons = grabButtons.Distinct().ToList();
            //}
        }
    }
}