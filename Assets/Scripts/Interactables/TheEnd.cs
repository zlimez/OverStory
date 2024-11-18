using UnityEngine;
using TMPro;
using System.Collections;

namespace Abyss.Interactables
{
    // TODO: Add persistence or refresh items logic
    public class TheEnd : Interactable
    {
        [SerializeField] public TextMeshProUGUI text;
        [SerializeField] public GameObject bg;
        [SerializeField] public float fadeDuration = 5f;
        private void Start()
        {
            SetAlpha(0f);
            bg.SetActive(false);
        }

        protected override void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
            {
                bg.SetActive(true);
                StartCoroutine(FadeInText());
            }

        }

        private IEnumerator FadeInText()
        {
            float elapsedTime = 0f; // 计时器

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;

                float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
                SetAlpha(alpha);

                yield return null;
            }

            SetAlpha(1f);
        }

        private void SetAlpha(float alpha)
        {
            Color color = text.color;
            color.a = alpha;
            text.color = color;
        }
    }

}
