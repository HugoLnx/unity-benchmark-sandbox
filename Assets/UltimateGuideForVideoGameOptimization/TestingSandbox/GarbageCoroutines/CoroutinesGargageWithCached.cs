using System;
using UnityEngine;

public class CoroutinesGargageWithCached : CoroutinesGarbageCreator
{
    private static readonly YieldInstruction WaitEOF = new WaitForEndOfFrame();
    private static readonly YieldInstruction WaitFixed = new WaitForFixedUpdate();
    private static readonly YieldInstruction Wait0_05Secs = new WaitForSeconds(0.05f);
    protected override Func<YieldInstruction>[] CreateWaitCommandsBuilders()
    {
        return new Func<YieldInstruction>[]
        {
            () => Wait0_05Secs,
            () => WaitEOF,
            () => WaitFixed,
        };
    }
}
