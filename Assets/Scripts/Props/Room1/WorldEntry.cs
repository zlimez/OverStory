using Abyss.Player;
using UnityEngine;

namespace Abyss.Gameplay
{
    public class WorldEntry : ConvoTrigger
    {
        [SerializeField] Transform initRespawnPoint;

        protected override void Execute()
        {
            base.Execute();
            var playerManager = player.GetComponent<PlayerManager>();
            playerManager.LastRest.Head = Settings.Scene.Room1;
            playerManager.LastRest.Tail = initRespawnPoint.position;
        }
    }
}
