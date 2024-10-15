using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemObtainedHint : MonoBehaviour
{
    public bool isActive;
    public GameObject hintBox;
    public Transform hintBoxTransform;
    public Image itemImage;
    public Text nameHolder;
    public List<Item> items;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (items.Count != 0 && isActive && !DialogueManager.Instance.InDialogue)
        {
            isActive = false;
            StartCoroutine(Display());
        }
    }

    public void Show(Item item)
    {
        // Debug.Log("Showing Hint box");
        isActive = true;
        items.Add(item);
        itemImage.sprite = item.itemImage;
        itemImage.preserveAspect = true;
        nameHolder.text = item.itemName;
    }

    IEnumerator Display()
    {

        hintBox.SetActive(true);

        // Debug.Log("Animating hint bnox");
        for (int j = 0; j < items.Count; j++)
        {
            itemImage.sprite = items[j].itemImage;
            nameHolder.text = items[j].itemName;

            animator.CrossFade("Window In", 0.1f);

            yield return new WaitForSeconds(2f);
            animator.CrossFade("Window Out", 0.1f);
            yield return new WaitForSeconds(0.5f);
        }
        hintBox.SetActive(false);
        items.Clear();
    }
}
