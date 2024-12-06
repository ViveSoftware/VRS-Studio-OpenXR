using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

#if USE_INPUT_SYSTEM_POSE_CONTROL // Scripting Define Symbol added by using OpenXR Plugin 1.6.0.
using Pose = UnityEngine.InputSystem.XR.PoseState;
#else
using Pose = UnityEngine.XR.OpenXR.Input.Pose;
#endif

namespace VRSStudio.Tracker
{
    public class VRSTrackerManager : MonoBehaviour
    {
        [Serializable]
        public struct TrackerInfo
        {
            public uint trackerId;
            public InputActionReference isTrackedAction;
            public InputActionReference poseStateAction;

            public bool isTracked { get; private set; }
            public Pose pose { get; private set; }
            public void Update(bool isTracked, Pose pose)
            {
                this.isTracked = isTracked;
                this.pose = pose;
            }
        }

        const string TAG = "VRSTrackerManager";
        [SerializeField]
        private List<TrackerInfo> trackerList = new List<TrackerInfo>();
        private Dictionary<uint, onReallocateTrackerDelegate> allocatedPair = new Dictionary<uint, onReallocateTrackerDelegate>();
        private List<onReallocateTrackerDelegate> customers = new List<onReallocateTrackerDelegate>();
        private List<UnityEngine.XR.InputDevice> inputDevices = new List<UnityEngine.XR.InputDevice>();
        private List<uint> connectedIds = new List<uint>();
        private List<uint> availableIds = new List<uint>();

        int frameChecked = 0;

        void CheckOnceInThisFrame()
        {
            if (frameChecked == Time.frameCount) return;

            frameChecked = Time.frameCount;

            UpdateDictionary();
        }

        //private void Update()
        //{
        //    CheckOnceInThisFrame();
        //}

        void OnEnable()
        {
            if (Instance == null)
                Instance = this;

            for (int i = 0; i < trackerList.Count; i++)
            {
                TrackerInfo trackerInfo = trackerList[i];
                if (trackerInfo.isTrackedAction != null && trackerInfo.isTrackedAction.action != null)
                {
                    trackerInfo.isTrackedAction.action.Enable();
                }
                if (trackerInfo.poseStateAction != null && trackerInfo.poseStateAction.action != null)
                {
                    trackerInfo.poseStateAction.action.Enable();
                }
                trackerList[i] = trackerInfo;
            }
        }

        private void OnDisable()
        {
            Instance = null;
        }

        public static VRSTrackerManager Instance { get; private set; }

        /// <summary>
        /// Get all connected tracker ids.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public uint[] GetConnectedIds()
        {
            CheckOnceInThisFrame();
            connectedIds.Clear();
            foreach (var tracker in trackerList)
            {
                if (tracker.isTracked)
                {
                    connectedIds.Add(tracker.trackerId);
                }
            }
            return connectedIds.ToArray();
        }


        /// <summary>
        /// Get an uint which is not currently used by others.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool GetFreeId(out uint id)
        {
            if (GetFreeIdList(out uint[] ids))
            {
                id = ids[0];
                return true;
            }
            id = int.MaxValue;
            return false;
        }

        public bool GetFreeIdList(out uint[] ids)
        {
            CheckOnceInThisFrame();
            GetConnectedIds();
            availableIds.Clear();
            for (int i = 0; i < connectedIds.Count; i++)
            {
                if (!allocatedPair.ContainsKey(connectedIds[i]))
                {
                    availableIds.Add(connectedIds[i]);
                }
            }

            ids = new uint[availableIds.Count];
            for (int i = 0; i < availableIds.Count; i++)
            {
                ids[i] = availableIds[i];
            }
            return ids.Length > 0;
        }

        public bool GetInputDevice(uint id, out UnityEngine.XR.InputDevice dev)
        {
            CheckOnceInThisFrame();
            InputDeviceCharacteristics chars = InputDeviceCharacteristics.TrackedDevice;
            inputDevices.Clear();
            InputDevices.GetDevicesWithCharacteristics(chars, inputDevices);

            for (int i = 0; i < inputDevices.Count; i++)
            {
                try
                {
                    if (inputDevices[i].serialNumber.Contains("VIVE_Ultimate_Tracker_") &&
                        inputDevices[i].serialNumber.Contains(id.ToString()))
                    {
                        dev = inputDevices[i];
                        //Debug.Log($"Found Tracker {inputDevices[i].serialNumber}");
                        return true;
                    }
                }
                catch (Exception) { }
            }
            dev = default;
            return false;
        }

