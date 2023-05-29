using System;
using UnityEngine;

/* OUTPUT....
============
=> This is the only way that works with components attached to destroyed objects.
============
destroyed == null
True


============
=> All of those other approaches won't "work properly" on components attached to destroyed objects.
============
ReferenceEquals(destroyed, null)
False

destroyed is null
False

destroyed ?? otherComponent
null

destroyed ??= otherComponent
null

destroyed?.MovePosition(...): rigidbody?.MovePosition(...):
EXCEPTION: The object of type 'Rigidbody' has been destroyed but you are still trying to access it.
Your script should either check if it is null or you should not destroy the object.
*/

public class DummyScript : MonoBehaviour
{
    private void Start()
    {
        Component destroyed = GetComponentInChildren<Rigidbody>();
        Component otherComponent = this;
        DestroyImmediate(destroyed.gameObject);

        Debug.Log("============");
        Debug.Log("=> This is the only way that works with components attached to destroyed objects.");
        Debug.Log("============");

        Debug.Log("destroyed == null");
        Debug.Log(destroyed == null);




        Debug.Log("============");
        Debug.Log("=> All of those other approaches won't \"work properly\" on components attached to destroyed objects.");
        Debug.Log("============");

        Debug.Log("ReferenceEquals(destroyed, null)");
        Debug.Log(ReferenceEquals(destroyed, null));

        Debug.Log("destroyed is null");
        Debug.Log(destroyed is null);

        Debug.Log("destroyed ?? otherComponent");
        Debug.Log(destroyed ?? otherComponent);

        Debug.Log("destroyed ??= otherComponent");
        Debug.Log(destroyed ??= otherComponent);

        Debug.Log("destroyed?.CompareTag(...)");
        TryAndLog(() => destroyed?.CompareTag("Tag"));
    }

    private void TryAndLog(Action action)
    {
        try {
            action();
            Debug.Log("SUCCESS");
        }
        catch (Exception e) {
            Debug.Log($"EXCEPTION: {e.Message}");
        }
    }
    private void TryAndLog<T>(Func<T> func)
    {
        TryAndLog(() => {
            func();
        });
    }
}