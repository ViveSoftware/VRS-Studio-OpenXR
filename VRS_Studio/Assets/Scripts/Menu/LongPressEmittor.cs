using HTC.UnityPlugin.LiteCoroutineSystem;
using HTC.UnityPlugin.Pointer3D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LongPressEmittor : MonoBehaviour
    , IPointerDownHandler
    , IPointer3DPressExitHandler
{
    [Serializable]
    public class UnityEventFloat : UnityEvent<float> { }
    [Serializable]
    public class UnityEventV3 : UnityEvent<Vector3> { }
    [Serializable]
    public class UnityEventQt : UnityEvent<Quaternion> { }

    [SerializeField]
    private PointerEventData.InputButton button = PointerEventData.InputButton.Left;
    [SerializeField]
    [Range(0.01f, 10f)]
    private float pressDuration = 1f;
    [SerializeField]
    [Range(0.01f, 10f)]
    private float preEmitDuration = 0.1f;

    public UnityEventV3 OnPressLocalPos;
    public UnityEventV3 OnPressWorldPos;
    public UnityEventFloat OnProgress;
    public UnityEvent OnCanceled;
    public UnityEvent OnPreEmit;
    public UnityEventV3 OnEmitLocalPos;
    public UnityEventV3 OnEmitWorldPos;

    private float startPressTime;
    private HashSet<PointerEventData> pressEDs;
    private LiteCoroutine progressCoroutine;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != button) { return; }

        if (pressEDs == null) { pressEDs = new HashSet<PointerEventData>(); }
        if (!pressEDs.Add(eventData)) { return; }

        startPressTime = Time.unscaledTime;
        if (progressCoroutine.IsNullOrDone())
        {
            var pos = eventData.pointerPressRaycast.worldPosition;
            if (OnPressLocalPos != null) { OnPressLocalPos.Invoke(transform.InverseTransformPoint(pos)); }
            if (OnPressWorldPos != null) { OnPressWorldPos.Invoke(pos); }
            LiteCoroutine.StartCoroutine(ref progressCoroutine, ProgressCoroutine(pos), false);
        }
    }

    void IPointer3DPressExitHandler.OnPointer3DPressExit(Pointer3DEventData eventData)
    {
        pressEDs.Remove(eventData);
    }

    private IEnumerator ProgressCoroutine(Vector3 pos)
    {
        do
        {
            var pressSpan = Time.unscaledTime - startPressTime;
            if (pressSpan >= pressDuration) { break; }

            if (pressEDs.Count == 0)
            {
                if (OnCanceled != null) { OnCanceled.Invoke(); }
                yield break;
            }

            if (OnProgress != null) { OnProgress.Invoke(1.1f * pressSpan / pressDuration); }
            yield return null;

        } while (true);

        if (OnPreEmit != null) { OnPreEmit.Invoke(); }

        var startPreEmitTime = startPressTime + pressDuration;
        do
        {
            var preEmitSpan = Time.unscaledTime - startPreEmitTime;
            if (preEmitSpan >= preEmitDuration) { break; }
            yield return null;

        } while (true);

        if (OnEmitLocalPos != null) { OnEmitLocalPos.Invoke(transform.InverseTransformPoint(pos)); }
        if (OnEmitWorldPos != null) { OnEmitWorldPos.Invoke(pos); }
    }
}
