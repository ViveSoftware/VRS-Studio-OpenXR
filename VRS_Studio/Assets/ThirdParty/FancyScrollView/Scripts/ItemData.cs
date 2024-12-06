/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

namespace FancyScrollView.Example03
{
    class ItemData
    {
        public string Message { get; }
        public MenuType Type { get; }

        public ItemData(string message)
        {
            Message = message;
        }

        public ItemData(string message, MenuType type)
        {
            Message = message;
            Type = type;
        }

        public enum MenuType
        {
            Main,
            Sub,
        }
    }
}
