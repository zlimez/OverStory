using Abyss.EventSystem;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public GameObject layout;
    public GameObject HpPoint;
    public int hp4eachGem = 20;

    void OnEnable()
    {
        if (GameManager.Instance != null)
            Load();
        else EventManager.StartListening(SystemEvents.SystemsReady, Load);
        EventManager.StartListening(PlayEvents.PlayerHealthChange, UpdateHpBar);
    }

    void OnDisable()
    {
        EventManager.StopListening(SystemEvents.SystemsReady, Load);
        EventManager.StopListening(PlayEvents.PlayerHealthChange, UpdateHpBar);
    }

    void Load(object input = null)
    {
        UpdateHpBar(GameManager.Instance.PlayerPersistence.PlayerAttr.Health);
        EventManager.StopListening(PlayEvents.PlayerHealthChange, Load);
    }

    void UpdateHpBar(object input)
    {
        ClearJem();
        float health = (float)input;
        float GemCnt = CountJem(health);

        for (; GemCnt >= 1; GemCnt--) Instantiate(HpPoint, layout.transform);


        if (GemCnt > 0)
        {
            GameObject partialJem = Instantiate(HpPoint, layout.transform);
            Image jemImage = partialJem.GetComponent<Image>();

            jemImage.fillAmount = GemCnt;
        }
    }

    void UpdateHpBar()
    {
        // ClearJem();

        // float GemCnt = CountJem(health);

        // for (; GemCnt >= 1; GemCnt--) Instantiate(HpPoint, layout.transform);


        // if (GemCnt > 0)
        // {
        //     GameObject partialJem = Instantiate(HpPoint, layout.transform);
        //     Image jemImage = partialJem.GetComponent<Image>();

        //     jemImage.fillAmount = GemCnt;
        // }
    }

    void ClearJem()
    {
        foreach (Transform child in layout.transform)
            Destroy(child.gameObject);
    }

    float CountJem(float hp)
    {
        float cnt;
        cnt = hp / hp4eachGem;
        return cnt;
    }
}
