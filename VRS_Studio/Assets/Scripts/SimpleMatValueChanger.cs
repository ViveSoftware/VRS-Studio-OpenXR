#pragma warning disable 0649
using HTC.UnityPlugin.LiteCoroutineSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.ViveSoftware.ExpLab.HandInteractionDemo
{
    public class SimpleMatValueChanger : MonoBehaviour
    {
        private static readonly string LOG_PREFIX = "[" + typeof(SimpleMatValueChanger).Name + "] ";

        [SerializeField]
        private Renderer m_targetRenderer;
        [SerializeField]
        private string m_propName;
        [SerializeField]
        private float m_smoothTime = 0.15f;
        [SerializeField]
        private float[] m_presetValues;
        [SerializeField]
        private UnityEvent m_onStart;

        private LiteCoroutine m_animCoroutine;
        private int m_targetPropID;
        private Material m_targetMaterial;
        private float m_propVelocity;

        public Renderer targetRenderer { get { return m_targetRenderer; } set { m_targetRenderer = value; } }

        public string propName { get { return m_propName; } set { m_propName = value; } }

        public float smoothTime { get { return m_smoothTime; } set { m_smoothTime = value; } }

#if UNITY_EDITOR
        private void Reset()
        {
            m_targetRenderer = GetComponent<Renderer>();
        }
#endif

        private void Start()
        {
            if (m_onStart != null) { m_onStart.Invoke(); }
        }

        public void SnapToValue(int index)
        {
            SnapToValue(m_presetValues[index]);
        }

        public void SnapToValue(float value)
        {
            LiteCoroutine.StopCoroutine(m_animCoroutine);
            if (ValidatePropID())
            {
                m_targetMaterial.SetFloat(m_targetPropID, value);
            }
        }

        public void SmoothToPresetValue(int index)
        {
            SmoothToValue(m_presetValues[index]);
        }

        public void SmoothToValue(float value)
        {
            TryStartAnim(value);
        }

        private bool TryStartAnim(float value)
        {
            if (!ValidatePropID()) { return false; }
            if (m_targetMaterial.GetFloat(m_targetPropID) == value) { return false; }
            LiteCoroutine.StartCoroutine(ref m_animCoroutine, AnimCoroutine(value));
            return true;
        }

        private bool ValidatePropID()
        {
            if (m_targetRenderer == null) { Debug.LogWarning(LOG_PREFIX + "ValidatePropID invalid renderer"); return false; }
            if (string.IsNullOrWhiteSpace(m_propName)) { Debug.LogWarning(LOG_PREFIX + "ValidatePropID invalid renderer"); return false; }

            var mat = m_targetRenderer.material;
            if (mat == null) { Debug.LogWarning(LOG_PREFIX + "ValidatePropID invalid materail"); return false; }

            var id = Shader.PropertyToID(m_propName);
            if (!mat.HasProperty(id)) { Debug.LogWarning(LOG_PREFIX + "ValidatePropID invalid prop name"); return false; }

            var targetChanged = false;
            if (m_targetPropID != id) { m_targetPropID = id; targetChanged = true; }
            if (m_targetMaterial != mat) { m_targetMaterial = mat; targetChanged = true; }
            if (targetChanged) { m_propVelocity = 0f; }
            return true;
        }

        private IEnumerator AnimCoroutine(float value)
        {
            float current;
            yield return null;
            while (!Mathf.Approximately(current = m_targetMaterial.GetFloat(m_targetPropID), value))
            {
                current = Mathf.SmoothDamp(current, value, ref m_propVelocity, m_smoothTime);
                m_targetMaterial.SetFloat(m_targetPropID, current);
                yield return null;
            }

            m_targetMaterial.SetFloat(m_targetPropID, value);
        }
    }
}