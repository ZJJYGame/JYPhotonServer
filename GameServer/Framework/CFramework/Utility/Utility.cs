﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Reflection;
#if UNITY_EDITOR
using Object = UnityEngine.Object;
using UnityEngine;
#endif
namespace Cosmos
{
    public static class MessageColor
    {
        public const string BLACK = "#000000";
        public const string WHITE = "#FFFFFF";
        public const string BLUE = "#254FDB";
        /// <summary>
        /// 棕色
        /// </summary>
        public const string BROWN = "#A52A2A"; 
        public const string DARKBLUE = "#0000A0";
        public const string GREEN = "#008000";
        /// <summary>
        /// 青橙绿
        /// </summary>
        public const string LIME = "#00FF00"; 
        public const string CYAN = "#00FFFF";
        public const string DARKCYAN = "#008B8B";
        public const string GREY = "#808080";
        /// <summary>
        /// 洋红
        /// </summary>
        public const string FUCHSIA = "#FF00FF";
        /// <summary>
        /// 海军蓝
        /// </summary>
        public const string NAVY = "#000080"; 
        public const string ORANGE = "#FFA500";
        public const string RED = "#FF0000";
        /// <summary>
        /// 蓝绿
        /// </summary>
        public const string TEAL = "#008080";
        public const string YELLOW = "#FF0000";
        /// <summary>
        /// 褐红
        /// </summary>
        public const string MAROON = "#800000"; 
        public const string PURPLE = "#800080";
        /// <summary>
        /// 蓝紫罗兰
        /// </summary>
        public const string BLUEVIOLET = "#8A2BE2";
        /// <summary>
        /// 紫兰
        /// </summary>
        public const string INDIGO = "#4B0082"; 
    }
    /// <summary>
    /// 通用工具类：
    /// 数组工具，反射工具，文字工具，加密工具，
    /// 数学工具，持久化数据工具，Debug工具，Editor工具等等
    /// </summary>
    public sealed partial class Utility
    {
        public static void DebugLog(object o, Object context = null)
        {
#if UNITY_EDITOR
            if (!ApplicationConst.Editor.EnableDebugLog)
                return;
            if (context == null)
                Debug.Log("<b>-->><color="+MessageColor.BLUE+">" + o + "</color></b>");
            else
                Debug.Log("<b>-->><color=" + MessageColor.BLUE + ">" + o + "</color></b>", context);
#endif
        }
        public static void DebugLog(object o, string messageColor, Object context = null)
        {
#if UNITY_EDITOR
            if (!ApplicationConst.Editor.EnableDebugLog)
                return;
            if (context == null)
                Debug.Log("<b>-->><color=" + messageColor + ">"+ o + "</color></b>");
            else
                Debug.Log("<b>-->><color=" + messageColor+ ">" + o + "</color></b>", context);
#endif
        }
        public static void DebugWarning(object o, Object context = null)
        {
#if UNITY_EDITOR
            if (!ApplicationConst.Editor.EnableDebugLog)
                return;
            if (context == null)
                Debug.LogWarning("<b>-->><color=#FF5E00>" + o + "</color></b>");
            else
                Debug.LogWarning("<b>-->><color=#FF5E00>" + o + "</color></b>", context);
#endif
        }
        public static void DebugError(object o, Object context = null)
        {
#if UNITY_EDITOR
            if (!ApplicationConst.Editor.EnableDebugLog)
                return;
            if (context == null)
                Debug.LogError("<b>-->><color=#FF0000>" + o + "</color></b>");
            else
                Debug.LogError("<b>-->><color=#FF0000>" + o + "</color></b>", context);
#endif
        }
        /// <summary>
        ///谨慎使用这个方法
        /// </summary>
        public static void ClearMemory()
        {
#if UNITY_EDITOR
            GC.Collect(); Resources.UnloadUnusedAssets();
#endif
        }
        /// <summary>
        ///字典工具 
        /// </summary>
        public static K GetValue<T, K>(Dictionary<T, K> dict, T key)
        {
            K value =default(K);
            bool isSuccess = dict.TryGetValue(key, out value);
            if (isSuccess)
                return value;
            return value;
        }
    }
}