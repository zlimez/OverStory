using System;
using AnyPortrait;
using UnityEngine;
using UnityEngine.Assertions;

namespace Environment.Enemy.Anim
{
    public class HogAnim : MonoBehaviour
    {
        // Polish to add stun animation
        public enum State
        {
            Idle,
            Walk,
            Charge,
            ChargeUp,
            Death
        }

        private State currState;
        [SerializeField] apPortrait portrait;
        // [SerializeField] float crossFadeSeconds = .01f;
        [SerializeField] float walkSpeed = 2.5f;
        [SerializeField] float runSpeed = 8f;
        bool isFirstFrame = true;
        EnemyManager _enemyManager;
        System.Action _playDeathAnim;

        void Awake()
        {
            portrait.Initialize();
            // StartCoroutine(_bTSetup);
            _enemyManager = GetComponent<EnemyManager>();
            currState = State.Idle;
        }

        void Update()
        {
            if (isFirstFrame)
            {
                PlayAnimation(currState.ToString());
                isFirstFrame = false;
            }
        }

        void OnEnable()
        {
            _playDeathAnim = () => TransitionToState(State.Death);
            GetComponent<EnemyManager>().OnDeath += _playDeathAnim;
        }

        void OnDisable()
        {
            _enemyManager.OnDeath -= _playDeathAnim;
        }

        public void TransitionToState(State newState)
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

        public void StateBySpeed(float speed)
        {
#if DEBUG
            Assert.IsTrue(speed >= 0);
#endif
            if (speed == 0)
                TransitionToState(State.Idle);
            else if (speed <= walkSpeed)
                TransitionToState(State.Walk);
            else
                TransitionToState(State.Charge);
        }
    }
}
