using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DigitalRuby.Tween;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using Chronellium.EventSystem;

public class ExitAttemptLogic : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;

    public AudioClip rubbleBGM;
    public AudioSource bgm;
    private CinemachineBasicMultiChannelPerlin noise;
    public GameObject player;
    public GameObject finalExit;
    public Transform UnderWaterStartPos;
    public PlayableDirector timeline;
    public CinemachineVirtualCamera virtualCam;
    [SerializeField] Conversation inquiry1;
    private void Start()
    {
        EventManager.StartListening(StaticEvent.Core_ResetPuzzle, ResetPuzzle);
    }

    private void ResetPuzzle(object input = null)
    {
        Debug.Log("Reseting puzzle");

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // player.transform.position = UnderWaterStartPos.position;

        // noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        // System.Action<ITween<float>> BandInCallBack = (t) =>
        // {
        //     noise.m_AmplitudeGain = t.CurrentValue;
        // };
        // BreathTimer.Instance.gameObject.SetActive(false);
        // // // completion defaults to null if not passed in
        // gameObject.Tween("Remove camera shake", noise.m_AmplitudeGain, 0.0f, 2.0f, TweenScaleFunctions.CubicEaseInOut, BandInCallBack);
    }
    public void StartRumbleBGM()
    {
        bgm.clip = rubbleBGM;
        bgm.Play();
    }

    public void StartChase()
    {
        finalExit.SetActive(true);
        EventManager.InvokeEvent(StaticEvent.Core_LowBreath);

    }

    // Update is called once per frame
    public void StartBreathTimer()
    {
        // BreathTimer.Instance.gameObject.SetActive(true);
    }

    public void startOhShtDialog()
    {
        DialogueManager.Instance.StartConversation(inquiry1);
    }

    void OnTriggerEnter(Collider other)
    {
        CinemachineBasicMultiChannelPerlin noise = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();


        timeline.Play();
    }

}
