//====================== Copyright 2017-2024, HTC.Corporation. All rights reserved. ======================
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace HTC.UnityPlugin.Vive3DSP
{
    [AddComponentMenu("VIVE/3DSP_AudioListener")]
    [HelpURL("https://hub.vive.com/storage/3dsp/vive_3dsp_audio_sdk_unity_plugin.html#audio-listener-settings")]
    public class Vive3DSPAudioListener : MonoBehaviour
    {
        public float GlobalGain
        {
            get { return globalGain; }
        }
        [SerializeField]
        private float globalGain = 0.0f;

        public Vive3DSPAudioRoom CurrentRoom
        {
            get { return currentRoom; }
            set {
                if (currentRoom != value)
                {
                    if (currentRoom != null)
                    {
                        currentRoom.StopBackgroundAudio();
                        currentRoom.IsCurrentRoom = false;
                    }

                    currentRoom = value;

                    if (currentRoom != null)
                    {
                        currentRoom.IsCurrentRoom = true;
                        currentRoom.PlayBackgroundAudio();
                    }
                }
            }
        }
        private Vive3DSPAudioRoom currentRoom;
        
        public HrtfModel HrtfModel
        {
            get { return hrtfModel; }
        }
        [SerializeField]
        private HrtfModel hrtfModel = HrtfModel.Real;

        public HeadsetConfig HeadsetConfig
        {
            get { return headsetConfig; }
        }
        [SerializeField]
        private HeadsetConfig headsetConfig = HeadsetConfig.Generic;

        public Vector3 Position
        {
            get { return transform.position; }
        }

        public Quaternion Rotation
        {
            get { return transform.rotation; }
        }

        public bool IsAddWatermark
        {
            get { return isAddWatermark; }
            set { isAddWatermark = value; }
        }
        [SerializeField]
        private bool isAddWatermark = false;

        public string WatermarkString
        {
            get { return watermarkString; }
            set { watermarkString = value; }
        }
        [SerializeField]
        private string watermarkString = "";

        void Awake()
        {
            Version ver = Vive3DSPAudio.Vive3DSPVersion.Current;
            if (Application.platform == RuntimePlatform.Android)
            {
                Debug.Log("v" + ver + ".android-beta");
            }
            else
            {
                Debug.Log("v" + ver + ".pc");
            }

            Vive3DSPAudio.SetHrtfModel(HrtfModel);
            Vive3DSPAudio.SetHeadsetModel(HeadsetConfig);
            OnValidate();
            Vive3DSPAudio.SetWatermarkEnable(IsAddWatermark);
            Vive3DSPAudio.SetWatermarkString(WatermarkString);
        }

        void OnEnable() {
            
        }

        void Update()
        {
            
            if (IsRecordToFile == true && 
                recordStatus == RecordStatus.STOP &&
                RecordFileName != "")
            {
                WavWriter.Create(RecordFileName);
                recordStatus = RecordStatus.RECORDING;
            }

            if (IsRecordToFile == false &&
                recordStatus == RecordStatus.RECORDING)
            {
                WavWriter.Close();
                recordStatus = RecordStatus.STOP;
            }
        }

        void OnAudioFilterRead(float[] data, int channels)
        {
            if (recordStatus == RecordStatus.RECORDING)
            {
                WavWriter.Write(data);
            }
        }

        public void OnValidate()
        {
            Vive3DSPAudio.SetGlobalGain(GlobalGain);
        }

        void OnDisable() {}

        private void OnDestroy()
        {
            WavWriter.Close();
            Vive3DSPAudio.DestroyAudioListener();
        }

        void LateUpdate()
        {
            if (AudioSettings.GetSpatializerPluginName() == "VIVE 3DSP Audio")
            {
                if (!Vive3DSPAudio.IsListenerInsideRoom(gameObject, CurrentRoom))
                {
                    CurrentRoom = Vive3DSPAudio.FindCurrentRoom(this.gameObject);
                }
                Vive3DSPAudio.PassObjToNative();
            }
        }

        public enum RecordStatus
        {
            STOP = 0,
            RECORDING
        }
        private RecordStatus recordStatus = RecordStatus.STOP;

        public bool IsRecordToFile
        {
            get { return isRecordToFile; }
        }
        [SerializeField]
        private bool isRecordToFile = false;

        public string RecordFileName
        {
            set { fname = value; }
            get { return fname; }
        }
        [SerializeField]
        private string fname = "";
        public void SetRecordWavFileName(string name)
        {
            RecordFileName = name;
        }
    }

    public static class WavWriter
    {
        private const int HEADER_SIZE = 44;
        private static FileStream fs = null;
        private static int SCALE_UP_FACTOR = 32767;

        public static void Create(string fileName)
        {
            fs = new FileStream(fileName, FileMode.Create);
            var emptyByte = new byte();
            for(int i = 0; i < HEADER_SIZE; i++)
            {
                fs.WriteByte(emptyByte);
            }
        }
        public static void Close()
        {
            if (fs != null)
            {
                WriteHeader();
                fs.Close();
                fs = null;

                #if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
                #endif
            }
        }

        public static void Write(float[] data)
        {
            if (fs != null)
            {
                Int16[] intData = new Int16[data.Length];
                Byte[] byteData = new Byte[data.Length * 2];
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] > 1.0f) data[i] = 1.0f;
                    if (data[i] < -1.0f) data[i] = -1.0f;

                    intData[i] = (short)(data[i] * SCALE_UP_FACTOR);
                    Byte[] byteArr = new Byte[2];
                    byteArr = BitConverter.GetBytes(intData[i]);
                    byteArr.CopyTo(byteData, i * 2);
                }

                fs.Write(byteData, 0, byteData.Length);
            }
        }

        private static void WriteHeader()
        {
            var sample_rate = AudioSettings.outputSampleRate;
            int channel_num = 0;
            switch (AudioSettings.speakerMode)
            {
                case AudioSpeakerMode.Mono:
                    channel_num = 1;
                    break;
                case AudioSpeakerMode.Stereo:
                    channel_num = 2;
                    break;
                case AudioSpeakerMode.Quad:
                    channel_num = 4;
                    break;
                case AudioSpeakerMode.Surround:
                    channel_num = 5;
                    break;
                case AudioSpeakerMode.Mode5point1:
                    channel_num = 6;
                    break;
                case AudioSpeakerMode.Mode7point1:
                    channel_num = 8;
                    break;
                case AudioSpeakerMode.Prologic:
                    channel_num = 2;
                    break;
                default: // & case AudioSpeakerMode.Raw
                    channel_num = 2;
                    break;
            }
            var sample_num = (fs.Length - 44) / channel_num / 2;

            fs.Seek(0, SeekOrigin.Begin);

            Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
            fs.Write(riff, 0, 4);

            Byte[] chunkSize = BitConverter.GetBytes(fs.Length - 8);
            fs.Write(chunkSize, 0, 4);

            Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
            fs.Write(wave, 0, 4);

            Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
            fs.Write(fmt, 0, 4);

            Byte[] subChunk1 = BitConverter.GetBytes(16);
            fs.Write(subChunk1, 0, 4);

            UInt16 one = 1;
            Byte[] audioFormat = BitConverter.GetBytes(one);
            fs.Write(audioFormat, 0, 2);

            Byte[] numChannels = BitConverter.GetBytes(channel_num);
            fs.Write(numChannels, 0, 2);

            Byte[] sampleRate = BitConverter.GetBytes(sample_rate);
            fs.Write(sampleRate, 0, 4);

            Byte[] byteRate = BitConverter.GetBytes(sample_rate * channel_num * 2);
            fs.Write(byteRate, 0, 4);

            UInt16 bloclAlign = (ushort)(channel_num * 2);
            fs.Write(BitConverter.GetBytes(bloclAlign), 0, 2);

            UInt16 bps = 16;
            Byte[] bitPerSample = BitConverter.GetBytes(bps);
            fs.Write(bitPerSample, 0, 2);

            Byte[] dataString = System.Text.Encoding.UTF8.GetBytes("data");
            fs.Write(dataString, 0, 4);

            Byte[] subChunk2 = BitConverter.GetBytes(sample_num * channel_num * 2);
            fs.Write(subChunk2, 0, 4);
        }
    }
}
