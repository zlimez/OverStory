using System.Collections;
using UnityEngine;

public class Burnable : MonoBehaviour
{
    // [SerializeField][Tooltip("Should be unique among all environment object that requires state persistence")] string envPartId;
    [SerializeField] GameObject[] fires;
    [SerializeField] float timeToBurnout;
    [SerializeField] Color burntColor;
    [SerializeField][Tooltip("Relative to original local scale")] Vector2 burntScale;
    [SerializeField] SpriteRenderer burnableRenderer;

    Color _startColor;

    bool _isBurning = false;

    public void Ignite()
    {
        if (_isBurning) return;
        _isBurning = true;
        foreach (GameObject fire in fires)
            fire.SetActive(true);
        _startColor = burnableRenderer.color;
        StartCoroutine(Burn());
    }

    IEnumerator Burn()
    {
        float et = 0f;
        float initSpriteHeight = burnableRenderer.bounds.size.y;
        Vector3 startScale = transform.localScale;
        float startY = transform.position.y, startX = transform.position.x;
        while (et < timeToBurnout)
        {
            et += Time.deltaTime;
            Vector3 newScale = Vector3.Lerp(Vector3.one, burntScale, et / timeToBurnout);
            transform.localScale = new Vector3(newScale.x * startScale.x, newScale.y * startScale.y, startScale.z);
            transform.position = new Vector3(startX, startY + initSpriteHeight * (newScale.y - 1) / 2, 0);
            burnableRenderer.color = Color.Lerp(_startColor, burntColor, et / timeToBurnout);
            yield return null;
        }
        gameObject.SetActive(false);
        foreach (GameObject fire in fires)
            fire.SetActive(false);
    }
}
