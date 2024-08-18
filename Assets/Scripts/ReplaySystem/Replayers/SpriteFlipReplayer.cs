using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeepBreath.ReplaySystem {
    public class SpriteFlipReplayer : MonoBehaviour, Replayer<string>
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private bool isFlipX = true;
        
        public void Consume(ActionReplayRecord<string> record) {
            SpriteFlipReplayRecord flipRecord = (SpriteFlipReplayRecord)record;
            if (flipRecord.IsNoChange) return;
            var flipData = flipRecord.GetData();
            if (isFlipX) {
                spriteRenderer.flipX = bool.Parse(flipData);
            } else {
                spriteRenderer.flipY = bool.Parse(flipData);
            }
        }
    }
}
