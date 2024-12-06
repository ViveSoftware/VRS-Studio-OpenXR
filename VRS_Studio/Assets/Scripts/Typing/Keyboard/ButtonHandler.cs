using HTC.UnityPlugin.LiteCoroutineSystem;
using HTC.UnityPlugin.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HTC.ViveSoftware.ExpLab.HandInteractionDemo
{
    public class ButtonHandler : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
    , IPointerDownHandler
    , IPointerUpHandler
    , IPointerClickHandler
    {
        [SerializeField]
        private ButtonHandlerGroup group;
        [SerializeField]
        private AudioSource mySounds;
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("hoverSound")]
        private AudioClip buttonHoverClip;
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("clickSound")]
        private AudioClip buttonDownClip;
        [SerializeField]
        private AudioClip buttonUpClip;
        [SerializeField]
        private Text text;
        [SerializeField]
        private KeyCode keyCode;

        private HashSet<PointerEventData> hovers = new HashSet<PointerEventData>();
        private HashSet<PointerEventData> presses = new HashSet<PointerEventData>();
        private LiteCoroutine clickAnim;
        private float pressTime;
        private float releaseTime;

        private static EnumArray<KeyCode, string> keyCodeChar = new EnumArray<KeyCode, string>();
        private class KeyCodeToIntResolver : EnumToIntResolver<KeyCode> { public override int Resolve(KeyCode e) { return (int)e; } }

        static ButtonHandler()
        {
            keyCodeChar[KeyCode.A] = "a";
            keyCodeChar[KeyCode.B] = "b";
            keyCodeChar[KeyCode.C] = "c";
            keyCodeChar[KeyCode.D] = "d";
            keyCodeChar[KeyCode.E] = "e";
            keyCodeChar[KeyCode.F] = "f";
            keyCodeChar[KeyCode.G] = "g";
            keyCodeChar[KeyCode.H] = "h";
            keyCodeChar[KeyCode.I] = "i";
            keyCodeChar[KeyCode.J] = "j";
            keyCodeChar[KeyCode.K] = "k";
            keyCodeChar[KeyCode.L] = "l";
            keyCodeChar[KeyCode.M] = "m";
            keyCodeChar[KeyCode.N] = "n";
            keyCodeChar[KeyCode.O] = "o";
            keyCodeChar[KeyCode.P] = "p";
            keyCodeChar[KeyCode.Q] = "q";
            keyCodeChar[KeyCode.R] = "r";
            keyCodeChar[KeyCode.S] = "s";
            keyCodeChar[KeyCode.T] = "t";
            keyCodeChar[KeyCode.U] = "u";
            keyCodeChar[KeyCode.V] = "v";
            keyCodeChar[KeyCode.W] = "w";
            keyCodeChar[KeyCode.X] = "x";
            keyCodeChar[KeyCode.Y] = "y";
            keyCodeChar[KeyCode.Z] = "z";

            keyCodeChar[KeyCode.Keypad1] = "1";
            keyCodeChar[KeyCode.Keypad2] = "2";
            keyCodeChar[KeyCode.Keypad3] = "3";
            keyCodeChar[KeyCode.Keypad4] = "4";
            keyCodeChar[KeyCode.Keypad5] = "5";
            keyCodeChar[KeyCode.Keypad6] = "6";
            keyCodeChar[KeyCode.Keypad7] = "7";
            keyCodeChar[KeyCode.Keypad8] = "8";
            keyCodeChar[KeyCode.Keypad9] = "9";
            keyCodeChar[KeyCode.Keypad0] = "0";

            keyCodeChar[KeyCode.Exclaim] = "!"; //1
            keyCodeChar[KeyCode.DoubleQuote] = "\"";
            keyCodeChar[KeyCode.Hash] = "#"; //3
            keyCodeChar[KeyCode.Dollar] = "$"; //4
            keyCodeChar[KeyCode.Ampersand] = "&"; //7
            keyCodeChar[KeyCode.Quote] = "\'"; //remember the special forward slash rule... this isnt wrong
            keyCodeChar[KeyCode.LeftParen] = "("; //9
            keyCodeChar[KeyCode.RightParen] = ")"; //0
            keyCodeChar[KeyCode.Asterisk] = "*"; //8
            keyCodeChar[KeyCode.Plus] = "+";
            keyCodeChar[KeyCode.Comma] = ",";
            keyCodeChar[KeyCode.Minus] = "-";
            keyCodeChar[KeyCode.Period] = ".";
            keyCodeChar[KeyCode.Slash] = "/";
            keyCodeChar[KeyCode.Colon] = ":";
            keyCodeChar[KeyCode.Semicolon] = ";";
            keyCodeChar[KeyCode.Less] = "<";
            keyCodeChar[KeyCode.Equals] = "=";
            keyCodeChar[KeyCode.Greater] = ">";
            keyCodeChar[KeyCode.Question] = "?";
            keyCodeChar[KeyCode.At] = "@"; //2
            keyCodeChar[KeyCode.LeftBracket] = "[";
            keyCodeChar[KeyCode.Backslash] = "\\"; //remember the special forward slash rule... this isnt wrong
            keyCodeChar[KeyCode.RightBracket] = "]";
            keyCodeChar[KeyCode.Caret] = "^"; //6
            keyCodeChar[KeyCode.Underscore] = "_";
            keyCodeChar[KeyCode.BackQuote] = "`";
            keyCodeChar[KeyCode.Tilde] = "~";
            keyCodeChar[KeyCode.Return] = "\n";
            keyCodeChar[KeyCode.Tab] = "\t";

            keyCodeChar[KeyCode.Alpha1] = "1";
            keyCodeChar[KeyCode.Alpha2] = "2";
            keyCodeChar[KeyCode.Alpha3] = "3";
            keyCodeChar[KeyCode.Alpha4] = "4";
            keyCodeChar[KeyCode.Alpha5] = "5";
            keyCodeChar[KeyCode.Alpha6] = "6";
            keyCodeChar[KeyCode.Alpha7] = "7";
            keyCodeChar[KeyCode.Alpha8] = "8";
            keyCodeChar[KeyCode.Alpha9] = "9";
            keyCodeChar[KeyCode.Alpha0] = "0";

            keyCodeChar[KeyCode.KeypadPeriod] = ".";
            keyCodeChar[KeyCode.KeypadDivide] = "/";
            keyCodeChar[KeyCode.KeypadMultiply] = "*";
            keyCodeChar[KeyCode.KeypadMinus] = "-";
            keyCodeChar[KeyCode.KeypadPlus] = "+";
            keyCodeChar[KeyCode.KeypadEquals] = "=";
            keyCodeChar[KeyCode.KeypadEnter] = "\n";
        }

