using System;
using UnityEngine;

public class CoroutinesGargageWithCoroutinesX : CoroutinesGarbageCreator
{
    protected override Func<YieldInstruction>[] CreateWaitCommandsBuilders()
    {
        return new Func<YieldInstruction>[]
        {
            () => CoroutinesX.WaitForSeconds(UnityEngine.Random.value * 0.1f),
            () => CoroutinesX.WaitForEndOfFrame,
            () => CoroutinesX.WaitForFixedUpdate,
        };
    }
}