        /// <summary>
        /// Check if device id is occupied by other customer.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsIdOccupied(uint id)
        {
            return !allocatedPair.ContainsKey(id);
        }

        /// <summary>
        /// Use to notify customer that your trackers are occupied by other customer.
        /// </summary>
        /// <param name="rolesWillBeReallocated">if null, means all tracker are will be reallocated.</param>
        public delegate void onReallocateTrackerDelegate(List<uint> rolesWillBeReallocated);

        /// <summary>
        /// If user want some trackers, means these trackers are already not in the orignal using purpose.  Notify callbacks to stop their usage.
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="reallocateNotifier"></param>
        /// <returns>
        /// If no tracker available, still return true.
        /// </returns>
        public bool RequestTrackers(List<uint> ids, onReallocateTrackerDelegate reallocateNotifier)
        {
            Log.d(TAG, "RequesetTracker");
            // Notify all customer who occupy the trackers
            List<onReallocateTrackerDelegate> needBeNotified = new List<onReallocateTrackerDelegate>();
            List<uint> needBeRemoved = new List<uint>();
            List<uint> needBeAdded = new List<uint>();
            foreach (var id in ids)
            {
                if (allocatedPair.ContainsKey(id))
                {
                    allocatedPair.TryGetValue(id, out var customer);
                    // If same customer request same tracker again, skip add.
                    if (customer == reallocateNotifier)
                        continue;
                    needBeAdded.Add(id);
                    needBeRemoved.Add(id);
                    // Make it unique.  Just call it once.
                    if (!needBeNotified.Contains(customer))
                    {
                        needBeNotified.Add(customer);
                    }
                }
                else
                    needBeAdded.Add(id);
            }

            foreach (var customer in needBeNotified)
            {
                customer(ids);
            }
            needBeNotified.Clear();

            foreach (var id in needBeRemoved)
            {
                allocatedPair.Remove(id);
            }
            needBeRemoved.Clear();

            foreach (var id in needBeAdded)
            {
                allocatedPair.Add(id, reallocateNotifier);
            }
            customers.Add(reallocateNotifier);

            return true;
        }

        /// <summary>
        /// Notify all cutomers that stop using any tracker.
        /// </summary>
        public void FreeAllTrackers()
        {
            var allTrackers = allocatedPair.Keys.ToList();
            foreach (var customer in customers)
            {
                customer(allTrackers);
            }
            customers.Clear();
            allocatedPair.Clear();
            return;
        }

        /// <summary>
        /// Free this cutomer's all trackers.
        /// Use this function, no notification will be received.
        /// </summary>
        /// <param name="reallocateNotifier">the customer</param>
        public void FreeAllTrackers(onReallocateTrackerDelegate reallocateNotifier)
        {
            List<uint> needBeRemoved = new List<uint>();
            foreach (var id in allocatedPair.Keys)
            {
                if (allocatedPair[id] != reallocateNotifier)
                    continue;
                needBeRemoved.Add(id);
            }

            foreach (var id in needBeRemoved)
            {
                allocatedPair.Remove(id);
            }
            needBeRemoved.Clear();
        }


        /// <summary>
        /// If customer request trackers, customer can release trakers requested. 
        /// Use this function, no notification will be received.
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="reallocateNotifier"></param>
        public void FreeTrackers(List<uint> ids, onReallocateTrackerDelegate reallocateNotifier)
        {
            List<uint> needBeRemoved = new List<uint>();
            foreach (var id in ids)
            {
                if (allocatedPair.ContainsKey(id))
                {
                    if (allocatedPair[id] != reallocateNotifier)
                        continue;
                    needBeRemoved.Add(id);
                }
            }

            foreach (var id in needBeRemoved)
            {
                allocatedPair.Remove(id);
            }
            needBeRemoved.Clear();
        }

        void UpdateDictionary()
        {
            for (int i = 0; i < trackerList.Count; i++)
            {
                TrackerInfo tracker = trackerList[i];
                if (tracker.isTrackedAction != null && tracker.isTrackedAction.action != null &&
                    tracker.poseStateAction != null && tracker.poseStateAction.action != null)
                {
                    try
                    {
                        bool isTracked = tracker.isTrackedAction.action.ReadValue<float>() > 0;
                        Pose pose = tracker.poseStateAction.action.ReadValue<Pose>();
                        tracker.Update(isTracked, pose);
                        trackerList[i] = tracker;
                    }
                    catch (Exception e) { }
                }
            }
        }
    }
}