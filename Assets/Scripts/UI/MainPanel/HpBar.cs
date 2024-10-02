using System.Collections;
using System.Collections.Generic;
using Abyss.Player;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public GameObject layout;
    public GameObject HpPoint;
    // [SerializeField] PlayerAttr playerAttributes;
    public float health = (float)65;
    public int hp4eachGem = 20;

    void Start()
    {
        ClearJem();

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
        {
            Destroy(child.gameObject);
        }
    }

    float CountJem(float hp)
    {
        float cnt;
        cnt = health/hp4eachGem;
        return cnt;
    }
}
