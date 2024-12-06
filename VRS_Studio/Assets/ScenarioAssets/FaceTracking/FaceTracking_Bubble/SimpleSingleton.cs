using UnityEngine;

namespace HTC.FaceTracking.Interaction
{
    public class SimpleSingleton<T> : MonoBehaviour where T : SimpleSingleton<T>
    {
        private static T instance = null;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType(typeof(T)) as T;

                }
                return instance;
            }
        }
    }
}