/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRSStudio.Avatar;
using VRSStudio.Spectator;

namespace FancyScrollView.Example03
{
    public class MenuPresenter : MonoBehaviour
    {
        public enum MenuCategory
        {
            Main,
            Spectator,
            Bodytracking,
            //PalmMap,
            Teleport,
            Menu,
            Invalid,
        }

        private static List<ItemData> mainItemData = new List<ItemData>();
        private static MenuCategory lastModifiedType = MenuCategory.Invalid;
        private static int prevSelectedIndex = 0;

        List<ItemData> spectatorItemData = new List<ItemData>();
        List<ItemData> bodytrackingItemData = new List<ItemData>();
        //List<ItemData> palmMapItemData = new List<ItemData>();
        List<ItemData> teleportItemData = new List<ItemData>();
        List<ItemData> menuItemData = new List<ItemData>();

        [SerializeField] ScrollView scrollView = default;
        [SerializeField] Scroller scroller = default;
        public AudioClip tutorial;

        private static MenuPresenter instance;
        public static MenuPresenter Instance { get { return instance; } private set { instance = value; } }
        private const int MAIN_MENU_MAX_COUNT = 5;

        private bool prevTracking = false;
        private string tutorialName = string.Empty;
        private bool addTutorialItem = false;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            //mainItemData.Add(new ItemData("Palm Map >", ItemData.MenuType.Main));
            // If mainItemData size changes, please update MAIN_MENU_MAX_COUNT too
            mainItemData.Add(new ItemData("Menu >", ItemData.MenuType.Main));
            mainItemData.Add(new ItemData("Teleport >", ItemData.MenuType.Main));
            mainItemData.Add(new ItemData("Bodytracking >", ItemData.MenuType.Main));
            mainItemData.Add(new ItemData("Spectator >", ItemData.MenuType.Main));
            mainItemData.Add(new ItemData("Reset Tutorial", ItemData.MenuType.Main));

            spectatorItemData.Add(new ItemData("< Spectator", ItemData.MenuType.Sub));
            if (VRSSpectatorManager.Instance)
            {
                spectatorItemData.Add(new ItemData("View: " + VRSSpectatorManager.Instance.spectatorMode.ToString(), ItemData.MenuType.Sub));
            }
            else
            {
                spectatorItemData.Add(new ItemData("View: Headset", ItemData.MenuType.Sub));
            }
            spectatorItemData.Add(new ItemData("Panorama", ItemData.MenuType.Sub));
            spectatorItemData.Add(new ItemData("Tutorial", ItemData.MenuType.Sub));
            bodytrackingItemData.Add(new ItemData("< Bodytracking", ItemData.MenuType.Sub));
            bodytrackingItemData.Add(new ItemData("Bodytracking: OFF", ItemData.MenuType.Sub));
            bodytrackingItemData.Add(new ItemData("Tutorial", ItemData.MenuType.Sub));
            //palmMapItemData.Add(new ItemData("< Palm Map", ItemData.MenuType.Sub));
            //palmMapItemData.Add(new ItemData("Tutorial", ItemData.MenuType.Sub));
            teleportItemData.Add(new ItemData("< Teleport", ItemData.MenuType.Sub));
            teleportItemData.Add(new ItemData("Tutorial", ItemData.MenuType.Sub));
            menuItemData.Add(new ItemData("< Menu", ItemData.MenuType.Sub));
            menuItemData.Add(new ItemData("Tutorial", ItemData.MenuType.Sub));

            StartCoroutine(WaitForUpdate());
        }

        private void OnEnable()
        {
            if (lastModifiedType == MenuCategory.Invalid) return;
            UpdateData();
        }

        private void OnDisable()
        {
            if (lastModifiedType == MenuCategory.Invalid) return;
            UpdateData();
        }

        private void LateUpdate()
        {
            if (lastModifiedType == MenuCategory.Invalid) return;
            UpdateData();
        }

        private IEnumerator WaitForUpdate()
        {
            yield return new WaitUntil(() => scrollView != null);
            scrollView.UpdateData(mainItemData.ToArray());
            scrollView.SelectCell(0);
        }

        private void UpdateData()
        {
            switch (lastModifiedType)
            {
                case MenuCategory.Main:
                    if (!transform.gameObject.activeSelf) return;
                    scrollView.UpdateData(mainItemData.ToArray());
                    if (prevSelectedIndex > (mainItemData.Count - 1)
                        || scroller.Position > (mainItemData.Count - 1))
                    {
                        scrollView.SelectCell(mainItemData.Count - 1);
                        scroller.Position = mainItemData.Count - 1;
                        prevSelectedIndex = mainItemData.Count - 1;
                    }
                    else
                    {
                        scrollView.SelectCell(prevSelectedIndex);
                        scroller.Position = prevSelectedIndex;
                    }
                    lastModifiedType = MenuCategory.Invalid;
                    break;
                case MenuCategory.Spectator:
                    scrollView.UpdateData(spectatorItemData.ToArray());
                    scrollView.SelectCell(1);
                    scroller.Position = 1;
                    lastModifiedType = MenuCategory.Invalid;
                    break;
                case MenuCategory.Bodytracking:
                    scrollView.UpdateData(bodytrackingItemData.ToArray());
                    scrollView.SelectCell(1);
                    lastModifiedType = MenuCategory.Invalid;
                    break;
                //case MenuCategory.PalmMap:
                //    scrollView.UpdateData(palmMapItemData.ToArray());
                //    scrollView.SelectCell(1);
                //    lastModifiedType = MenuCategory.Invalid;
                //    break;
                case MenuCategory.Teleport:
                    scrollView.UpdateData(teleportItemData.ToArray());
                    scrollView.SelectCell(1);
                    lastModifiedType = MenuCategory.Invalid;
                    break;
                case MenuCategory.Menu:
                    scrollView.UpdateData(menuItemData.ToArray());
                    scrollView.SelectCell(1);
                    lastModifiedType = MenuCategory.Invalid;
                    break;
            }
        }

