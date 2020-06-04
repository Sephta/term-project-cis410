using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    public BoxCollider playerHitBox;
    public PlayerController pc;

    public void EnablePlayerHitBox()
    {
        playerHitBox.enabled = !playerHitBox.enabled;
    }

    public void DisablePlayerHitBox()
    {
        playerHitBox.enabled = !playerHitBox.enabled;
        pc.UpdatePlayerState(pc.prevState);
    }
}
