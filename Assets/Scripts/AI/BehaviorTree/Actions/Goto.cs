// using System.Collections;
// using System.Collections.Generic;
// using BehaviorTree;
// using UnityEngine;

// namespace BehaviorTree.Actions
// {
//     public class Goto : CfAction
//     {
//         Transform _transform;
//         Transform _dest;
//         AnimationCurve _mvmtCurve;
//         float _duration;
//         float _timer = 0;

//         public override void Setup(BT tree)
//         {
//             base.Setup(tree);
//             List<object> dataRef = Tree.GetData(_params);
//             _transform = (Transform)dataRef[0];
//             _dest = (Transform)dataRef[1];
//             _mvmtCurve = (AnimationCurve)dataRef[2];
//             _duration = (float)dataRef[3];
//         }
//         public override void Update()
//         {
//             throw new System.NotImplementedException();
//         }


//     }
// }
