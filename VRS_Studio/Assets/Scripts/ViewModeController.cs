using HTC.UnityPlugin.Vive;
using System.Collections;
using UnityEngine;
using VIVE.OpenXR.SecondaryViewConfiguration;
using VRSStudio.Spectator;

public class ViewModeController : MonoBehaviour
{
    public Transform labelOrigin, lineOrigin, lineEnd;
    public TextMesh text;

    private ViveRoleProperty viveRole = ViveRoleProperty.New(ControllerRole.RightHand);
    private SpectatorMode currMode;
    private SpectatorMode prevMode = SpectatorMode.ManualPose;
    private bool isSpectatorStarted = false;

    void OnEnable()
    {
        StartCoroutine(SetCallback());
    }

    void OnDisable()
    {
        var sh = SpectatorCameraBased.Instance;

        if (sh)
        {
            sh.OnSpectatorStart -= OnSpectatorStart;
            sh.OnSpectatorStop -= OnSpectatorStop;
        }
    }

    void Update()
    {
        var deviceIndex = viveRole.GetDeviceIndex();
        var isValid = VivePose.IsValid(deviceIndex);

        labelOrigin.gameObject.SetActive(isSpectatorStarted && isValid);
        lineOrigin.gameObject.SetActive(isSpectatorStarted && isValid);
        lineEnd.gameObject.SetActive(isSpectatorStarted && isValid);

        if (!isSpectatorStarted) return;

        currMode = VRSSpectatorManager.Instance.spectatorMode;
        if (currMode != prevMode) text.text = currMode + " Mode";
        prevMode = currMode;
    }

    IEnumerator SetCallback()
    {
        yield return new WaitUntil(() => SpectatorCameraBased.Instance != null);
        var sh = SpectatorCameraBased.Instance;

        sh.OnSpectatorStart += OnSpectatorStart;
        sh.OnSpectatorStop += OnSpectatorStop;
    }

    private void OnSpectatorStart()
    {
        isSpectatorStarted = true;
        labelOrigin.gameObject.SetActive(true);
        lineOrigin.gameObject.SetActive(true);
        lineEnd.gameObject.SetActive(true);
    }

    private void OnSpectatorStop()
    {
        isSpectatorStarted = false;
        labelOrigin.gameObject.SetActive(false);
        lineOrigin.gameObject.SetActive(false);
        lineEnd.gameObject.SetActive(false);
    }
}
