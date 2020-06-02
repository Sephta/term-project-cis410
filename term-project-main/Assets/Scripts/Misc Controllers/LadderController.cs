using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderController : MonoBehaviour
{
    void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Player") {
            other.transform.Translate(Vector3.up * 0.125f);
        }
    }
}
