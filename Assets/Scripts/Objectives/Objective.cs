using UnityEngine;

public abstract class Objective : MonoBehaviour
{
    public string description;
    public bool isCompleted { get; protected set; }

    public abstract void CheckCompletion();
}
