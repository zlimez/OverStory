using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Chronellium.Utils;
using Michsky.UI.Shift;

namespace Chronellium.MiniGames
{
    public class MiniGameUIWrapper : Singleton<MiniGameUIWrapper>
    {
        [SerializeField] private ModalWindowManager successModal;
        [SerializeField] private ModalWindowManager failureModal;
        [SerializeField] private ModalWindowManager infoModal;
        [SerializeField] private SelectedMiniGame selectedMiniGame;
        [SerializeField] private Canvas miniGameCanvas;
        [SerializeField] private Canvas uiWrapperCanvas;

        private MiniGameUnitBase activeMiniGameInstance;
        private ModalWindowManager activeModal;

        private void Start()
        {
            MiniGameUnitBase prefab = selectedMiniGame.selectedMiniGamePrefab;
            if (prefab != null)
            {
                // Set the mini-game Canvas sorting order lower than the UI Wrapper Canvas
                miniGameCanvas.sortingOrder = 0;
                uiWrapperCanvas.sortingOrder = 1;

                InitiateMiniGame(prefab);
            }
            else
            {
                Debug.LogError("Selected mini-game prefab not found.");
            }
        }

        private void InitiateMiniGame(MiniGameUnitBase miniGamePrefab)
        {

            if (activeMiniGameInstance != null)
            {
                Destroy(activeMiniGameInstance.gameObject);
            }

            // activeMiniGameInstance = Instantiate(miniGamePrefab);
            // activeMiniGameInstance.transform.position = Vector3.zero;
            activeMiniGameInstance = Instantiate(miniGamePrefab, miniGameCanvas.transform);
            activeMiniGameInstance.transform.position = Vector3.zero;

            activeMiniGameInstance.OnMiniGameSolvedSuccessfully += ShowSuccessModal;
            activeMiniGameInstance.OnMiniGameSolvedWrongly += ShowFailureModal;
        }

        public void ShowSuccessModal()
        {
            activeModal = successModal;
            successModal.ModalWindowIn();
        }

        public void ShowFailureModal()
        {
            activeModal = failureModal;
            failureModal.ModalWindowIn();
        }

        public void ShowInfoModal()
        {
            activeModal = infoModal;
            infoModal.ModalWindowIn();
        }

        // TODO: use hotkey instead?
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Press esc");
                ShowInfoModal();
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log("Press start");
                selectedMiniGame.StartSelectedMiniGame();
            }
        }

        private void Exit()
        {
            // Implement logic for exiting the miniGame scene
            SceneManager.LoadScene("Main Menu (Desktop)");
        }
    }
}
