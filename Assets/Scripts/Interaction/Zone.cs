using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    [SerializeField] string zoneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<CharacterController2D>(out CharacterController2D controller)) controller.zone = zoneName;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<CharacterController2D>(out CharacterController2D controller)) controller.zone = null;
    }
}
