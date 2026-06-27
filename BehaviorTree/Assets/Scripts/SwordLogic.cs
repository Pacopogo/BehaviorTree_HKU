using Unity.VisualScripting;
using UnityEngine;

public class SwordLogic : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
       

        Debug.Log(collision.gameObject.name);
        if (!collision.gameObject.GetComponent<MoveAgent>())
            return;

        collision.gameObject.SetActive(false);
    }
}
