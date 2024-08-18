using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Chronellium.Utils;

public class ScreenshotTaker : StaticInstance<ScreenshotTaker>
{
    public static ScreenshotTaker instance;
    Texture2D currentCapture;

    IEnumerator CaptureRoutine(Sprite outputSprite)
    {
        yield return new WaitForEndOfFrame();
        try
        {
            currentCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            currentCapture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
            currentCapture.Apply();
            outputSprite = Sprite.Create(currentCapture, new Rect(0, 0, Screen.width, Screen.height), new Vector2(0, 0));
            // TODO: Save to persistent storage
        }
        catch (System.Exception e)
        {
            Debug.LogError("Screen capture failed!");
            Debug.LogError(e.ToString());
        }
    }

    public void TakeScreenshot(Sprite outputSprite)
    {
        StartCoroutine(CaptureRoutine(outputSprite));
    }
}

