using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRSStudio.Avatar;
using VRSStudio.Common;

namespace VRSStudio.Tracker
{
    public class TrackerIndicatorManager : MonoBehaviour
    {
        readonly Dictionary<uint, TrackerIndicator> dict = new Dictionary<uint, TrackerIndicator>();

        Timer updateTimer = new Timer(0.5f);

        public GameObject indicatorPrefab;

        private void OnEnable()
        {
            if (indicatorPrefab == null)
                enabled = false;
        }

        private void OnDisable()
        {
            foreach (var indicator in dict.Values)
            {
                Destroy(indicator.gameObject);
            }
            dict.Clear();
        }

        void Update()
        {
            if (!updateTimer.IsSet)
                updateTimer.Set();
            if (!updateTimer.Check())
                return;

            var tm = VRSTrackerManager.Instance;
            if (tm == null) return;

            var connected = tm.GetConnectedIds();
            foreach (var connectedId in connected)
            {
                if (dict.TryGetValue(connectedId, out TrackerIndicator indicator))
                {
                    tm.GetInputDevice(connectedId, out var dev);
                    indicator.SetDevice((int)connectedId, dev, GetDeviceName(connectedId));
                }
                else
                {
                    var instance = Instantiate(indicatorPrefab);
                    indicator = instance.GetComponent<TrackerIndicator>();
                    indicator.name = connectedId.ToString();
                    tm.GetInputDevice(connectedId, out var dev);
                    indicator.SetDevice((int)connectedId, dev, GetDeviceName(connectedId));
                    dict.Add(connectedId, indicator);
                }
            }
        }

        private string GetDeviceName(uint connectedId)
        {
            string name = "Tracker " + connectedId.ToString();
            if (VRSBodyTrackingManager.Instance)
            {
                string role = FindKeyByValue(VRSBodyTrackingManager.Instance.roleIdMapping, connectedId);
                if (!string.IsNullOrEmpty(role))
                {
                    name = role;
                }
            }
            return name;
        }

        private TKey FindKeyByValue<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TValue value)
        {
            return dictionary.FirstOrDefault(x => EqualityComparer<TValue>.Default.Equals(x.Value, value)).Key;
        }
    }
}