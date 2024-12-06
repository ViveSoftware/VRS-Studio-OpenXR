//====================== Copyright 2017-2024, HTC.Corporation. All rights reserved. ======================
using UnityEngine;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace HTC.UnityPlugin.Vive3DSP
{
    [AddComponentMenu("VIVE/3DSP_AudioRoom")]
    [HelpURL("https://hub.vive.com/storage/3dsp/vive_3dsp_audio_sdk_unity_plugin.html#audio-room-settings")]
    public class Vive3DSPAudioRoom : MonoBehaviour
    {
        public bool RoomEffect
        {
            set { roomEffect = value; }
            get { return roomEffect; }
        }
        [SerializeField]
        private bool roomEffect = true;

        private bool preRoomEffect = true;

        public RoomReverbPreset ReverbPreset
        {
            set { reverbPreset = value; }
            get { return reverbPreset; }
        }
        [SerializeField]
        private RoomReverbPreset reverbPreset = RoomReverbPreset.Generic;

        public RoomBackgroundAudioType BackgroundType
        {
            set
            {
                m_backgroundType = value;
                backgroundType = value;
                BackgroundClip = GetBackgroundAudioClip();
            }
            get { return m_backgroundType; }
        }
        [SerializeField]
        private RoomBackgroundAudioType backgroundType = RoomBackgroundAudioType.None;
        private RoomBackgroundAudioType m_backgroundType = RoomBackgroundAudioType.None;

        public int Id
        {
            get { return id; }
            private set { id = value; }
        }
        private int id = -1;

        [System.Obsolete("This will be deprecated. Please use BackgroundVolume instead.")]
        public float backgroundVolume
        {
            get { return m_sourceVolume; }
            set
            {
                m_sourceVolume = value;
                if (audioSource != null)
                {
                    audioSource.volume = (float)Math.Pow(10.0, m_sourceVolume * 0.05);
                }
            }
        }

        public float BackgroundVolume
        {
            get { return m_sourceVolume; }
            set
            {
                m_sourceVolume = value;
                sourceVolume = value;
                if (audioSource != null)
                {
                    audioSource.volume = (float)Math.Pow(10.0, m_sourceVolume * 0.05);
                }
            }
        }
        [SerializeField]
        private float sourceVolume = -30.0f;
        private float m_sourceVolume = -30.0f;
        public Vector3 Size
        {
            set { size = value; }
            get { return size; }
        }
        [SerializeField]
        private Vector3 size = Vector3.one;

        #region Walls Material
        public RoomPlateMaterial Ceiling
        {
            set { ceiling = value; }
            get { return ceiling; }
        }
        [SerializeField]
        private RoomPlateMaterial ceiling = RoomPlateMaterial.Steel;
        public RoomPlateMaterial FrontWall
        {
            set { frontWall = value; }
            get { return frontWall; }
        }
        [SerializeField]
        private RoomPlateMaterial frontWall = RoomPlateMaterial.Wood;
        public RoomPlateMaterial BackWall
        {
            set { backWall = value; }
            get { return backWall; }
        }
        [SerializeField]
        private RoomPlateMaterial backWall = RoomPlateMaterial.Wood;
        public RoomPlateMaterial RightWall
        {
            set { rightWall = value; }
            get { return rightWall; }
        }
        [SerializeField]
        private RoomPlateMaterial rightWall = RoomPlateMaterial.Wood;
        public RoomPlateMaterial LeftWall
        {
            set { leftWall = value; }
            get { return leftWall; }
        }
        [SerializeField]
        private RoomPlateMaterial leftWall = RoomPlateMaterial.Wood;

        public RoomPlateMaterial Floor
        {
            set { floor = value; }
            get { return floor; }
        }
        [SerializeField]
        private RoomPlateMaterial floor = RoomPlateMaterial.Steel;
        #endregion

        #region ReflectionRate
        public float CeilingReflectionRate
        {
            set { ceilingReflectionRate = value; }
            get { return ceilingReflectionRate; }
        }
        [SerializeField]

        private float ceilingReflectionRate = 1.0f;
        public float FrontWallReflectionRate
        {
            set { frontWallReflectionRate = value; }
            get { return frontWallReflectionRate; }
        }
        [SerializeField]
        private float frontWallReflectionRate = 1.0f;

        public float BackWallReflectionRate
        {
            set { backWallReflectionRate = value; }
            get { return backWallReflectionRate; }
        }
        [SerializeField]
        private float backWallReflectionRate = 1.0f;

        public float RightWallReflectionRate
        {
            set { rightWallReflectionRate = value; }
            get { return rightWallReflectionRate; }
        }
        [SerializeField]
        private float rightWallReflectionRate = 1.0f;

        public float LeftWallReflectionRate
        {
            set { leftWallReflectionRate = value; }
            get { return leftWallReflectionRate; }
        }
        [SerializeField]
        private float leftWallReflectionRate = 1.0f;

        public float FloorReflectionRate
        {
            set { floorReflectionRate = value; }
            get { return floorReflectionRate; }
        }
        [SerializeField]
        private float floorReflectionRate = 1.0f;
        public float ReflectionLevel
        {
            set { reflectionLevel = value; }
            get { return reflectionLevel; }
        }
        [SerializeField]
        public float reflectionLevel = 0.0f;
        #endregion

        public float ReverbLevel
        {
            set { reverbLevel = value; }
            get { return reverbLevel; }
        }
        [SerializeField]
        public float reverbLevel = 0.0f;

        [SerializeField]
        private AudioClip userDefineClip = null;
        
        private AudioSource audioSource
        {
            get
            {
                if (source == null)
                {
                    source = gameObject.AddComponent<AudioSource>();
                }
                return source;
            }
        }
        private AudioSource source = null;

        public bool IsCurrentRoom { get; set; } = false;

        private VIVE_3DSP_ROOM_PROPERTY roomProperty;
        
        public IntPtr PropPtr
        {
            get
            {
                if (propPtr == IntPtr.Zero)
                {
                    propPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VIVE_3DSP_ROOM_PROPERTY))); 
                }
                Marshal.StructureToPtr(roomProperty, propPtr, true);
                return propPtr;
            }
            private set
            {
                if (propPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(propPtr);
                }
                propPtr = value;
            }
        }
        private IntPtr propPtr = IntPtr.Zero;

        private AudioClip BackgroundClip
        {
            get { return audioSource.clip; }
            set
            {
                if (audioSource.clip != value)
                {
                    audioSource.clip = value;
                    PlayBackgroundAudio();
                }
            }
        }
        
        void Awake()
        {
            audioSource.hideFlags = HideFlags.HideInInspector | HideFlags.HideAndDontSave;
            audioSource.spatialize = false;
            audioSource.playOnAwake = true;
            audioSource.loop = true;
            audioSource.dopplerLevel = 0.0f;
            audioSource.spatialBlend = 0.0f;
            preRoomEffect = roomEffect;
        }
        
        void Start()
        {
            Id = Vive3DSPAudio.CreateRoom(this);
            roomProperty.id = Id;
            BackgroundClip = GetBackgroundAudioClip();
            BackgroundVolume = sourceVolume;
        }

        private void OnEnable()
        {
            RoomEffect = preRoomEffect;
            if (RoomEffect)
                roomProperty.enable = 1u;
            else
                roomProperty.enable = 0U;

            Update();
        }

        private void OnDisable()
        {
            preRoomEffect = RoomEffect;
            RoomEffect = false;
            roomProperty.enable = 0u;
            UpdateRoomProperty();
        }

        void Update()
        {
            if (backgroundType == RoomBackgroundAudioType.UserDefine)
            {
                BackgroundClip = GetBackgroundAudioClip();
            }
            UpdateRoomProperty();
        }

        void OnDestroy()
        {
            PropPtr = IntPtr.Zero;
            Vive3DSPAudio.DestroyRoom(this);
            Id = -1;
            Destroy(audioSource);
        }

        void UpdateRoomProperty()
        {
            roomProperty.enable = RoomEffect ? 1u : 0u;
            roomProperty.posX = transform.position.x;
            roomProperty.posY = transform.position.y;
            roomProperty.posZ = transform.position.z;
            roomProperty.rotX = transform.rotation.x;
            roomProperty.rotY = transform.rotation.y;
            roomProperty.rotZ = transform.rotation.z;
            roomProperty.rotW = transform.rotation.w;
            roomProperty.sizeX = Vector3.Scale(transform.lossyScale, Size).x;
            roomProperty.sizeY = Vector3.Scale(transform.lossyScale, Size).y;
            roomProperty.sizeZ = Vector3.Scale(transform.lossyScale, Size).z;

            roomProperty.preset = (uint)ReverbPreset;
            roomProperty.reflection_level = ReflectionLevel;
            roomProperty.reverb_level = ReverbLevel;
            roomProperty.gain = 1.0f;
            roomProperty.reflection_rate_left = LeftWallReflectionRate;
            roomProperty.reflection_rate_right = RightWallReflectionRate;
            roomProperty.reflection_rate_back = BackWallReflectionRate;
            roomProperty.reflection_rate_front = FrontWallReflectionRate;
            roomProperty.reflection_rate_ceiling = CeilingReflectionRate;
            roomProperty.reflection_rate_floor = FloorReflectionRate;
            roomProperty.material_left = (uint)LeftWall;
            roomProperty.material_right = (uint)RightWall;
            roomProperty.material_back = (uint)BackWall;
            roomProperty.material_front = (uint)FrontWall;
            roomProperty.material_ceiling = (uint)Ceiling;
            roomProperty.material_floor = (uint)Floor;

            if (backgroundType != BackgroundType)
            {
                BackgroundType = backgroundType;
            }

            if (sourceVolume != BackgroundVolume)
            {
                BackgroundVolume = sourceVolume;
            }

            if (RoomEffect)
            {
                PlayBackgroundAudio();
            }
            else
            {
                StopBackgroundAudio();
            }

        }

        private AudioClip GetBackgroundAudioClip()
        {
            AudioClip tempClip = null;
            switch (backgroundType)
            {
                case RoomBackgroundAudioType.UserDefine:
                    tempClip = userDefineClip;
                    break;
                case RoomBackgroundAudioType.None:
                    tempClip = null;
                    break;
                case RoomBackgroundAudioType.AirConditioner:
                    tempClip = (AudioClip)Resources.Load("BGAudioClips/AirConditionerBackgroundNoise");
                    break;
                case RoomBackgroundAudioType.BigRoom:
                    tempClip = (AudioClip)Resources.Load("BGAudioClips/BigRoomBackgroundNoise");
                    break;
                case RoomBackgroundAudioType.BrownNoise:
                    tempClip = (AudioClip)Resources.Load("BGAudioClips/BrownNoise");
                    break;
                case RoomBackgroundAudioType.PinkNoise:
                    tempClip = (AudioClip)Resources.Load("BGAudioClips/PinkNoise");
                    break;
                case RoomBackgroundAudioType.Refrigerator:
                    tempClip = (AudioClip)Resources.Load("BGAudioClips/RefrigeratorBackgroundNoise");
                    break;
                case RoomBackgroundAudioType.SmallRoom:
                    tempClip = (AudioClip)Resources.Load("BGAudioClips/SmallRoomBackgroundNoise");
                    break;
                case RoomBackgroundAudioType.WhiteNoise:
                    tempClip = (AudioClip)Resources.Load("BGAudioClips/WhiteNoise");
                    break;
                default:
                    break;
            }

            return tempClip;
        }

        public void PlayBackgroundAudio()
        {
            if ((!audioSource.isPlaying) && (backgroundType != RoomBackgroundAudioType.None) && (IsCurrentRoom == true) && RoomEffect)
            {
                audioSource.Play();
            }
        }

        public void StopBackgroundAudio()
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, size);
        }
    }
}
