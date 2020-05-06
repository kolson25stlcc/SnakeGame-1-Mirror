using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
    public enum ControlType
    {
        Swipe, 
        Tilt,
        ArrowKeys
    }
    public enum SnakeType
    {
        Original,
        ColorChoice,
        Rainbow,
        Pastel,
        RGB
    }

    public static Color32 clrOriginal = new Color32(107, 224, 113, 255); // green

    public static Color32[] clrRainbow = { new Color32(0, 255, 0, 255), // green
                                            new Color32(0, 0, 255, 255), // blue
                                            new Color32(139, 0, 255, 255), // violet
                                            new Color32(255, 0, 0, 255), // red
                                            new Color32(255, 127, 0, 255), // orange
                                            new Color32(255, 255, 0, 255), }; // yellow

    public static Color32[] clrPastel = { new Color32(245, 247, 184, 255), // yellow
                                            new Color32(149, 225, 247, 255), // blue
                                            new Color32(182, 185, 237, 255), // purple
                                            new Color32(247, 181, 204, 255), // pink
                                            new Color32(186, 235, 213, 255),}; // green

    public Difficulty difficulty { get; private set; }
    public ControlType controlType { get; private set; }
    public SnakeType snakeType { get; private set; }
}
