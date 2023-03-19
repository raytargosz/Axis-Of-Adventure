using UnityEngine;

public class FreezeWorld : MonoBehaviour
{
    private float previousTimeScale;
    private bool worldFrozen;

    private void Update()
    {
        bool isHoldingShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (isHoldingShift && !worldFrozen)
        {
            Freeze();
        }
        else if (!isHoldingShift && worldFrozen)
        {
            Unfreeze();
        }
    }

    private void Freeze()
    {
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        worldFrozen = true;
    }

    private void Unfreeze()
    {
        Time.timeScale = previousTimeScale;
        worldFrozen = false;
    }
}
