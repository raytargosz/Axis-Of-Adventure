using UnityEngine;

public class TargetController : MonoBehaviour
{
    public DoorController doorController;
    private bool isActivated;

    public void Activate()
    {
        if (!isActivated)
        {
            isActivated = true;
            doorController.Unlock();
        }
    }

    public void Deactivate()
    {
        if (isActivated)
        {
            isActivated = false;
            doorController.Lock();
        }
    }

    public bool IsActivated()
    {
        return isActivated;
    }
}
