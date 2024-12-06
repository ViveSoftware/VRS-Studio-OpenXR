//====================== Copyright 2017-2024, HTC.Corporation. All rights reserved. ======================
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace HTC.UnityPlugin.Vive3DSP
{
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("VIVE/3DSP_AudioSource")]
    [HelpURL("https://hub.vive.com/storage/3dsp/vive_3dsp_audio_sdk_unity_plugin.html#audio-source-settings")]
    public class Vive3DSPAudioSource : MonoBehaviour
    {
        public int Id
        {
            get { return id; }
            private set { id = value; }
        }
        private int id = -1;

        public AudioSource audioSource
        {
            get { return GetComponent<AudioSource>(); }
        }

        public bool SoundDecayEffectSwitch
        {
            get { return soundDecayEffectSwitch; }
            set { soundDecayEffectSwitch = value; }
        }
        [SerializeField]
        private bool soundDecayEffectSwitch = false;

        public float Gain
        {
            get { return gain; }
            set { gain = value; }
        }
        [SerializeField]
        private float gain = 0.0f;

        public SoundDecayMode SoundDecayMode
        {
            set { soundDecayMode = value; }
            get { return soundDecayMode; }
        }
        [SerializeField]
        private SoundDecayMode soundDecayMode = SoundDecayMode.PointSourceDecay;

        public float MinimumDecayVolumeDb
        {
            set { minimumDecayVolumeDb = value; }
            get { return minimumDecayVolumeDb; }
        }
        [SerializeField]
        private float minimumDecayVolumeDb = -96.0f;

        public float MinDistance
        {
            get { return minDistance; }
            set { minDistance = Math.Max(value, 0.0f); }
        }
        [SerializeField]
        private float minDistance = 0.0f;

        public float MaxDistance
        {
            get { return maxDistance; }
            set { maxDistance = Math.Max(value, 0.0f); }
        }
        [SerializeField]
        private float maxDistance = 1000.0f;

        public bool Spatializer3d
        {
            get { return spatializer_3d; }
            set { spatializer_3d = value; }
        }
        [SerializeField]
        private bool spatializer_3d = true;

        public bool Reverb
        {
            get { return reverb; }
            set { reverb = value; }
        }
        [SerializeField]
        private bool reverb = false;

        public bool Occlusion
        {
            get { return occlusion; }
            set { occlusion = value; }
        }
        [SerializeField]
        private bool occlusion = false;

        public OccEffectEngine OcclusionEffectEngine
        {
            set { occlusionEffectEngine = value; }
            get { return occlusionEffectEngine; }
        }
        [SerializeField]
        private OccEffectEngine occlusionEffectEngine = OccEffectEngine.FrequencyDependent;

        public bool SourceDirectivity
        {
            get { return sourceDirectivity; }
            set { sourceDirectivity = value; }
        }
        [SerializeField]
        private bool sourceDirectivity = false;

        public float Shape
        {
            set { shape = value; }
            get { return shape; }
        }
        [SerializeField]
        private float shape = 0.0f;

        public float Focus
        {
            set { focus = value; }
            get { return focus; }
        }
        [SerializeField]
        private float focus = 1.0f;

    #if UNITY_EDITOR
        // Directivity gizmo meshes.
        private Mesh sourceDirectivityGizmoMesh = null;
        // Directivity gizmo resolution.
        private const int gizmoResolution = 180;
    #endif  // UNITY_EDITOR

        public bool Nearfield
        {
            get { return nearfield; }
            set { nearfield = value; }
        }
        [SerializeField]
        private bool nearfield = false;

        public float NearfieldIntensity
        {
            get { return nearInten; }
            set { nearInten = value; }
        }
        [SerializeField]
        private float nearInten = 3.0f;

        private VIVE_3DSP_SOURCE_PROPERTY sourceProp;
        
        public IntPtr PropPtr
        {
            get
            {
                if (propPtr == IntPtr.Zero){
                    propPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VIVE_3DSP_SOURCE_PROPERTY)));
                }
                
                Marshal.StructureToPtr(sourceProp, propPtr, true);
                
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

        public bool isVirtual
        {
            get { return audioSource.isVirtual; }
        }

        // Native audio spatializer effect data.
        public enum EffectData
        {
            SoundDecayModeSwitch = 0,
            SourceId,
            IsPlaying
        }

        void Awake()
        {
            if ((audioSource.spatialize == false) && (audioSource.spatialBlend == 0.0f))
            {
                audioSource.spatialBlend = 1.0f;
            };
        }

        void OnEnable()
        {
            Vive3DSPAudio.CreateSource(this);
            audioSource.enabled = true;
            audioSource.spatialize = true;
            
            if (audioSource.playOnAwake && !audioSource.isPlaying)
            {
                Play();
            }
        }

        void OnDisable()
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
            Vive3DSPAudio.DestroySource(this);
        }

        void OnDestroy()
        {
            PropPtr = IntPtr.Zero;
        }
        void Start()
        {
            if (audioSource.playOnAwake && !audioSource.isPlaying)
            {
                Play();
            }
        }

        void Update()
        {
            audioSource.spatialize = true;
            
            // Update effect data
            if (audioSource.spatialize)
            {
                audioSource.SetSpatializerFloat((int)EffectData.IsPlaying, Convert.ToInt32(audioSource.isPlaying));

                float id = 0.0f;
                audioSource.GetSpatializerFloat((int)EffectData.SourceId, out id);
                Id = (int)id;

                sourceProp.chNum = (uint)AudioSettings.speakerMode;

                sourceProp.sourceId = Id;
                sourceProp.gain = (float)Gain;

                sourceProp.posX = transform.position.x;
                sourceProp.posY = transform.position.y;
                sourceProp.posZ = transform.position.z;
                sourceProp.rotX = transform.rotation.x;
                sourceProp.rotY = transform.rotation.y;
                sourceProp.rotZ = transform.rotation.z;
                sourceProp.rotW = transform.rotation.w;

                sourceProp.spatializerEnable = Convert.ToUInt32(Spatializer3d);
                sourceProp.reverbEnable = Convert.ToUInt32(Reverb);
                sourceProp.occlusionEnable = Convert.ToUInt32(Occlusion);
                sourceProp.occType = (uint)OcclusionEffectEngine;  // 0: FreqDep, 1: FreqFree
                sourceProp.nearfieldEnable = Convert.ToUInt32(Nearfield);
                sourceProp.soundDecayEnable = Convert.ToUInt32(SoundDecayEffectSwitch);

                // over-write the unity volume rolloff
                audioSource.SetSpatializerFloat((int)EffectData.SoundDecayModeSwitch, Convert.ToInt32(SoundDecayEffectSwitch));
                sourceProp.decayMode = (uint)SoundDecayMode; 
                sourceProp.minDistance = MinDistance;
                sourceProp.maxDistance = MaxDistance;
                sourceProp.minDecayVolumeDb = MinimumDecayVolumeDb;
                sourceProp.sourceDirectivityEnable = Convert.ToUInt32(SourceDirectivity);
                sourceProp.shape = Shape;
                sourceProp.focus = Focus;
                sourceProp.nearInten = (float)NearfieldIntensity;
            }
        }

        public void Play()
        {
            if (!audioSource.isPlaying)
            {
                return;
            }
            audioSource.Play();
        }

        void OnApplicationFocus(bool hasFocus)
        {
            if ((hasFocus) && (audioSource != null))
            {
                audioSource.UnPause();
            }
        }

        #if UNITY_EDITOR
            void OnDrawGizmosSelected()
            {
                if (sourceDirectivity)
                {
                    Gizmos.color = Color.green;
                    if (sourceDirectivityGizmoMesh == null)
                    {
                        sourceDirectivityGizmoMesh = new Mesh();
                        sourceDirectivityGizmoMesh.hideFlags = HideFlags.HideAndDontSave;
                    }
                    DrawSourceDirectivity3DGizmo(transform, sourceDirectivityGizmoMesh, shape, focus);
                }
            }

            // Draw a source directivity pattern in the scene to show the relationship between the sound source and the listener.
            private void DrawSourceDirectivity3DGizmo(Transform source, Mesh mesh, float sourceShape, float sourceFocus)
            {
                Vector2[] points = Vive3DSPAudio.SetSourceDirectivityPoints(sourceShape, sourceFocus, gizmoResolution);
                int numVertices = 5 * gizmoResolution + 1;
                Vector3[] vertices = new Vector3[numVertices];
                vertices[0] = new Vector3(points[0].x, 0.0f, points[0].y);
                // The xz plane (points) draws a 3D source directivity field through the rotation of the direction axis of the sound source.
                float cos45 = 0.70710678f;
                for (int i = 0; i < points.Length; ++i)
                {
                    vertices[i * 5 + 1] = new Vector3(points[i].x, 0.0f, points[i].y);
                    vertices[i * 5 + 2] = new Vector3(points[i].x * cos45, points[i].x * cos45, points[i].y);
                    vertices[i * 5 + 3] = new Vector3(0.0f, points[i].x, points[i].y);
                    vertices[i * 5 + 4] = new Vector3(points[i].x * cos45, -points[i].x * cos45, points[i].y);
                    vertices[i * 5 + 5] = new Vector3(0.0f, -points[i].x, points[i].y);
                }
                int[] triangles = new int[12 * gizmoResolution];
                int index = 0;
                for (int i = 0; i < gizmoResolution - 1; ++i)
                {
                    if (i < gizmoResolution - 2 && i > 0) {
                        // The first triangle in the positive direction of the y-axis.
                        triangles[index++] = (i * 5 - 4);
                        triangles[index++] = ((i + 1) * 5 - 4);
                        triangles[index++] = ((i + 1) * 5 - 4) + 1;
                        // The second triangle in the positive direction of the y-axis.
                        triangles[index++] = (i * 5 - 4) + 1;
                        triangles[index++] = ((i + 1) * 5 - 4) + 1;
                        triangles[index++] = ((i + 1) * 5 - 4) + 2;
                        // The first triangle in the negative direction of the y-axis.
                        triangles[index++] = (i * 5 - 4);
                        triangles[index++] = ((i + 1) * 5 - 4) + 3;
                        triangles[index++] = ((i + 1) * 5 - 4);
                        // The second triangle in the negative direction of the y-axis.
                        triangles[index++] = (i * 5 - 4) + 3;
                        triangles[index++] = ((i + 1) * 5 - 4) + 4;
                        triangles[index++] = ((i + 1) * 5 - 4) + 3;
                    } else if (i == 0) {
                        // The first triangle in the positive direction of the y-axis.
                        triangles[index++] = i;
                        triangles[index++] = i + 1;
                        triangles[index++] = i + 2;
                        // The second triangle in the positive direction of the y-axis.
                        triangles[index++] = i;
                        triangles[index++] = i + 2;
                        triangles[index++] = i + 3;
                        // The first triangle in the negative direction of the y-axis.
                        triangles[index++] = i;
                        triangles[index++] = i + 5;
                        triangles[index++] = i + 4;
                        // The second triangle in the negative direction of the y-axis.
                        triangles[index++] = i;
                        triangles[index++] = i + 4;
                        triangles[index++] = i + 1;
                    } else {
                        // Connect the last four triangles back to the first vertex.
                        // The first triangle in the positive direction of the y-axis.
                        triangles[index++] = (i * 5 - 4);
                        triangles[index++] = 0;
                        triangles[index++] = (i * 5 - 4) + 1;
                        // The second triangle in the positive direction of the y-axis.
                        triangles[index++] = (i * 5 - 4) + 1;
                        triangles[index++] = 0;
                        triangles[index++] = (i * 5 - 4) + 2;
                        // The first triangle in the negative direction of the y-axis.
                        triangles[index++] = (i * 5 - 4) + 4;
                        triangles[index++] = 0;
                        triangles[index++] = (i * 5 - 4) + 3;
                        // The second triangle in the negative direction of the y-axis.
                        triangles[index++] = (i * 5 - 4) + 3;
                        triangles[index++] = 0;
                        triangles[index++] = (i * 5 - 4);
                    }
                }
                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.RecalculateNormals();
                float relativeMaxDis = Vive3DSPAudio.CalculateRelativeDistance(source);
                Vector3 scale = 2.0f * relativeMaxDis * Vector3.one;
                Gizmos.DrawWireMesh(mesh, source.position, source.rotation, scale);
            }
        #endif  // UNITY_EDITOR
    }
}
