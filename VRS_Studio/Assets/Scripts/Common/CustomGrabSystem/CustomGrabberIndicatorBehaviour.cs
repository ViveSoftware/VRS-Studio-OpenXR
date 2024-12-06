using UnityEngine;
using VRSStudio.Common;

namespace VRSStudio.Input.CustomGrab
{
    public class CustomGrabberIndicatorBehaviour : MonoBehaviour
    {
        public Renderer indicator;
        public Material indicatorMat;

        // According to the distance to nearest grabbable object.  Modify the material's alpha.  If close to object, show.  If away, hide.
        public float fadeShowDist = 0.5f;
        public float fadeGoneDist = 1f;

        // Only effect when indicator is not null
        public Color colorEmpty = new Color(0, 0.8f, 0.8f, 0.2f);
        public Color colorHover = new Color(0, 0.8f, 0.8f, 0.5f);
        public Color colorGrabbed = new Color(0.65f, 0.75f, 0.8f, 0.2f);
        public Color colorAway = new Color(0, 0.8f, 0.8f, 0.0f);

        public bool isGrabbed = false;
        public bool isGrabble = false;
        public float show = 0;

        public void OnGrab(CustomGrabber grabber)
        {
            isGrabbed = true;
            UpdateIndicatorMat();
        }

        public void OnRelease(CustomGrabber grabber)
        {
            isGrabbed = false;
            UpdateIndicatorMat();
        }

        public void OnAvailable(CustomGrabber grabber, bool available)
        {
            isGrabble = available;
            UpdateIndicatorMat();
        }

        Timer timer = new Timer(0.1f);

        void Start()
        {
            if (indicator == null) return;
            if (indicatorMat == null && indicator.material.HasColor("_Color"))
            {
                indicatorMat = indicator.material;
            }
            timer.Set();
            UpdateDistance();
        }

        void Update()
        {
            if (timer.Check())
            {
                timer.Set();
                if (isGrabbed || isGrabble) return;
                var oldShow = show;
                UpdateDistance();
                if (oldShow != show)
                    UpdateIndicatorMat();
            }
        }

        private void UpdateDistance()
        {
            CustomGrabbable.GetNearestGrabbable(transform.position, out float dist);
            if (dist < fadeShowDist)
            {
                show = 1;
            }
            else if (dist > fadeGoneDist)
            {
                show = 0;
            }
            else
            {
                if (fadeGoneDist - fadeShowDist <= 0)
                    show = 0;
                else
                    show = 1 - (dist - fadeShowDist) / (fadeGoneDist - fadeShowDist);
            }
        }

        private void UpdateIndicatorMat()
        {
            if (indicatorMat == null) return;
            if (isGrabbed)
                indicatorMat.SetColor("_Color", colorGrabbed);
            else if (isGrabble)
                indicatorMat.SetColor("_Color", colorEmpty);
            else
            {
                var fade = Color.Lerp(colorAway, colorHover, show);
                indicatorMat.SetColor("_Color", fade);
            }
        }

        private void OnValidate()
        {
            fadeShowDist = Mathf.Min(fadeGoneDist, fadeShowDist);
        }
    }
}