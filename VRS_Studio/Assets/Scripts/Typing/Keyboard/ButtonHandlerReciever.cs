using HTC.UnityPlugin.LiteCoroutineSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HTC.ViveSoftware.ExpLab.HandInteractionDemo
{
    public class ButtonHandlerReciever : MonoBehaviour
    {
        [SerializeField]
        private CustomInputField inputField;
        [SerializeField]
        private ButtonHandlerGroup[] keyboardHandlers;
        [SerializeField]
        private bool activeOnStart;
        [SerializeField]
        private float repeadKeyDelay = 0.5f;
        [SerializeField]
        private float repeatKeyInterval = 0.15f;

        private struct RepeatKey : IEquatable<RepeatKey>
        {
            public string cmd;
            public KeyCode code;
            public float nextPressTime;

            public bool Equals(RepeatKey other) { return code == other.code && cmd == other.cmd; }
            public override bool Equals(object obj) { return obj != null && obj is RepeatKey && Equals((RepeatKey)obj); }
            public override int GetHashCode() { return (cmd, code).GetHashCode(); }
        }

        [Flags]
        private enum Modifier
        {
            LeftShift = 1 << 0,
            RightShift = 1 << 1,
            LeftControl = 1 << 2,
            RightControl = 1 << 3,
            LeftCommand = 1 << 4,
            RightCommand = 1 << 5,
            LeftAlt = 1 << 6,
            RightAlt = 1 << 7,
        }

        private List<ButtonHandlerGroup> registeredHandlers;
        private Modifier modifiers;
        private EventModifiers eventModifiers;
        private List<RepeatKey> repeatKeys = new List<RepeatKey>();
        private LiteCoroutine repeatCoroutine;

        private void Start()
        {
            if (activeOnStart)
            {
                //inputField.ActivateInputField();
                EventSystem.current.SetSelectedGameObject(inputField.gameObject);
            }
        }

        protected virtual void OnEnable()
        {
            if (keyboardHandlers != null && keyboardHandlers.Length > 0)
            {
                if (registeredHandlers == null) { registeredHandlers = new List<ButtonHandlerGroup>(); }
                foreach (var h in keyboardHandlers)
                {
                    if (h != null)
                    {
                        h.OnKeyClick += OnButtonClick;
                        h.OnKeyDown += OnButtonDown;
                        h.OnKeyUp += OnButtonUp;
                        registeredHandlers.Add(h);
                    }
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (registeredHandlers != null && registeredHandlers.Count > 0)
            {
                foreach (var h in registeredHandlers)
                {
                    if (h != null)
                    {
                        h.OnKeyClick -= OnButtonClick;
                        h.OnKeyDown -= OnButtonDown;
                        h.OnKeyUp -= OnButtonUp;
                    }
                }
                registeredHandlers.Clear();
            }

            repeatKeys.Clear();
        }

        private void OnButtonDown(string keyCmd, KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.LeftShift:
                    SetMod(Modifier.LeftShift, true);
                    SetMod(EventModifiers.Shift, true);
                    break;
                case KeyCode.RightShift:
                    SetMod(Modifier.RightShift, true);
                    SetMod(EventModifiers.Shift, true);
                    break;
                case KeyCode.LeftControl:
                    SetMod(Modifier.LeftControl, true);
                    SetMod(EventModifiers.Control, true);
                    break;
                case KeyCode.RightControl:
                    SetMod(Modifier.RightControl, true);
                    SetMod(EventModifiers.Control, true);
                    break;
                case KeyCode.LeftCommand:
                    SetMod(Modifier.LeftCommand, true);
                    SetMod(EventModifiers.Command, true);
                    break;
                case KeyCode.RightCommand:
                    SetMod(Modifier.RightCommand, true);
                    SetMod(EventModifiers.Command, true);
                    break;
                case KeyCode.LeftAlt:
                    SetMod(Modifier.LeftAlt, true);
                    SetMod(EventModifiers.Alt, true);
                    break;
                case KeyCode.RightAlt:
                    SetMod(Modifier.RightAlt, true);
                    SetMod(EventModifiers.Alt, true);
                    break;
                case KeyCode.Numlock:
                    SetMod(EventModifiers.Numeric, true);
                    break;
                case KeyCode.CapsLock:
                    SetMod(EventModifiers.CapsLock, true);
                    break;
                default:
                    if (string.IsNullOrEmpty(keyCmd)) { break; }
                    if (keyCode >= KeyCode.F1 && keyCode <= KeyCode.F15) { break; }

                    var cmd = AnyModSet(EventModifiers.Shift) ? keyCmd.ToUpper() : keyCmd.ToLower();

                    if (keyCode == KeyCode.None)
                    {
                        inputField.EnterKeyCommand(cmd, eventModifiers);
                    }
                    else
                    {
                        AddRepeatKey(keyCmd, keyCode);
                        inputField.EnterKeyCode(cmd[0], keyCode, eventModifiers);
                    }
                    break;
            }
        }

        private void OnButtonUp(string keyCmd, KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.LeftShift:
                    SetMod(Modifier.LeftShift, false);
                    SetMod(EventModifiers.Shift, AnyModSet(Modifier.RightShift));
                    break;
                case KeyCode.RightShift:
                    SetMod(Modifier.RightShift, false);
                    SetMod(EventModifiers.Shift, AnyModSet(Modifier.LeftShift));
                    break;
                case KeyCode.LeftControl:
                    SetMod(Modifier.LeftControl, false);
                    SetMod(EventModifiers.Control, AnyModSet(Modifier.RightControl));
                    break;
                case KeyCode.RightControl:
                    SetMod(Modifier.RightControl, false);
                    SetMod(EventModifiers.Control, AnyModSet(Modifier.LeftControl));
                    break;
                case KeyCode.LeftCommand:
                    SetMod(Modifier.LeftCommand, false);
                    SetMod(EventModifiers.Command, AnyModSet(Modifier.RightCommand));
                    break;
                case KeyCode.RightCommand:
                    SetMod(Modifier.RightCommand, false);
                    SetMod(EventModifiers.Command, AnyModSet(Modifier.LeftCommand));
                    break;
                case KeyCode.LeftAlt:
                    SetMod(Modifier.LeftAlt, false);
                    SetMod(EventModifiers.Alt, AnyModSet(Modifier.RightAlt));
                    break;
                case KeyCode.RightAlt:
                    SetMod(Modifier.RightAlt, false);
                    SetMod(EventModifiers.Alt, AnyModSet(Modifier.LeftAlt));
                    break;
                case KeyCode.Numlock:
                    SetMod(EventModifiers.Numeric, false);
                    break;
                case KeyCode.CapsLock:
                    SetMod(EventModifiers.CapsLock, false);
                    break;
                default:
                    if (string.IsNullOrEmpty(keyCmd)) { break; }
                    if (keyCode >= KeyCode.F1 && keyCode <= KeyCode.F15) { break; }

                    RemoveRepeatKey(keyCmd, keyCode);
                    break;
            }
        }

        private void OnButtonClick(string keyCmd, KeyCode keyCode)
        {
        }

        private bool AnyModSet(Modifier mod)
        {
            return (modifiers & mod) > 0;
        }

        private void SetMod(Modifier mod, bool value)
        {
            if (value) { modifiers |= mod; }
            else { modifiers &= ~mod; }
        }

        private bool AnyModSet(EventModifiers mod)
        {
            return (eventModifiers & mod) > 0;
        }

        private void SetMod(EventModifiers mod, bool value)
        {
            if (value) { eventModifiers |= mod; }
            else { eventModifiers &= ~mod; }
        }

        private void AddRepeatKey(string cmd, KeyCode code)
        {
            repeatKeys.Add(new RepeatKey() { cmd = cmd, code = code, nextPressTime = Time.unscaledTime + repeadKeyDelay });
            //Debug.Log("AddRepeatKey " + cmd + " " + code);
            if (repeatCoroutine.IsNullOrDone())
            {
                LiteCoroutine.StartCoroutine(ref repeatCoroutine, RepeatKeyCoroutine());
            }
        }

        private void RemoveRepeatKey(string cmd, KeyCode code)
        {
            //Debug.Log("RemoveRepeatKey " + cmd + " " + code);
            repeatKeys.Remove(new RepeatKey() { cmd = cmd, code = code });
            //var i = repeatKeys.IndexOf(new RepeatKey() { cmd = cmd, code = code });
            //if (i >= 0) { repeatKeys.RemoveAt(i); }
        }

        private IEnumerator RepeatKeyCoroutine()
        {
            yield return null;

            while (repeatKeys.Count > 0)
            {
                var now = Time.unscaledTime;
                for (int i = repeatKeys.Count - 1; i >= 0; --i)
                {
                    var rk = repeatKeys[i];
                    var cmd = AnyModSet(EventModifiers.Shift) ? rk.cmd.ToUpper() : rk.cmd.ToLower();
                    while (now > rk.nextPressTime)
                    {
                        inputField.EnterKeyCode(cmd[0], rk.code, eventModifiers);
                        rk.nextPressTime += repeatKeyInterval;
                    }
                    repeatKeys[i] = rk;
                }

                yield return null;
            }
        }
    }
}