using HTC.UnityPlugin.PoseTracker;
using UnityEngine;

public class PoseFreezerController : MonoBehaviour
{
    public GameObject poseFreezer;
    public GameObject canvasObj;

    private void Update()
    {
        if (canvasObj == null) return;

        if (canvasObj.activeSelf) EnablePoseFreezer();
        else DisablePoseFreezer();
    }

    public void EnablePoseFreezer()
    {
        var pf = poseFreezer.GetComponent<PoseFreezer>();
        pf.enabled = true;
    }

    public void DisablePoseFreezer()
    {
        var pf = poseFreezer.GetComponent<PoseFreezer>();
        pf.enabled = false;
    }
}