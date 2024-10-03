using System;
using AnyPortrait;
using UnityEngine;

namespace Abyss.Environment.Enemy.Anim
{
    public class PortraitAnim : MonoBehaviour
    {
        [SerializeField] protected apPortrait portrait;
        protected bool isFirstFrame = true;
        EnemyManager _enemyManager;
        protected Action _playDeathAnim;
        protected string currState;

        protected virtual void Awake()
        {
            portrait.Initialize();
            _enemyManager = GetComponent<EnemyManager>();
        }

        void LateUpdate()
        {
            if (isFirstFrame)
            {
                PlayAnimation(currState.ToString());
                isFirstFrame = false;
            }
        }

        protected virtual void OnEnable()
        {
            _enemyManager.OnDeath += _playDeathAnim;
        }

        void OnDisable()
        {
            _enemyManager.OnDeath -= _playDeathAnim;
        }

        public void TransitionToState(string newState)
        {
            if (currState == newState)
                return;
            currState = newState;
            isFirstFrame = true;
        }

        private void PlayAnimation(string animToPlay)
        {
            try
            {
                apAnimPlayData animData = portrait.CrossFade(animToPlay);
                if (animData == null)
                    Debug.LogWarning("Failed to play animation " + animToPlay);
            }
            catch (Exception)
            {
                Debug.LogWarning($"Error playing animation {animToPlay}. The portrait is likely not initialized");
            }
        }
    }
}
