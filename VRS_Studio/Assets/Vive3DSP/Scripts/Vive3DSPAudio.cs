//====================== Copyright 2017-2024, HTC.Corporation. All rights reserved. ======================
/**
*   release version:    1.3.9.0
*   script version:     1.3.9.0
*/

using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;

namespace HTC.UnityPlugin.Vive3DSP
{
    public enum OccComputeMode
    {
        VeryLight = 0,
        Simple,
        Normal,
        Detailed
    }

    public enum OccGeometryMode
    {
        Sphere = 0,
        Box,
        Cylinder,
        Preserved
    }

    public enum ChannelType
    {
        Mono = 0,                               /**< A single speaker, typically in front of the user. */
        Stereo
    }

    public struct Vive3DSPQuaternion
    {
        public float x;                         /**< The x-coordinate of the vector part. */
        public float y;                         /**< The y-coordinate of the vector part. */
        public float z;                         /**< The z-coordinate of the vector part. */
        public float w;
    }

    public enum OccMaterial
    {
        None = 0,
        Concrete ,
        PorousConcrete,
        Marble,
        Brick,
        Glass,
        Wood,
        Gypsum,
        Curtain,
        Plywood,
        Steel,
        StoneWall,
        PlasterAbsorber_Concrete,
        PlasterAbsorber_Brick,
        PlasterAbsorber_Wood,
        PlasterAbsorber_Gypsum,
        TimberBoard_Concrete,
        TimberBoard_Brick,
        TimberBoard_Wood,
        TimberBoard_Gypsum,
        CeramicTile_Concrete,
        CeramicTile_Brick,
        CeramicTile_Wood,
        CeramicTile_Gypsum,
        CorkBoard_Concrete,
        CorkBoard_Brick,
        CorkBoard_Wood,
        CorkBoard_Gypsum,
        SoftCurtain_Concrete,
        SoftCurtain_Brick,
        SoftCurtain_Wood,
        SoftCurtain_Gypsum,
        Curtain_Concrete,
        Curtain_Brick,
        Curtain_Wood,
        Curtain_Gypsum,
        Cotton_Concrete,
        Cotton_Brick,
        Cotton_Wood,
        Cotton_Gypsum,
        Plaster_Concrete,
        Plaster_Brick,
        Plaster_Wood,
        Plaster_Gypsum,
        Plywood_Concrete,
        Plywood_Brick,
        Plywood_Wood,
        Plywood_Gypsum,
        GlazedTile_Concrete,
        GlazedTile_Brick,
        GlazedTile_Wood,
        GlazedTile_Gypsum,
        UserDefine
    }

    public enum OccEffectEngine
    {
        FrequencyDependent = 0,
        FrequencyFree
    }

    public enum RoomPlane
    {
        Floor = 0,
        Ceiling,
        LeftWall,
        RightWall,
        FrontWall,
        BackWall
    }

    public enum RoomPlateMaterial
    {
        None = 0,
        Concrete ,
        PorousConcrete,
        Marble,
        Brick,
        Glass,
        Wood,
        Gypsum,
        Curtain,
        Plywood,
        Steel,
        StoneWall,
        PlasterAbsorber_Concrete,
        PlasterAbsorber_Brick,
        PlasterAbsorber_Wood,
        PlasterAbsorber_Gypsum,
        TimberBoard_Concrete,
        TimberBoard_Brick,
        TimberBoard_Wood,
        TimberBoard_Gypsum,
        CeramicTile_Concrete,
        CeramicTile_Brick,
        CeramicTile_Wood,
        CeramicTile_Gypsum,
        CorkBoard_Concrete,
        CorkBoard_Brick,
        CorkBoard_Wood,
        CorkBoard_Gypsum,
        SoftCurtain_Concrete,
        SoftCurtain_Brick,
        SoftCurtain_Wood,
        SoftCurtain_Gypsum,
        Curtain_Concrete,
        Curtain_Brick,
        Curtain_Wood,
        Curtain_Gypsum,
        Cotton_Concrete,
        Cotton_Brick,
        Cotton_Wood,
        Cotton_Gypsum,
        Plaster_Concrete,
        Plaster_Brick,
        Plaster_Wood,
        Plaster_Gypsum,
        Plywood_Concrete,
        Plywood_Brick,
        Plywood_Wood,
        Plywood_Gypsum,
        GlazedTile_Concrete,
        GlazedTile_Brick,
        GlazedTile_Wood,
        GlazedTile_Gypsum,
        UserDefine
    }

