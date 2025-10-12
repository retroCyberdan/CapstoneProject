using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsKeys // <- uso una classe statica per gestire i PlayerPrefs
{
    public const string Volume = "Volume"; // <- una variabile const è automaticamente static
    public const string Graphics = "Graphics";
    public const string Resolution = "Resolution";
    public const string FullScreen = "FullScreen";
}