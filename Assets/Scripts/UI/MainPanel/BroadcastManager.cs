using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Abyss.Utils;
using Abyss.EventSystem;
using Tuples;
using System;

public class BroadcastManager : MonoBehaviour
{
    [SerializeField] float speedMod, defaultCharInterval = 0.005f;
    [SerializeField] private float MaintainTime = 2f;

    [Header("Broadcast Bubble")]
    [SerializeField] private GameObject broadcastBox;
    [SerializeField] private TextMeshProUGUI broadcasttext;

    [Header("Broadcast Content")]
    [SerializeField] public string PlayerIntelligenceChange = "Your intelligence has increased by {value} points.";
    [SerializeField] public string WeaponEquipped = "You equip the {weaponName}.";
    [SerializeField] public string WeaponUnequipped = "You unequip your weapon. You are now unarmed.";
    [SerializeField] public string GetItem = "You obtained x{quantity} {itemType}: \"{itemName}\".";
    [SerializeField] public string LoseItem = "You remove from your inventory x{quantity} {itemType}: \"{itemName}\".";
    
    private Queue<string> msgQueue = new Queue<string>();
    private bool isMsgPlaying = false;
    private Coroutine msgCoroutine;


    void OnEnable()
    {
        EventManager.StartListening(PlayEvents.PlayerIntelligenceChange, PlayerIntelligenceChange2Msg);
        EventManager.StartListening(PlayEvents.WeaponEquipped, WeaponEquipped2Msg);
        EventManager.StartListening(PlayEvents.WeaponUnequipped, WeaponUnequipped2Msg);
        EventManager.StartListening(PlayEvents.PlayerItemChange, PlayerItemChange2Msg);
        EventManager.StartListening(PlayEvents.Message, UseEnqueueMsg);
    }

    void OnDisable()
    {
        EventManager.StopListening(PlayEvents.PlayerIntelligenceChange, PlayerIntelligenceChange2Msg);
        EventManager.StopListening(PlayEvents.WeaponEquipped, WeaponEquipped2Msg);
        EventManager.StopListening(PlayEvents.WeaponUnequipped, WeaponUnequipped2Msg);
        EventManager.StopListening(PlayEvents.PlayerItemChange, PlayerItemChange2Msg);
        EventManager.StopListening(PlayEvents.Message, UseEnqueueMsg);
    }

    void PlayerIntelligenceChange2Msg(object args)
    {
        float value = (float) args;
        EnqueueMsg(PlayerIntelligenceChange.Replace("{value}", value.ToString()), MaintainTime);
    }

    void WeaponEquipped2Msg(object args)
    {
        WeaponItem weapon = (WeaponItem) args;
        EnqueueMsg(WeaponEquipped.Replace("{weaponName}", weapon.itemName), MaintainTime);
    }

    void WeaponUnequipped2Msg(object args)
    {
        EnqueueMsg(WeaponUnequipped, MaintainTime);
    }

    void PlayerItemChange2Msg(object args)
    {
        RefPair<Item, int> itemWithCnt = (RefPair<Item, int>) args;
        Item item = itemWithCnt.Head;
        int cnt = itemWithCnt.Tail;
        if (item.itemType == ItemType.Spells || item.itemType == ItemType.Constructions || item.itemType == ItemType.Blueprints) return;
        string msg;
        if (cnt > 0) msg = GetItem.Replace("{itemType}", item.itemType.ToString()).Replace("{itemName}", item.itemName).Replace("{quantity}", cnt.ToString());
        else
        {
            cnt = -cnt;
            msg = LoseItem.Replace("{itemType}", item.itemType.ToString()).Replace("{itemName}", item.itemName).Replace("{quantity}", cnt.ToString());

        }
        EnqueueMsg(msg, MaintainTime);
    }

    void UseEnqueueMsg(object args)
    {
        string msg = (string) args;
        EnqueueMsg(msg, MaintainTime);
    }


    public void EnqueueMsg(string msg, float duration)
    {
        msgQueue.Enqueue(msg);
        if (!isMsgPlaying)
        {
            StartNextMsg(duration);
        }
    }

    private void StartNextMsg(float duration)
    {
        if (msgQueue.Count > 0)
        {
            string nextMsg = msgQueue.Dequeue();
            msgCoroutine = StartCoroutine(DisplayMsg(nextMsg, duration));
        }
    }

    private IEnumerator DisplayMsg(string msg, float duration)
    {
        isMsgPlaying = true;
        broadcasttext.text = "";
        broadcastBox.SetActive(true);

        foreach (char letter in msg.ToCharArray())
        {
            broadcasttext.text += letter;
            yield return new WaitForSecondsRealtime(defaultCharInterval / speedMod);
        }

        yield return new WaitForSeconds(duration);
        broadcastBox.SetActive(false);
        isMsgPlaying = false;

        if (msgQueue.Count > 0)
        {
            StartNextMsg(duration);
        }
    }

    public void ClearQueue()
    {
        if (msgCoroutine != null)
        {
            StopCoroutine(msgCoroutine);
        }
        msgQueue.Clear();
        broadcastBox.SetActive(false);
        isMsgPlaying = false;
    }


}

