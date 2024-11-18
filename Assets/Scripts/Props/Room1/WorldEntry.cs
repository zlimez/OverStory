using Abyss.Player;
using Abyss.SceneSystem;
using UnityEngine;

public class WorldEntry : ConvoTrigger
{
    [SerializeField] Transform initRespawnPoint;

    protected override void Execute()
    {
        base.Execute();
        var playerManager = player.GetComponent<PlayerManager>();
        playerManager.LastRest.Head = AbyssScene.Room1;
        playerManager.LastRest.Tail = initRespawnPoint.position;
    }
}
