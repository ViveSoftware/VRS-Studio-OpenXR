/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System.Linq;
using UnityEngine;

namespace FancyScrollView.Example03
{
    class Example03 : MonoBehaviour
    {
        readonly ItemData[] itemData =
        {
            new ItemData(
                "Mini map VO"
            ),
            new ItemData(
                "Mini map tutorial"
            ),
            //new ItemData(
            //    "Bubble VO"
            //),
            new ItemData(
                "Spectator VO"
            ),
            new ItemData(
                "Spectator tutorial"
            ),
            new ItemData(
                "Spectator: HMD"
            ),
            //new ItemData(
            //    "Bottle VO"
            //),
            //new ItemData(
            //    "3DObject VO"
            //),
            new ItemData(
                "Bodytracking VO"
            ),
            new ItemData(
                "Bodytracking tutorial"
            ),
            new ItemData(
                "3DSP Demo"
            )//),
            //new ItemData(
            //    "Keyboard VO"
            //)
        };

        [SerializeField] ScrollView scrollView = default;

        void Start()
        {
            var items = Enumerable.Range(0, 4)
                .Select(i => new ItemData($"Cell {i}"))
                .ToArray();

            scrollView.UpdateData(itemData);
            scrollView.SelectCell(0);
        }
    }
}
