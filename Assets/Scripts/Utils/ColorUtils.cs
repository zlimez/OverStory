using UnityEngine;

public class ColorUtils {
    public static Color transparent = new Color(0, 0, 0, 0);

    public static Color CubicLerpColor(Color from, Color to, float t) {
        float clampedT = Mathf.Clamp(t, 0, 1);
        return Color.LerpUnclamped(from, to, 1 + Mathf.Pow(clampedT - 1, 3));
    }
    public static Color HexToRGB(string hex, float alpha)
    {
        if (hex.Length != 6) // make sure the string is exactly 6 characters long
        {
            Debug.LogError("Invalid hex color: " + hex);
            return Color.black; // return default color
        }

        // parse each pair of hex digits into integers
        int r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        int g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        int b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        // convert each integer to a float in the range 0-1
        float rf = r / 255.0f;
        float gf = g / 255.0f;
        float bf = b / 255.0f;

        // return the corresponding color
        return new Color(rf, gf, bf, alpha);
    }
}
