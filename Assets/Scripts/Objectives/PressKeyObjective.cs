using UnityEngine;

public class PressKeyObjective : Objective
{
    public KeyCode targetKey;

    public override void CheckCompletion()
    {
        if (Input.GetKeyDown(targetKey))
        {
            isCompleted = true;
        }
    }
}