        public void AddSpectatorResetCameraViewItem()
        {
            lastModifiedType = MenuCategory.Spectator;
            spectatorItemData.Insert(3, new ItemData("Reset Camera", ItemData.MenuType.Sub));
        }

        public void RemoveSpectatorResetCameraViewItem()
        {
            lastModifiedType = MenuCategory.Spectator;
            spectatorItemData.RemoveAt(3);
        }

        public void UpdateSpectatorViewItem(string msg)
        {
            lastModifiedType = MenuCategory.Spectator;
            if (spectatorItemData.Count == 0)
            {
                spectatorItemData.Add(new ItemData("< Spectator", ItemData.MenuType.Sub));
                spectatorItemData.Add(new ItemData("View: Headset", ItemData.MenuType.Sub));
                spectatorItemData.Add(new ItemData("Panorama", ItemData.MenuType.Sub));
                spectatorItemData.Add(new ItemData("Tutorial", ItemData.MenuType.Sub));
            }
            spectatorItemData.Insert(1, new ItemData(msg, ItemData.MenuType.Sub));
            spectatorItemData.RemoveAt(2);
        }

        public void ShowSubmenu()
        {
            if (scrollView.GetSelectedItemData().Message.Contains("Spectator"))
            {
                lastModifiedType = MenuCategory.Spectator;
            }
            else if (scrollView.GetSelectedItemData().Message.Contains("Bodytracking"))
            {
                lastModifiedType = MenuCategory.Bodytracking;
            }
            //else if (scrollView.GetSelectedItemData().Message.Contains("Palm Map"))
            //{
            //    prevSelectedIndex = scrollView.GetSelectedIndex();
            //    lastModifiedType = MenuCategory.PalmMap;
            //}
            else if (scrollView.GetSelectedItemData().Message.Contains("Teleport"))
            {
                lastModifiedType = MenuCategory.Teleport;
            }
            else if (scrollView.GetSelectedItemData().Message.Contains("Menu"))
            {
                lastModifiedType = MenuCategory.Menu;
            }
        }

        public void ShowSpectatorSubmenu()
        {
            lastModifiedType = MenuCategory.Spectator;
        }

        public void BackToMainMenu()
        {
            lastModifiedType = MenuCategory.Main;
        }

        public bool IsSpectatorSubmenu()
        {
            if (scrollView.GetFirstItemData().Message.Contains("Spectator") && scrollView.GetFirstItemData().Type == ItemData.MenuType.Sub) return true;

            return false;
        }

        public bool IsTeleportSubmenu()
        {
            if (scrollView.GetFirstItemData().Message.Contains("Teleport") && scrollView.GetFirstItemData().Type == ItemData.MenuType.Sub) return true;

            return false;
        }

        public bool IsInSubmenu()
        {
            if (scrollView.GetFirstItemData().Type == ItemData.MenuType.Sub) return true;

            return false;
        }

        public static void AddTutorialViewItem(string tutorialName)
        {
            if (mainItemData.Any(x => x.Message.Equals(tutorialName)))
            {
                tutorialName = string.Empty;
                return;
            }

            int index = mainItemData.FindIndex(x => x.Message.Contains("Reset"));
            if (index != -1)
            {
                mainItemData.Insert(index, new ItemData(tutorialName, ItemData.MenuType.Main));
                prevSelectedIndex = index;
            }
            lastModifiedType = MenuCategory.Main;
        }

        public static void RemoveTutorialViewItem()
        {
            if (mainItemData.Count > MAIN_MENU_MAX_COUNT)
            {
                int index1 = mainItemData.FindIndex(x => x.Message.Contains("Spectator"));
                int startIndex = index1 + 1;
                if (index1 != -1)
                {
                    mainItemData.RemoveAt(startIndex);
                }
            }
            lastModifiedType = MenuCategory.Main;
        }

        public void UpdateMainMenu()
        {
            if (mainItemData.Count <= 0) { return; }
            scrollView.UpdateData(mainItemData.ToArray());

            if (prevSelectedIndex > (mainItemData.Count - 1)
                        || scroller.Position > (mainItemData.Count - 1))
            {
                scrollView.SelectCell(mainItemData.Count - 1);
                scroller.Position = mainItemData.Count - 1;
                prevSelectedIndex = mainItemData.Count - 1;
            }
            else
            {
                scrollView.SelectCell(prevSelectedIndex);
                scroller.Position = prevSelectedIndex;
            }
        }

        public void UpdateBodytrackingStatus()
        {
            if (VRSBodyTrackingManager.Instance == null ||
                prevTracking == VRSBodyTrackingManager.Instance.IsTracking()) { return; }

            lastModifiedType = MenuCategory.Bodytracking;
            if (VRSBodyTrackingManager.Instance.IsTracking())
            {
                bodytrackingItemData.Insert(1, new ItemData("Bodytracking: ON", ItemData.MenuType.Sub));
            }
            else
            {
                bodytrackingItemData.Insert(1, new ItemData("Bodytracking: OFF", ItemData.MenuType.Sub));
            }
            bodytrackingItemData.RemoveAt(2);
            scroller.Position = 2;
            prevTracking = VRSBodyTrackingManager.Instance.IsTracking();
        }

        public void SetPrevSelectedIndex(int idx)
        {
            prevSelectedIndex = idx;
        }
    }
}
