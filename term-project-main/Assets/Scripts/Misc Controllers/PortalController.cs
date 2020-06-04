using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{
    public PlayerInput playerInput;
    public PlayerController playerController;
    public Canvas worldSpaceUI;

    public bool canTransport = false;
    public bool toHub = false;
    public bool toGrassland = false;
    public bool toDesert = false;
    public bool toTundra = false;

    public void Awake()
    {
        if (worldSpaceUI.enabled)
        {
            worldSpaceUI.enabled = false;
        }
    }

    void Update()
    {
        if (canTransport && playerInput.interactKey)
        {
            playerController.SavePlayer();
            UsePortal();
        }
    }

    void OnTriggerEnter(Collider other) {
        worldSpaceUI.enabled = true;
        if (other.gameObject.tag == "Player") {
            canTransport = true;
        } else {
            canTransport = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        worldSpaceUI.enabled = false;
        if (other.gameObject.tag == "Player") {
            canTransport = false;
        }
    }

    void UsePortal()
    {
        if (toHub)
        {
            SceneManager.LoadScene("Hub");
        }
        if (toGrassland)
        {
            SceneManager.LoadScene("Grassland");
        }
        if (toDesert)
        {
            SceneManager.LoadScene("Desert");
        }
        if (toTundra)
        {
            SceneManager.LoadScene("Tundra");
        }
    }
}
