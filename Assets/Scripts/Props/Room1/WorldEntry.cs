using Abyss.Player;
using Abyss.SceneSystem;
using Tuples;
using UnityEngine;

public class WorldEntry : ConvoTrigger
{
    [SerializeField] Transform initRespawnPoint;

    protected override void Execute()
    {
        base.Execute();
        player.GetComponent<PlayerManager>().LastRest = new Pair<AbyssScene, Vector3>(AbyssScene.Room1, initRespawnPoint.position);
    }
}
