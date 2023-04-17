using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private PressurePadController_2 pressurePadA;
    [SerializeField] private PressurePadController_2 pressurePadB;

    private bool moveToStartPoint = true;
    private bool playerOnElevator = false;

    private void Start()
    {
        pressurePadA.OnPadTriggered += PadTriggered;
        pressurePadB.OnPadTriggered += PadTriggered;
    }

    private void PadTriggered(PressurePadController_2 pad)
    {
        moveToStartPoint = pad == pressurePadA;
    }

    private void Update()
    {
        if (playerOnElevator || pressurePadA.PadTriggered || pressurePadB.PadTriggered)
        {
            MoveElevator();
        }
    }

    private void MoveElevator()
    {
        float step = speed * Time.deltaTime;
        if (moveToStartPoint)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPoint.position, step);
            if (transform.position == startPoint.position)
            {
                moveToStartPoint = false;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, endPoint.position, step);
            if (transform.position == endPoint.position)
            {
                moveToStartPoint = true;
            }
        }
    }

    public void SetPlayerOnElevator(bool isOnElevator)
    {
        playerOnElevator = isOnElevator;
    }
}
