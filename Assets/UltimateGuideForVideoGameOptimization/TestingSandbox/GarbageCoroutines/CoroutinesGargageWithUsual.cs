using System;
using UnityEngine;

public class CoroutinesGargageWithUsual : CoroutinesGarbageCreator
{
    protected override Func<YieldInstruction>[] CreateWaitCommandsBuilders()
    {
        return new Func<YieldInstruction>[]
        {
            () => new WaitForSeconds(UnityEngine.Random.value * 0.1f),
            () => new WaitForEndOfFrame(),
            () => new WaitForFixedUpdate(),
        };
    }
}
