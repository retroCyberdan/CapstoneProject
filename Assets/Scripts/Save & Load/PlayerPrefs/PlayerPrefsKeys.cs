using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsKeys // <- uso una classe statica per gestire i PlayerPrefs
{
    public const string Volume = "Volume"; // <- volume globale (Master)
    public const string BGMVolume = "BGMVolume"; // <- volume musica
    public const string SFXVolume = "SFXVolume"; // <- volume effetti sonori
    public const string Graphics = "Graphics";
    public const string Resolution = "Resolution";
    public const string FullScreen = "FullScreen";
}