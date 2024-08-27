using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;
using Beautify.Universal;

public class StartSceneCutsceneManager : MonoBehaviour
{
    private List<float> distance = new List<float>();
    private int cur_idx;

    public void NextTarget()
    {
        Beautify.Universal.BeautifySettings.settings.frameBandVerticalSize.value = .0f;
        System.Action<ITween<float>> BandInCallBack = (t) =>
        {
            Beautify.Universal.BeautifySettings.settings.depthOfFieldDistance.value = t.CurrentValue;
        };

        // completion defaults to null if not passed in
        gameObject.Tween("BandIn", Beautify.Universal.BeautifySettings.settings.depthOfFieldDistance.value, distance[cur_idx++], 2.0f, TweenScaleFunctions.CubicEaseInOut, BandInCallBack);
    }


    // Start is called before the first frame update void Start()
    void Awake()
    {
        distance.Add(4.367f);
        distance.Add(51.69f);
        cur_idx = 0;
        NextTarget();
    }

}
