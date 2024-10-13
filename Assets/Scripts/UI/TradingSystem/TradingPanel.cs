using Abyss.EventSystem;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TradingSystem : MonoBehaviour
{
    public GameObject tradingPanel;


    public Button CloseButton;



    private bool isTradingOpen = false;

    void Start()
    {
        tradingPanel.SetActive(false);
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