    public enum RoomReverbPreset
    {
        Generic = 0,
        Bathroom,
        Livingroom,
        Church,
        Arena,
        UserDefine
    }

    public enum SoundDecayMode
    {
        PointSourceDecay = 0,
        LineSourceDecay,
        LinearDecay,
        NoDecay,
    }

    public enum RoomBackgroundAudioType
    {
        None = 0,
        BigRoom,
        SmallRoom,
        AirConditioner,
        Refrigerator,
        PinkNoise,
        BrownNoise,
        WhiteNoise,
        UserDefine,
    }

    public static class Vive3DSPAudio
    {
        private static List<Vive3DSPAudioSource> srcList = new List<Vive3DSPAudioSource>();
        private static List<Vive3DSPAudioOcclusion> occList = new List<Vive3DSPAudioOcclusion>();

        private static HashSet<Vive3DSPAudioRoom> roomList = new HashSet<Vive3DSPAudioRoom>();

        public const string VIVE_3DSP_FORUM_URL = "https://forum.vive.com/forum/70-vive-audio-sdks/";

        public struct Vive3DSPVersion
        {
            public uint major;
            public uint minor;
            public uint build;
            public uint revision;
            public static Version Current
            {
                get {
                    int[] nativeVer = new int[4];
                    IntPtr verPtr = Marshal.AllocHGlobal(4 * sizeof(int));
                    Vive3DSPAudioNative.Get3DSPNativeVersion(verPtr);
                    Marshal.Copy(verPtr, nativeVer, 0, 4);
                    Version ver = new Version(nativeVer[0], nativeVer[1], nativeVer[2]);
                    Marshal.FreeHGlobal(verPtr);
                    return ver;
                }
            }
        }
        
        // Source
        public static void CreateSource(Vive3DSPAudioSource srcComponent)
        {
            srcList.Add(srcComponent);
        }
        public static void DestroySource(Vive3DSPAudioSource source)
        {
            srcList.Remove(source);
        }

        // Listener
        public static void CreateAudioListener(Vive3DSPAudioListener listener) {}

        public static void DestroyAudioListener() {}

        public static void SetHrtfModel(HrtfModel hrtfModel)
        {
            Vive3DSPAudioNative.Set3DSPListenerHrtfModel(hrtfModel);
        }

        public static void SetHeadsetModel(HeadsetConfig headsetConfig)
        {
            HeadsetModel headsetModel = HeadsetModel.Generic;
            if (headsetConfig != HeadsetConfig.Generic)
            {
#if (UNITY_STANDALONE_WIN)
                try
                {
                    Assembly assembly = Assembly.Load("SteamVR");
                    Type steamVRType = assembly.GetType("Valve.VR.SteamVR");
                    
                    PropertyInfo instancePropertyInfo = steamVRType.GetProperty("instance", BindingFlags.Public | BindingFlags.Static);
                    MethodInfo instanceMethodInfo = instancePropertyInfo.GetGetMethod();
                    object instance = instanceMethodInfo.Invoke(null, null);

                    PropertyInfo modelNamePropertyInfo = steamVRType.GetProperty("hmd_ModelNumber");
                    MethodInfo modelNameMethodInfo = modelNamePropertyInfo.GetGetMethod();
                    string headsetName = (string) modelNameMethodInfo.Invoke(instance, null);
                    
                    if (Regex.IsMatch(headsetName, @"[\s\S]*VIVE Pro 2[\s\S]*"))
                    {
                        headsetModel = HeadsetModel.VIVEPro2;
                        Debug.Log("VIVEPro2");
                    }
                    else if (Regex.IsMatch(headsetName, @"[\s\S]*VIVE_Pro[\s\S]*"))
                    {
                        headsetModel = HeadsetModel.VIVEPro;
                        Debug.Log("VIVEPro");
                    }
                }
                catch
                {
                    Debug.LogWarning("If you want to develope PCVR content, please install SteamVR plugin.");
                }
#elif UNITY_ANDROID
                if (Application.platform == RuntimePlatform.Android)
                {
                    string headsetName = SystemInfo.deviceModel;
                    if (Regex.IsMatch(headsetName, @"[\s\S]*HTC VIVE Focus 3[\s\S]*"))
                    {
                        headsetModel = HeadsetModel.Focus3;
                        Debug.Log("VIVEFocus3");
                    }else if(Regex.IsMatch(headsetName, @"[\s\S]*VIVE XR Series[\s\S]*")) 
                    {
                        headsetModel = HeadsetModel.VIVEXRSeries;
                        Debug.Log("VIVEXRSeries");
                    }
                }
#endif
            }
            Vive3DSPAudioNative.Set3DSPListenerHeadsetModel(headsetModel);
        }
        
