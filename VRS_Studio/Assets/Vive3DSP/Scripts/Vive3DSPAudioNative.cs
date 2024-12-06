//====================== Copyright 2017-2024, HTC.Corporation. All rights reserved. ======================
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace HTC.UnityPlugin.Vive3DSP
{
    public enum HrtfModel
    {
        Real = 0,
        Lossless
    }

    public enum HeadsetConfig
    {
        Generic = 0,
        AutoDetection
    }

    public enum HeadsetModel
    {
        Generic = 0,
        VIVEPro,
        VIVEPro2,
        Focus3,
        VIVEXRSeries
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VIVE_3DSP_SOURCE_PROPERTY
    {
        public int sourceId;
        public uint spatializerEnable;
        public uint reverbEnable;
        public uint occlusionEnable;
        public uint soundDecayEnable;
        public uint sourceDirectivityEnable;
		public uint nearfieldEnable;

        public uint chNum;

        public uint occType;  // 0: FreqDep, 1: FreqFree

        public uint decayMode; 

        public float gain;
        public float minDistance;
        public float maxDistance;
        public float minDecayVolumeDb;
        public float posX;
        public float posY;
        public float posZ;
        public float rotX;
        public float rotY;
        public float rotZ;
        public float rotW;
        public float shape;
        public float focus;
        public float nearInten;
    }
    public struct VIVE_3DSP_ROOM_PROPERTY
    {
        public int id;
        public uint enable;
        public float posX;
        public float posY;
        public float posZ;
        public float rotX;
        public float rotY;
        public float rotZ;
        public float rotW;
        public float sizeX;
        public float sizeY;
        public float sizeZ;
        public uint preset;
        public uint material_left;
        public uint material_right;
        public uint material_back;
        public uint material_front;
        public uint material_floor;
        public uint material_ceiling;
        public float reflection_rate_left;
        public float reflection_rate_right;
        public float reflection_rate_back;
        public float reflection_rate_front;
        public float reflection_rate_floor;
        public float reflection_rate_ceiling;
        public float gain;
        public float reflection_level;
        public float reverb_level;
    }

    public struct VIVE_3DSP_OCCLUSION_PROPERTY
    {
        public int id;
        public uint enable;
        public float posX;
        public float posY;
        public float posZ;
        public float rotX;
        public float rotY;
        public float rotZ;
        public float rotW;
        public float sizeX;
        public float sizeY;
        public float sizeZ;
        public uint material;
        public float rhf;
        public float lfratio;
        public float density;
        public float radius;
        public float height;
        public uint mode;
        public OccComputeMode computeMode;
        public float angle;
    }

    public class Vive3DSPAudioNative
    {
        public const string pluginName = "audioplugin";

        protected Vive3DSPAudioNative() { }

        [DllImport(pluginName, EntryPoint = "setSourceProperty", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setSourceProperty(int sourceId, IntPtr prop);
        public static void SetSourceProperty(int sourceId, IntPtr prop)
        {
            try
            {
                if (sourceId != -1){
                    setSourceProperty(sourceId, prop);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        [DllImport(pluginName, EntryPoint = "setListenerGain", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setListenerGain(float gain);
        public static void Set3DSPListenerGain(float gain)
        {
            try
            {
                setListenerGain(gain);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        [DllImport(pluginName, EntryPoint = "setListenerHrtfModel", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setListenerHrtfModel(HrtfModel hrtfModel);
        public static void Set3DSPListenerHrtfModel(HrtfModel hrtfModel)
        {
            try
            {
                setListenerHrtfModel(hrtfModel);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        [DllImport(pluginName, EntryPoint = "setListenerHeadsetModel", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setListenerHeadsetModel(HeadsetModel headsetModel);
        public static void Set3DSPListenerHeadsetModel(HeadsetModel headsetModel)
        {
            try
            {
                setListenerHeadsetModel(headsetModel);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        [DllImport(pluginName, EntryPoint = "setWatermarkString", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setWatermarkString(string watermarkString);
        public static void Set3DSPWatermarkString(string watermarkString)
        {
            try
            {
                setWatermarkString(watermarkString);

            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        [DllImport(pluginName, EntryPoint = "setWatermarkEnable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setWatermarkEnable(bool isAddWatermark);
        public static void Set3DSPWatermarkEnable(bool isAddWatermark)
        {
            try
            {
                setWatermarkEnable(isAddWatermark);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        [DllImport(pluginName, EntryPoint = "createRoom", CallingConvention = CallingConvention.Cdecl)]
        private static extern int createRoom();
        public static int Create3DSPRoomObject()
        {
            try
            {
                return createRoom();
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                return -1;
            }
        }

        [DllImport(pluginName, EntryPoint = "destroyRoom", CallingConvention = CallingConvention.Cdecl)]
        private static extern void destroyRoom(int room);
        public static void Destroy3DSPRoomObject(int room)
        {
            try
            {
                destroyRoom(room);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
        
        [DllImport(pluginName, EntryPoint = "setRoomProperty", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setRoomProperty(int room, IntPtr prop);
        public static void Set3DSPRoomProperty(int room, IntPtr prop)
        {
            try
            {
                setRoomProperty(room, prop);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        [DllImport(pluginName, EntryPoint = "createOccluder", CallingConvention = CallingConvention.Cdecl)]
        private static extern int createOccluder();
        public static int Create3DSPOcclusionObject()
        {
            try
            {
                return createOccluder();
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                return -1;
            }
        }

        [DllImport(pluginName, EntryPoint = "destroyOccluder", CallingConvention = CallingConvention.Cdecl)]
        private static extern void destroyOccluder(int occ);
        public static void Destroy3DSPOcclusionObject(int occ)
        {
            try
            {
                destroyOccluder(occ);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                return;
            }
        }

        [DllImport(pluginName, EntryPoint = "setOccluderProperty", CallingConvention = CallingConvention.Cdecl)]
        private static extern int setOccluderProperty(int occlusionId, IntPtr prop);
        public static Int32 Set3DSPOcclusionProperty(int occlusionId, IntPtr prop)
        {
            try
            {
                return setOccluderProperty(occlusionId, prop);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                return -1;
            }
        }

        [DllImport(pluginName, EntryPoint = "getVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern int getVersion(IntPtr ver);
        public static Int32 Get3DSPNativeVersion(IntPtr ver)
        {
            try
            {
                return getVersion(ver);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                return -1;
            }
        }
    }
}
