using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitDetection : MonoBehaviour
{
    [Header("HitBox")]
    public BoxCollider enemyHitBox;


    private PlayerController pc;


    public void HitBoxStartFrame()
    {
        enemyHitBox.enabled = true;
    }

    public void HitBoxEndFrame()
    {
        enemyHitBox.enabled = false;
    }
}
