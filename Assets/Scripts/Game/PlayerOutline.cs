using UnityEngine;

public class PlayerOutline : MonoBehaviour
{
    public GameObject player;
    public GameObject playerOutline;
    public LayerMask obstacleMask;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        playerOutline.SetActive(false);
    }

    private void Update()
    {
        Vector3 direction = player.transform.position - mainCamera.transform.position;
        RaycastHit hit;

        if (Physics.Raycast(mainCamera.transform.position, direction, out hit, Mathf.Infinity, obstacleMask))
        {
            if (hit.collider.CompareTag("Player"))
            {
                playerOutline.SetActive(false);
            }
            else
            {
                playerOutline.SetActive(true);
            }
        }
    }
}