        public static void SetGlobalGain(float gain)
        {
            Vive3DSPAudioNative.Set3DSPListenerGain(gain);
        }

        public static void SetWatermarkString(string watermarkString)
        {
            Vive3DSPAudioNative.Set3DSPWatermarkString(watermarkString);
        }

        public static void SetWatermarkEnable(bool isAddWatermark)
        {
            Vive3DSPAudioNative.Set3DSPWatermarkEnable(isAddWatermark);
        }

        public static int CreateOcclusion(Vive3DSPAudioOcclusion occ)
        {
            var id = Vive3DSPAudioNative.Create3DSPOcclusionObject();
            if (id != -1)
            {
                occList.Add(occ);
            }
            return id;
        }

        public static void DestroyOcclusion(Vive3DSPAudioOcclusion occ)
        {
            if (occ.Id != -1)
            {
                Vive3DSPAudioNative.Destroy3DSPOcclusionObject(occ.Id);
            }
            occList.Remove(occ);
        }
        
        // Room
        public static int CreateRoom(Vive3DSPAudioRoom room)
        {
            var id = Vive3DSPAudioNative.Create3DSPRoomObject();
            if (id != -1)
            {
                roomList.Add(room);
            }

            return id;
        }
        public static void DestroyRoom(Vive3DSPAudioRoom room)
        {
            roomList.Remove(room);
            Vive3DSPAudioNative.Destroy3DSPRoomObject(room.Id);
        }

        public static Vive3DSPAudioRoom FindCurrentRoom(GameObject listener)
        {
            Vive3DSPAudioRoom currentRoom = null;
            foreach (var room in roomList)
            {
                if (IsListenerInsideRoom(listener, room))
                {
                    if (currentRoom == null)
                    {
                        currentRoom = room;
                        continue;
                    }

                    if (currentRoom.Size.magnitude > room.Size.magnitude)
                    {
                        currentRoom = room;
                    }
                }
            }
            return currentRoom;
        }

        public static bool IsListenerInsideRoom(GameObject listener, Vive3DSPAudioRoom room)
        {
            if (room == null)
            {
                return false;
            }
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero); ;
            Vector3 relativePosition = listener.transform.position - room.transform.position;
            Quaternion rotationInverse = Quaternion.Inverse(room.transform.rotation);
            bounds.size = Vector3.Scale(room.transform.lossyScale, room.Size);
            return bounds.Contains(rotationInverse * relativePosition);
        }
        
        public static float ConvertAmplitudeFromDb(float db)
        {
            return Mathf.Pow(10.0f, 0.05f * db);
        }

        public static void PassObjToNative()
        {
            foreach (var src in srcList)
            {
                Vive3DSPAudioNative.SetSourceProperty(src.Id, src.PropPtr);
            }

            foreach (var room in roomList)
            {
                Vive3DSPAudioNative.Set3DSPRoomProperty(room.Id, room.PropPtr);
            }

            foreach (var occ in occList)
            {
                Vive3DSPAudioNative.Set3DSPOcclusionProperty(occ.Id, occ.PropPtr);
            }
        }

        // Set position of points for drawing source directivity.
        public static Vector2[] SetSourceDirectivityPoints(float sourceDirectivityShape, float sourceDirectivityFocus, int points) {
            Vector2[] pointsPosition = new Vector2[points];
            for (int i = 0; i < points; ++i) {
                float theta = i * 2.0f * Mathf.PI / points;
                float r = Mathf.Pow(Mathf.Abs((1 - sourceDirectivityShape) + sourceDirectivityShape * Mathf.Cos(theta)), sourceDirectivityFocus);
                float x = r * Mathf.Cos(theta);
                float y = r * Mathf.Sin(theta);
                pointsPosition[i] = new Vector2(y, x);
            }
            return pointsPosition;
        }

        // Calculate the relative distance between listener and source.
        public static float CalculateRelativeDistance(Transform audioSourceTransform) {
            float distanceX;
            float distanceZ;
            float maxDistance;
            var listener = GameObject.FindObjectOfType<AudioListener>();
            if (listener != null) {
                Transform listenerTransform = listener.transform;
            }
            distanceX = Mathf.Abs(audioSourceTransform.position.x - listener.transform.position.x);
            distanceZ = Mathf.Abs(audioSourceTransform.position.z - listener.transform.position.z);
            maxDistance = Mathf.Sqrt(distanceX * distanceX + distanceZ * distanceZ);
            return maxDistance;
        }
    }
}
