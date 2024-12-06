using HTC.UnityPlugin.Vive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;
using VIVE.OpenXR.SecondaryViewConfiguration;
using VIVE.OpenXR.Toolkits.Spectator.Helper;
using VRSStudio.Common;
using VRSStudio.Common.Input;
using VRSController = VRSStudio.Common.Input.Controller;

public class Capture360Photo : MonoBehaviour
{
    [Tooltip("The camera used to capture 360 photo.  Must not null.")]
    public Camera cam360;
    public LayerMask mask = -1;

    private const float SpectatorCameraStereoSeparationDefault = 0.065f;
    private const int CubemapResolutionDefault = 4096;
    private const int CubemapDepthBufferDefault = 32;
    private const string SavePhoto360AlbumName = "Screenshots";
    [Tooltip("Used to adjsut the result direction.  Should face HMD direction.  Must not null. ")]
    public Transform billboardObj;
    public Transform indicator;
    public Renderer indicatorRenderer;

    public Material matSilver;
    public Material matView;
    public AudioClip shutter;

    bool canCapture360Photo;

    Timer timerView = new Timer(60);

    private void Start()
    {
        GetWriteStoragePermission();
        timerView.Reset();
    }

    static readonly InputUsage usagesCtrl = InputUsage.SecondaryButton;
    readonly VRSController ctrlL = new VRSController(true, usagesCtrl);
    readonly List<InputDevice> inputDevices = new List<InputDevice>();

    private void UpdateInput()
    {
        if (!ctrlL.dev.isValid) InputDeviceTools.GetController(ctrlL, inputDevices);
        if (ctrlL.dev.isValid) InputDeviceTools.UpdateController(ctrlL);
    }

    public IEnumerator Capture()
    {
        yield return StartCoroutine(Capture(billboardObj.position, billboardObj.rotation.eulerAngles.y, mask, TextureProcessHelper.PanoramaType.Monoscopic));
    }

    bool isAnimating = false;
    IEnumerator CaptureAndAnimate()
    {
        if (isAnimating) yield break;
        isAnimating = true;

        // Reset Indicator
        UpdateIndicatorTexture(false);
        indicatorRenderer.transform.eulerAngles = indicator.transform.eulerAngles;

        StartCoroutine("Capture");

        // Animation
        if (indicator == null) yield break;

        if (RobotAssistantManager.Instance)
        {
            RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(shutter, true);
            ViveInput.TriggerHapticVibration(HandRole.LeftHand, 0.5f);
        }

        float angle = 0;

        // Rotate around Z axis for 1 second
        while (angle < 360)
        {
            angle += Time.unscaledDeltaTime * 360;
            indicator.localEulerAngles = Vector3.forward * Mathf.Min(angle, 360);
            yield return null;
        }

        UpdateIndicatorTexture(true);
        timerView.Set();

        indicator.localEulerAngles = Vector3.zero;

        isAnimating = false;
    }

    private void Update()
    {
        UpdateInput();
        if (ctrlL.btnSec.IsDown)
        {
            StartCoroutine(CaptureAndAnimate());
        }
        if (timerView.Check())
        {
            UpdateIndicatorTexture(false);
            timerView.Reset();
        }
    }

    private void UpdateIndicatorTexture(bool view)
    {
        if (view && result360 != null)
        {
            indicatorRenderer.material = matView;
            matView.mainTexture = result360;
        }
        else
        {
            indicatorRenderer.material = matSilver;
        }
    }

    /// <summary>
    /// Get the write external storage permission for 360 capture feature.
    /// Please declare <b>WRITE_EXTERNAL_STORAGE</b> permission <b>AND</b>
    /// flag <b>requestLegacyExternalStorage</b> as <b>true</b> in
    /// AndroidManifest.xml if you call this function.
    /// <para />
    /// IMPORTANT: THIS API SUPPORTS ANDROID API LEVEL <b>29 OR BELOW</b> ONLY.
    /// </summary>
    public void GetWriteStoragePermission()
    {
        //#region Permission request by Wave API

        //string permission = "android.permission.WRITE_EXTERNAL_STORAGE";

        //void PermissionRequestFor360CaptureCallback(List<PermissionManager.RequestResult> results)
        //{
        //	foreach (var item in results)
        //	{
        //		if (item.PermissionName == permission)
        //		{
        //			canCapture360Photo = true;
        //		}

        //		Debug.Log(item.Granted
        //			? $"{item.PermissionName} is granted"
        //			: $"{item.PermissionName} is not granted");
        //	}
        //}
        //var permissionManager = PermissionManager.instance;
        //if (permissionManager == null) return;
        //if (permissionManager.isPermissionGranted(permission))
        //	canCapture360Photo = true;
        //else
        //	permissionManager.requestPermissions(new string[] { permission }, PermissionRequestFor360CaptureCallback);

        //#endregion
        canCapture360Photo = SpectatorCameraBased.IsAllowSpectatorCameraCapture360Image;
    }

