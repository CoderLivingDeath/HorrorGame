using Cysharp.Threading.Tasks;
using ScriptableAnimation;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static AnimationLibrary;

public class SequenceComponent : MonoBehaviour
{
    [SerializeReference]
    public BaseAnimationContext context;

    [SerializeReference]
    public BaseAnimationContext[] ctxarray;

    [ContextMenu("Execute animation")]
    public void TestMethod()
    {
        Test().Forget();
    }

    public async UniTask Test()
    {
        foreach(var item in ctxarray)
        {
            await PlayAnimation(item, destroyCancellationToken);
        }
    }
}
