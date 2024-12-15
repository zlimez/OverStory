using System.Collections;
using Abyss.EventSystem;
using Abyss.Interactables;
using UnityEngine;
using UnityEngine.UI;

public class BGChanger : MonoBehaviour
{
    [SerializeField] public RawImage frontBG;
    [SerializeField] public RawImage backBG;
    [SerializeField] private float fadeDuration = 1.0f; 

    private Coroutine fadeCoroutine;

    void OnEnable()
    {
        UpdateBackgroundImage(null);
        EventManager.StartListening(SystemEvents.ChangeCameraBG, UpdateBackgroundImage);
    }
    
    void OnDisable() => EventManager.StopListening(SystemEvents.ChangeCameraBG, UpdateBackgroundImage);

    private void UpdateBackgroundImage(object input)
    {
        Texture bg = (Texture)input;

        if (bg == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                Debug.LogWarning("Player 对象未找到！");
                return;
            }

            GameObject[] bgTriggers = GameObject.FindGameObjectsWithTag("CameraBGTrigger");

            foreach (var trigger in bgTriggers)
            {
                Collider2D triggerCollider = trigger.GetComponent<Collider2D>();
                if (triggerCollider == null)
                {
                    Debug.LogWarning($"{trigger.name} 没有 Collider2D 组件！");
                    continue;
                }

                if (triggerCollider.bounds.Contains(player.transform.position))
                {
                    Debug.Log($"Player 在 {trigger.name} 触发器中");
                    BGChangePost bgChangePost = trigger.GetComponent<BGChangePost>();
                    frontBG.texture = bgChangePost.CameraBG;
                    break;
                }
            }
        }
        else
        {
            backBG.texture = frontBG.texture;
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeInBackground(bg));
        }
    }

    private IEnumerator FadeInBackground(Texture bg)
    {
        frontBG.texture = bg;

        Color color = frontBG.color;
        color.a = 0f;
        frontBG.color = color;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration); 
            frontBG.color = color;
            yield return null;
        }

        color.a = 1f;
        frontBG.color = color;
    }
}
