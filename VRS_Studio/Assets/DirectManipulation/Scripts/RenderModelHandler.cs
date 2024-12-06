using System.Collections.Generic;
using UnityEngine;
using VRSStudio.Avatar;

public class RenderModelHandler : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> renderGameobjs = new List<GameObject>();
    private List<SkinnedMeshRenderer> skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
    private bool lastState = false;

    private void OnEnable()
    {
        for (int i = 0; i < renderGameobjs.Count; i++)
        {
            GameObject obj = renderGameobjs[i];
            SkinnedMeshRenderer meshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
            if (meshRenderer != null && !skinnedMeshRenderers.Contains(meshRenderer))
            {
                skinnedMeshRenderers.Add(meshRenderer);
            }
        }
    }

    private void Update()
    {
        EnableRender(ShouldDemoHand());
    }

    private bool ShouldDemoHand()
    {
        if (VRSBodyTrackingManager.Instance && VRSBodyTrackingManager.Instance.IsTracking())
        {
            if (HandInteractionHandler.Instance)
            {
                return HandInteractionHandler.Instance.IsHandDemoNeeded();
            }
        }
        return true;
    }


    private void EnableRender(bool enable)
    {
        for (int i = 0; i < skinnedMeshRenderers.Count; i++)
        {
            if (skinnedMeshRenderers[i].enabled != enable)
            {
                skinnedMeshRenderers[i].enabled = enable;
            }
        }
    }
}
