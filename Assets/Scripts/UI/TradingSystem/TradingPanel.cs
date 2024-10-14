using System.Collections.Generic;
using Abyss.EventSystem;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// public enum AreaType
// {
//     Player,
//     NPC
// }


// public class TradingArea
// {
//     public List<Countable<Item>> Items;
//     public AreaType area;
//     public int capability;

//     public int totalValue()
//     {
//         int value=0;
//         foreach (var itemStack in Items)
//         {
//             value += itemStack.Data.value * itemStack.Count ;
//         }
//         if (capability == 1) value = value / 2;
//         return value;
//     }
    
// }

public class TradingSystem : MonoBehaviour
{
    public GameObject tradingPanel;
    public Button CloseButton;
    private bool isTradingOpen = false;

    // TradingArea TopArea;
    // TradingArea ButtomArea;

    void Start()
    {
        tradingPanel.SetActive(false);
        // TopArea.capability = 1;
        // ButtomArea.capability = 5;
    }

    void OnEnable()
    {
        // if (GameManager.Instance == null)
        //     EventManager.StartListening(SystemEventCollection.SystemsReady, InitUpdateInventory);
        // else
        // {
        //     level = GameManager.Instance.PlayerPersistence.InventoryLevel;
        //     UpdateInventoryImage(level - 1);
        // }
    }

    void InitUpdateTrading(object input = null)
    {
        // level = GameManager.Instance.PlayerPersistence.InventoryLevel;
        // UpdateInventoryImage(level - 1);
        // EventManager.StopListening(SystemEventCollection.SystemsReady, InitUpdateInventory);
    }

    public void ToggleTrading()
    {
        isTradingOpen = !isTradingOpen;
        tradingPanel.SetActive(isTradingOpen);
    }

}