using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public bool canTransport = false;

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            canTransport = true;
        } else {
            canTransport = false;
        }
    }
}
