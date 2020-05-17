using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{
    public PlayerInput playerInput;
    public PlayerController playerController;

    public bool canTransport = false;
    public bool toGrassland = false;
    public bool toHub = false;

    void Update()
    {
        if (canTransport && playerInput.interactKey)
        {
            playerController.SavePlayer();
            UsePortal();
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            canTransport = true;
        } else {
            canTransport = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") {
            canTransport = false;
        }
    }

    void UsePortal()
    {
        if (toGrassland)
        {
            SceneManager.LoadScene("Grassland");
        }
        if (toHub)
        {
            SceneManager.LoadScene("Hub");
        }
    }
}
