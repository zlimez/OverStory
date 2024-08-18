using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Michsky.UI.Shift;
using TMPro;

namespace Chronellium.MiniGames
{
    public class MiniGameButton : MonoBehaviour
    {
        [SerializeField] private MiniGameUnitBase miniGame;
        [SerializeField] private Button button;
        [SerializeField] private SelectedMiniGame selectedMiniGame;
        [SerializeField] private TextMeshProUGUI windowTitle;
        [SerializeField] private TextMeshProUGUI windowDescription;
        [SerializeField] private TextMeshProUGUI gameMode;
        [SerializeField] private TextMeshProUGUI difficulty;
        // [SerializeField] private SelectedMiniGameModal selectedMiniGameModal;

        private void Awake()
        {
            windowTitle.text = miniGame.title;
            windowDescription.text = miniGame.description;
            gameMode.text = miniGame.gameMode.ToString();
            difficulty.text = miniGame.difficulty.ToString();
        }

        private void Start()
        {
            button.onClick.AddListener(UpdateSelectedMiniGame);
        }

        private void UpdateSelectedMiniGame()
        {
            // selectedMiniGameModal.ModalWindowIn();
            selectedMiniGame.UpdateSelectedMiniGame(miniGame);
        }
    }

}