    RenderTexture result360 = null;

    public Texture Get360Image()
    {
        return result360;
    }

    bool isGoing = false;
    public IEnumerator Capture(Vector3 pos, float yawRot, LayerMask mask, TextureProcessHelper.PanoramaType panoramaType)
    {
        if (isGoing) yield break;
        isGoing = true;
#if UNITY_ANDROID
        // if I/O external storage is not granted, return
        if (Application.platform == RuntimePlatform.Android && !canCapture360Photo)
        {
            Debug.LogError(
                "Cannot capturing 360 photo due to no permission for I/O external storage, pls make sure the permission is granted.");
            yield break;
        }

        #region Create a new camera component for capture 360 photo

        cam360.transform.position = pos;
        cam360.transform.eulerAngles = Vector3.up * yawRot;
        // set the spectatorCamera360's stereo target eye according to the panorama type
        cam360.stereoTargetEye = panoramaType == TextureProcessHelper.PanoramaType.Stereoscopic
            ? StereoTargetEyeMask.Both
            : StereoTargetEyeMask.None;
        cam360.cullingMask = mask;
        cam360.stereoSeparation = SpectatorCameraStereoSeparationDefault;

        #endregion
        var texEquirect = TextureProcessHelper.Capture360RenderTexture(cam360, CubemapResolutionDefault, panoramaType);

        yield return null;

        if (texEquirect == null)
        {
            Debug.LogError(
                "Capture360RenderTexture return null, pls check the error log on the above for more details");
            yield break;
        }

        RenderTexture texAdjusted = texEquirect;

        // Adjust angle
        if (yawRot != 0)
        {
            texAdjusted = RenderTexture.GetTemporary(texEquirect.width, texEquirect.height, 0, texEquirect.format);
            texEquirect.wrapMode = TextureWrapMode.Repeat;
            Graphics.Blit(texEquirect, texAdjusted, Vector2.one, new Vector2(yawRot / 360.0f, 0));
        }

        yield return null;


        // save 360 photo
        // get the file path first
        string fileRootPath;
        if (!Application.isEditor && Application.platform == RuntimePlatform.Android)
        {
            fileRootPath = IOProcessHelper.GetAndroidExternalStoragePicturesDirectory();
            fileRootPath = Path.Combine(fileRootPath, SavePhoto360AlbumName);
            // make sure the fileRootPath is exist
            Directory.CreateDirectory(fileRootPath);
        }
        else  // Editor Run here
        {
            fileRootPath = Application.persistentDataPath;
            fileRootPath = Path.Combine(fileRootPath, SavePhoto360AlbumName);
            // make sure the fileRootPath is exist
            Directory.CreateDirectory(fileRootPath);
        }

        // init the filename
        string filetitle = $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}";
        Debug.Log($"File will save as {filetitle}.jpg");

        try
        {
            TextureProcessHelper.SaveRenderTextureToDisk("Screenshots", filetitle, TextureProcessHelper.PictureOutputFormat.JPG, texAdjusted);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error on output the panoramic photo: {e}");

        }

        RenderTexture.ReleaseTemporary(texEquirect);
        RenderTexture.ReleaseTemporary(texAdjusted);

        if (result360 == null)
        {
            result360 = RenderTexture.GetTemporary(1024, 512, 0, texEquirect.format);
            result360.wrapMode = TextureWrapMode.Repeat;
        }
        Graphics.Blit(texAdjusted, result360);

        //DestroyImmediate(mat);
#endif

        isGoing = false;
    }

    private void OnDestroy()
    {
        if (result360 != null)
        {
            RenderTexture.ReleaseTemporary(result360);
        }
    }
}



#if UNITY_EDITOR
[CustomEditor(typeof(Capture360Photo))]
public class Capture360PhotoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Capture"))
        {
            Capture360Photo obj = (Capture360Photo)target;
            obj.Capture();
        }
    }
}
#endif