#if UNITY_EDITOR
        [ContextMenu("Replace With LensUIButton")]
        private void ReplaceWithLensUIButton()
        {
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                var oldBtn = GetComponent<Button>();
                if (oldBtn != null)
                {
                    var compEnabled = oldBtn.enabled;
                    var interactable = oldBtn.interactable;
                    var transition = oldBtn.transition;
                    var targetGraphic = oldBtn.targetGraphic;
                    var spriteState = oldBtn.spriteState;
                    var colors = oldBtn.colors;
                    var navigation = oldBtn.navigation;
                    var onClick = oldBtn.onClick;

                    DestroyImmediate(oldBtn);
                    var newBtn = gameObject.AddComponent<Triton.LensUI.LensUIButton>();

                    newBtn.enabled = compEnabled;
                    newBtn.interactable = interactable;
                    newBtn.transition = transition;
                    newBtn.targetGraphic = targetGraphic;
                    newBtn.spriteState = spriteState;
                    newBtn.colors = colors;
                    newBtn.navigation = navigation;
                    newBtn.onClick = onClick;

                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
                }
            }
        }
        [ContextMenu("Replace With UGUIButton")]
        private void ReplaceWithUGUIButton()
        {
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                var oldBtn = GetComponent<Triton.LensUI.LensUIButton>();
                if (oldBtn != null)
                {
                    var compEnabled = oldBtn.enabled;
                    var interactable = oldBtn.interactable;
                    var transition = oldBtn.transition;
                    var targetGraphic = oldBtn.targetGraphic;
                    var spriteState = oldBtn.spriteState;
                    var colors = oldBtn.colors;
                    var navigation = oldBtn.navigation;
                    var onClick = oldBtn.onClick;

                    DestroyImmediate(oldBtn);
                    var newBtn = gameObject.AddComponent<Button>();

                    newBtn.enabled = compEnabled;
                    newBtn.interactable = interactable;
                    newBtn.transition = transition;
                    newBtn.targetGraphic = targetGraphic;
                    newBtn.spriteState = spriteState;
                    newBtn.colors = colors;
                    newBtn.navigation = navigation;
                    newBtn.onClick = onClick;

                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
                }
            }
        }
