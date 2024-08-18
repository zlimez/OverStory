using UnityEngine;
using DigitalRuby.Tween;

public class CinematicBandAnimation : MonoBehaviour
{

    float startBandSize = 0.1f;
    public bool isPlayerEntering = false;
    // Start is called before the first frame update
    void Start()
    {
        OnTriggerEnter(null);
    }

    private void OnTriggerEnter(Collider other)
    {
        isPlayerEntering = !isPlayerEntering;

        if (isPlayerEntering)
        {
            BandIn();
        }
        else
        {
            BandOut();
        }
    }
    private void BandIn()
    {
        Beautify.Universal.BeautifySettings.settings.frameBandVerticalSize.value = .0f;
        System.Action<ITween<float>> BandInCallBack = (t) =>
        {
            Beautify.Universal.BeautifySettings.settings.frameBandVerticalSize.value = t.CurrentValue;
        };

        // completion defaults to null if not passed in
        gameObject.Tween("BandIn", 0.0f, startBandSize, 2.0f, TweenScaleFunctions.CubicEaseInOut, BandInCallBack);
    }
    private void BandOut()
    {
        Beautify.Universal.BeautifySettings.settings.frameBandVerticalSize.value = startBandSize;
        System.Action<ITween<float>> BandInCallBack = (t) =>
        {
            Beautify.Universal.BeautifySettings.settings.frameBandVerticalSize.value = t.CurrentValue;
        };

        // completion defaults to null if not passed in
        gameObject.Tween("BandOut", startBandSize, 0.0f, 2.0f, TweenScaleFunctions.CubicEaseInOut, BandInCallBack);
    }
}