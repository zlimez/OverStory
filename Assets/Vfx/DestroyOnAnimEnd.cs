using System;
using UnityEngine;

public class DestroyOnAnimEnd : MonoBehaviour
{
    public Action OnAnimEnd;

    public void DestroySelf()
    {
        OnAnimEnd?.Invoke();
        Destroy(gameObject);
    }
}