#endif
        [ContextMenu("Find Group")]
        public void FindGroupInParent()
        {
            group = GetComponentInParent<ButtonHandlerGroup>();
        }

        private void Start()
        {
            if (group == null) { FindGroupInParent(); }
            if (mySounds == null) { mySounds = GetComponentInChildren<AudioSource>(true); }
            if (text == null) { text = GetComponentInChildren<Text>(true); }
        }

        private void OnDestroy()
        {
            group = null;
        }

        public void HoverSound()
        {
            var clip = buttonHoverClip;
            if (clip == null && group != null) { clip = group.KeyHoveredClip; }
            PlayValidClip(clip);
        }

        public void KeyDownSound()
        {
            var clip = buttonDownClip;
            if (clip == null && group != null) { clip = group.KeyDownClip; }
            PlayValidClip(clip);
        }

        public void KeyUpSound()
        {
            var clip = buttonUpClip;
            if (clip == null && group != null) { clip = group.KeyUpClip; }
            PlayValidClip(clip);
        }

        private void PlayValidClip(AudioClip clip)
        {
            if (clip != null) { mySounds.PlayOneShot(clip); }
        }

        public void SetText(string str)
        {
            text.text = str;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (hovers.Add(eventData) && hovers.Count == 1)
            {
                HoverSound();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (hovers.Remove(eventData) && hovers.Count == 0)
            {
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (group != null)
            {
                group.NotifyKeyClick(text.text, keyCode);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (presses.Add(eventData) && presses.Count == 1)
            {
                if (group != null)
                {
                    var c = keyCodeChar[keyCode];
                    group.NotifyKeyDown(string.IsNullOrEmpty(c) ? text.text : c, keyCode);
                }

                KeyDownSound();

                pressTime = Time.time;

                if (group != null && clickAnim.IsNullOrDone())
                {
                    LiteCoroutine.StartCoroutine(ref clickAnim, PressAnim(
                        group.ButtonPressAnimCurve,
                        group.ButtonReleaseAnimCurve,
                        group.ButtonPressDuration,
                        group.ButtonReleaseDuration,
                        group.PressedDepth));
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (presses.Remove(eventData) && presses.Count == 0)
            {
                if (group != null)
                {
                    var c = keyCodeChar[keyCode];
                    group.NotifyKeyUp(string.IsNullOrEmpty(c) ? text.text : c, keyCode);
                }

                KeyUpSound();

                releaseTime = Time.time;
            }
        }

        private struct AnimatedObj
        {
            public Transform obj;
            public Vector3 initPos;
        }

        private List<AnimatedObj> animatingObjs;
        private IEnumerator PressAnim(AnimationCurve pressCurve, AnimationCurve releaseCurve, float pressDuration, float releaseDuration, float pressDepth)
        {
            if (transform.childCount == 0) { yield break; }

            animatingObjs = ListPool<AnimatedObj>.Get();
            for (var i = transform.childCount - 1; i >= 0; --i)
            {
                var obj = transform.GetChild(i);
                animatingObjs.Add(new AnimatedObj()
                {
                    obj = obj,
                    initPos = obj.localPosition,
                });
            }

            float value;
            while (true)
            {
                var now = Time.time;
                if (pressTime > releaseTime)
                {
                    var curveTime = (now - pressTime) / pressDuration;

                    if (pressTime - releaseTime < releaseDuration)
                    {
                        // adding previous progress left
                        curveTime += 1f - (pressTime - releaseTime) / releaseDuration;
                    }

                    value = curveTime < 1f ? pressCurve.Evaluate(curveTime) : 1f;
                }
                else
                {
                    var curveTime = (now - releaseTime) / releaseDuration;

                    if (releaseTime - pressTime < pressDuration)
                    {
                        // adding previous progress left
                        curveTime += 1f - (releaseTime - pressTime) / pressDuration;
                    }

                    value = curveTime < 1f ? (1f - releaseCurve.Evaluate(curveTime)) : 0f;
                }

                foreach (var animObj in animatingObjs)
                {
                    var pos = animObj.initPos;
                    pos.z += pressDepth * value;
                    animObj.obj.localPosition = pos;
                }

                if (now >= pressTime + pressDuration && now >= releaseTime + releaseDuration && presses.Count == 0)
                {
                    yield break;
                }

                yield return null;
            }
        }
    }
}