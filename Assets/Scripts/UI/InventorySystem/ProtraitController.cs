using UnityEngine;

public class ProtraitController : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject cameraPrefab;
    private GameObject player;
    private Camera mainCamera;
    private GameObject playerInstance;
    private GameObject cameraInstance;
    private void OnEnable()
    {
        player = GameObject.FindWithTag("Player");
        
        mainCamera = Camera.main;

        if (player == null || mainCamera == null)
        {
            Debug.LogError("Player 或 Main Camera 未找到，请确保它们在场景中存在。");
            return;
        }

        SpawnAtLocations();
    }

    private void OnDisable()
    {
        if (playerInstance != null)
        {
            Destroy(playerInstance);
        }

        if (cameraInstance != null)
        {
            Destroy(cameraInstance);
        }
    }

    private void SpawnAtLocations()
    {
        if (playerPrefab != null)
        {
            playerInstance = Instantiate(playerPrefab, player.transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Player 预制件未指定。");
        }

        if (cameraPrefab != null)
        {
            Vector3 cameraPosition = playerInstance.transform.position;
            cameraPosition.z = mainCamera.transform.position.z;
            cameraInstance = Instantiate(cameraPrefab, cameraPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Camera 预制件未指定。");
        }
    }
}
