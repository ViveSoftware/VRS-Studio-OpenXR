//====================== Copyright 2017-2024, HTC.Corporation. All rights reserved. ======================
using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace HTC.UnityPlugin.Vive3DSP
{
    [AddComponentMenu("VIVE/3DSP_AudioOcclusion")]
    [HelpURL("https://hub.vive.com/storage/3dsp/vive_3dsp_audio_sdk_unity_plugin.html#audio-occlusion-settings")]
    public class Vive3DSPAudioOcclusion : MonoBehaviour
    {
        public int Id
        {
            get { return id; }
            private set { id = value; }
        }
        private int id = -1;

        public OccGeometryMode OcclusionGeometry
        {
            get { return occlusionGeometry; }
            set { occlusionGeometry = value; }
        }
        [SerializeField]
        private OccGeometryMode occlusionGeometry = OccGeometryMode.Box;

        public bool OcclusionEffect
        {
            set { occlusionEffect = value; }
            get { return occlusionEffect; }
        }
        [SerializeField]
        private bool occlusionEffect = true;

        public OccMaterial OcclusionMaterial
        {
            set { occlusionMaterial = value; }
            get { return occlusionMaterial; }
        }
        [SerializeField]
        private OccMaterial occlusionMaterial = OccMaterial.Curtain;

        public OccComputeMode OcclusionComputeMode
        {
            set { occlusionComputeMode = value; }
            get { return occlusionComputeMode; }
        }
        [SerializeField]
        private OccComputeMode occlusionComputeMode = OccComputeMode.VeryLight;

        public float OcclusionIntensity
        {
            set { occlusionIntensity = value; }
            get { return occlusionIntensity; }
        }
        [SerializeField]
        private float occlusionIntensity = 1.5f;

        public float HighFreqAttenuation
        {
            set { highFreqAttenuation = value; }
            get { return highFreqAttenuation; }
        }
        [SerializeField]
        private float highFreqAttenuation = -50.0f;

        public float LowFreqAttenuationRatio
        {
            set { lowFreqAttenuationRatio = value; }
            get { return lowFreqAttenuationRatio; }
        }
        [SerializeField]
        private float lowFreqAttenuationRatio = 0.0f;

        private VIVE_3DSP_OCCLUSION_PROPERTY occProperty;
        
        public IntPtr PropPtr
        {
            get
            {
                if (propPtr == IntPtr.Zero){
                    propPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VIVE_3DSP_OCCLUSION_PROPERTY))); 
                }
                Marshal.StructureToPtr(occProperty, propPtr, true);
                return propPtr;
            }
            set
            {
                if (propPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(propPtr);
                }
                propPtr = value;
            }
        }
        private IntPtr propPtr = IntPtr.Zero;

        public Vector3 OcclusionCenter
        {
            set { occlusionCenter = value; }
            get { return occlusionCenter; }
        }
        [SerializeField]
        private Vector3 occlusionCenter = Vector3.zero;

        public float OcclusionRadius
        {
            get { return occlusionRadius; }
            set { occlusionRadius = value; }
        }
        [SerializeField]
        private float occlusionRadius = 1.0f;

        public Vector3 OcclusionSize
        {
            set { occlusionSize = value; }
            get { return occlusionSize; }
        }
        [SerializeField]
        private Vector3 occlusionSize = Vector3.one;

        public float OcclusionHeight
        {
            set { occlusionHeight = value; }
            get { return occlusionHeight; }
        }
        [SerializeField]
        private float occlusionHeight = 1.0f;

        public Vector3 OcclusionRotation
        {
            set { occlusionRotation = value; }
            get { return occlusionRotation; }
        }
        [SerializeField]
        private Vector3 occlusionRotation = Vector3.zero;

        public float OcclusionAngle
        {
            set { occlusionAngle = value; }
            get { return occlusionAngle; }
        }
        [SerializeField]
        private float occlusionAngle = 30.0f;

        void Awake() {}
        
        void OnEnable() {}

        void Start()
        {
            Id = Vive3DSPAudio.CreateOcclusion(this);
            occProperty.id = Id;
        }

        void OnDisable()
        {
            occProperty.enable = 0u;
        }
        void OnDestroy(){
            PropPtr = IntPtr.Zero;
            Vive3DSPAudio.DestroyOcclusion(this);
            Id = -1;
        }

        void Update()
        {
            occProperty.enable = OcclusionEffect ? 1u : 0u;

            if (OcclusionGeometry == OccGeometryMode.Sphere)
            {
                var radius = (Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.y), Mathf.Abs(transform.lossyScale.z)) * OcclusionRadius) / 2;
                occProperty.radius = radius;
                occProperty.rotX = transform.rotation.x;
                occProperty.rotY = transform.rotation.y;
                occProperty.rotZ = transform.rotation.z;
                occProperty.rotW = transform.rotation.w;
                occProperty.height = 0.0f;
            }
            else if (OcclusionGeometry == OccGeometryMode.Box)
            {
                occProperty.radius = 0.0f;
                occProperty.rotX = transform.rotation.x;
                occProperty.rotY = transform.rotation.y;
                occProperty.rotZ = transform.rotation.z;
                occProperty.rotW = transform.rotation.w;
                occProperty.height = 0.0f;
            }
            else
            {
                var radius = (Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.z)) * OcclusionRadius) / 2;
                occProperty.radius = radius;
                Quaternion rot = transform.rotation * Quaternion.Euler(OcclusionRotation);
                occProperty.rotX = rot.x;
                occProperty.rotY = rot.y;
                occProperty.rotZ = rot.z;
                occProperty.rotW = rot.w;
                occProperty.height = transform.lossyScale.y * OcclusionHeight;
            }
            occProperty.density = OcclusionIntensity;
            occProperty.material = (uint)OcclusionMaterial;
            Vector3 pos = transform.position + transform.rotation * Vector3.Scale(OcclusionCenter, transform.lossyScale);
            occProperty.posX = pos.x;
            occProperty.posY = pos.y;
            occProperty.posZ = pos.z;
            Vector3 size = Vector3.Scale(transform.lossyScale, OcclusionSize);
            occProperty.sizeX = size.x;
            occProperty.sizeY = size.y;
            occProperty.sizeZ = size.z;
            occProperty.rhf = HighFreqAttenuation;
            occProperty.lfratio = LowFreqAttenuationRatio;
            occProperty.mode = (uint)OcclusionGeometry;
            occProperty.computeMode = OcclusionComputeMode;
            occProperty.angle = OcclusionAngle;
        }

        void OnDrawGizmosSelected()
        {
            if (OcclusionGeometry == OccGeometryMode.Sphere)
            {
                Gizmos.color = Color.black;
                var posUpdate = transform.position + transform.rotation * Vector3.Scale(OcclusionCenter, transform.lossyScale);
                float maxScaleVal = Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.y), Mathf.Abs(transform.lossyScale.z));
                Vector3 scaleVec = new Vector3(maxScaleVal, maxScaleVal, maxScaleVal);
                Gizmos.matrix = Matrix4x4.TRS(posUpdate, transform.rotation, scaleVec);
                Gizmos.DrawWireSphere(Vector3.zero, OcclusionRadius / 2);
            }
            else if (OcclusionGeometry == OccGeometryMode.Box)
            {
                Gizmos.color = Color.black;
                var posUpdate = transform.position + transform.rotation * Vector3.Scale(OcclusionCenter, transform.lossyScale);
                Gizmos.matrix = Matrix4x4.TRS(posUpdate, transform.rotation, transform.lossyScale);
                Gizmos.DrawWireCube(Vector3.zero, OcclusionSize);
            }
            else
            {
                Gizmos.color = Color.black;
                Vector3 centerShift = new Vector3(0, OcclusionHeight, 0);
                Vector3 occCenter = Vector3.zero;

                Matrix4x4 defaultMatrix = Gizmos.matrix;

                Vector3[] pointArray = new Vector3[4];
                Vector3[] topArray = new Vector3[4];
                Vector3[] btmArray = new Vector3[4];

                Quaternion rot = transform.rotation * Quaternion.Euler(OcclusionRotation);

                for (int i = 0; i < 2; i++)
                {
                    if (i == 0)
                        occCenter = OcclusionCenter + centerShift;
                    else
                        occCenter = OcclusionCenter - centerShift;

                    Vector3 posUpdate = transform.position + rot * Vector3.Scale(occCenter, transform.lossyScale);
                    float maxScaleVal = Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.z));
                    Vector3 scaleVec = new Vector3(maxScaleVal, maxScaleVal, maxScaleVal);
                    Gizmos.matrix = Matrix4x4.TRS(posUpdate, rot, scaleVec);

                    DrawCircle(pointArray);

                    if (i == 0)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            topArray[j] = rot * pointArray[j] * maxScaleVal + posUpdate;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            btmArray[j] = rot * pointArray[j] * maxScaleVal + posUpdate;
                        }
                    }
                }

                Gizmos.matrix = defaultMatrix;

                for (int j = 0; j < 4; j++)
                {
                    Gizmos.DrawLine(topArray[j], btmArray[j]);
                }
            }
        }

        void DrawCircle(Vector3[] pointArray)
        {
            Vector3 beginPoint = Vector3.zero;
            Vector3 firstPoint = Vector3.zero;
            int pointCnt = 0;
            for (int theta_cnt = 0; theta_cnt < 40; theta_cnt++)
            {
                float theta = (2 * Mathf.PI / 40) * theta_cnt;

                float x = (OcclusionRadius / 2) * Mathf.Cos(theta);
                float z = (OcclusionRadius / 2) * Mathf.Sin(theta);
                Vector3 endPoint = new Vector3(x, 0, z);
                if (theta == 0)
                {
                    firstPoint = endPoint;
                }
                else
                {
                    Gizmos.DrawLine(beginPoint, endPoint);
                }

                if (theta_cnt % 10 == 0)
                {
                    pointArray[pointCnt] = endPoint;
                    pointCnt++;
                }

                beginPoint = endPoint;
            }

            Gizmos.DrawLine(firstPoint, beginPoint);

        }
    }
}

