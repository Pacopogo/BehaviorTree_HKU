using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayer : MonoBehaviour
{
    [SerializeField] private Transform cameraTrans;
    [SerializeField] private MoveAgent agent;
    private RaycastHit hit;

    public void NewDestination(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Hit: " + hit.point);
            agent.MoveTarget(hit.point);
        }


    }
}
