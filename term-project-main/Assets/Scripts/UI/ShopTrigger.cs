using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    [SerializeField]
    private UI_Shop shop = null;

    public AudioSource enterSound;
    public AudioSource exitSound;
    
    bool shouldPlay = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            shop.DisplayShop();

            if (shouldPlay)
            {
                shouldPlay = false;
                enterSound.PlayOneShot(enterSound.clip, 0.4f);
            }

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shop.HideShop();
            
            exitSound.PlayOneShot(exitSound.clip, 0.4f);
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            shouldPlay = true;
        }
    }
}
