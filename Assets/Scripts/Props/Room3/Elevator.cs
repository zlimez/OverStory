using System.Collections;
using Abyss.Interactables;
using UnityEngine;

public class Elevator : Interactable
{
    [SerializeField] PowerPanel.Config requiredConfig;
    [SerializeField] float maxSpeed = 10f, acceleration = 1f;

    float _speed = 0;

    // protected override void OnTriggerEnter2D(Collider2D collider)
    // {
    //     if (GameManager.Instance.EnvStatePersistence.ContainsKey(PowerPanel.CONFIG_KEY))
    //     {
    //         PowerPanel.Config config = (PowerPanel.Config)GameManager.Instance.EnvStatePersistence[PowerPanel.CONFIG_KEY];
    //         if (config == requiredConfig)
    //             base.OnTriggerEnter2D(collider);
    //     }
    // }

    public override void Interact()
    {
        base.Interact();
        StartCoroutine(Elevate());
    }

    IEnumerator Elevate()
    {
        while (true)
        {
            transform.position += _speed * Time.deltaTime * Vector3.up;
            _speed = Mathf.Min(_speed + acceleration * Time.deltaTime, maxSpeed);
            yield return null;
        }
    }
}
