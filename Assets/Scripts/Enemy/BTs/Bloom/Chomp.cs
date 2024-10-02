using System.Collections.Generic;
using Abyss.Environment.Enemy;
using Abyss.Environment.Enemy.Anim;
using Abyss.Player;
using UnityEngine;

namespace BehaviorTree.Actions
{
    public class Chomp : CfAction
    {
        float _chompTime, _restTime, _chompDmg;
        AnimationCurve _chompCurve;
        BloomAnim _bloomAnim;
        Transform _neck1Tfm, _neck2Tfm, _neck3Tfm, _headTfm;
        EnemyManager _enemyManager;

        float chompTimer, restTimer, hChompTime;
        bool isResting = false;
        Vector3 startPos, targetPos;

        public Chomp(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            List<object> dataRef = Tree.GetData(_params);
            _chompTime = (float)dataRef[0];
            _restTime = (float)dataRef[1];
            _chompDmg = (float)dataRef[2];
            _chompCurve = (AnimationCurve)dataRef[3];
            _bloomAnim = (BloomAnim)dataRef[4];
            _neck1Tfm = (Transform)dataRef[5];
            _neck2Tfm = (Transform)dataRef[6];
            _neck3Tfm = (Transform)dataRef[7];
            _headTfm = (Transform)dataRef[8];
            _enemyManager = (EnemyManager)dataRef[9];
            hChompTime = _chompTime / 2;
        }

        public override void Update()
        {
            if (isResting)
            {
                if (restTimer >= _restTime)
                {
                    isResting = false;
                    restTimer = 0;
                    State = State.SUCCESS;
                }
                else restTimer += Time.deltaTime;
            }
            else
            {
                if (chompTimer == 0)
                {
                    startPos = _bloomAnim.TonguePos;
                    targetPos = Tree.GetDatum<Transform>("target").position + Vector3.up;
                    _bloomAnim.TransitionToState(BloomAnim.State.MouthOpen.ToString());
                    _enemyManager.OnStrikePlayer += ChompHit;
                }
                if (chompTimer >= _chompTime)
                {
                    isResting = true;
                    chompTimer = 0;
                    _bloomAnim.TransitionToState(BloomAnim.State.Idle.ToString());
                    _enemyManager.OnStrikePlayer -= ChompHit;
                }
                else
                {
                    chompTimer += Time.deltaTime;
                    if (chompTimer <= hChompTime)
                    {
                        float nt = _chompCurve.Evaluate(chompTimer / hChompTime);
                        _bloomAnim.MoveTo(startPos + new Vector3((targetPos.x - startPos.x) * nt, nt * nt * (targetPos.y - startPos.y), targetPos.z));
                    }
                    else
                    {
                        float nt = _chompCurve.Evaluate((chompTimer - hChompTime) / hChompTime);
                        _bloomAnim.MoveTo(targetPos + new Vector3((startPos.x - targetPos.x) * nt, nt * nt * (startPos.y - targetPos.y), startPos.z));
                    }
                    _neck1Tfm.position = _bloomAnim.Neck1Pos;
                    _neck2Tfm.position = _bloomAnim.Neck2Pos;
                    _neck3Tfm.position = _bloomAnim.Neck3Pos;
                    _headTfm.position = _bloomAnim.HeadPos;
                }
            }

            void ChompHit(float str)
            {
                Tree.GetDatum<Transform>("target").gameObject.GetComponent<PlayerManager>().TakeHit(str + _chompDmg);
            }
        }
    }
}
