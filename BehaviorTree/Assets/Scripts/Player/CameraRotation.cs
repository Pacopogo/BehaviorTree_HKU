using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private float speed = 6.0f;
    [SerializeField] private float direction;
    public void Horizontal(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>().x;
        direction = Mathf.Round(direction);
    }

    private void FixedUpdate()
    {
        if (direction == 0)
            return;

        transform.Rotate(Vector3.up * -direction * speed * Time.fixedDeltaTime);
    }
}
