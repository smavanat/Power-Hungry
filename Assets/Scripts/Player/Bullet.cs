using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Enemy") {
            SoundManager.PlaySound("death");
            Destroy(other.gameObject);
        }
        Destroy(gameObject);
    }
}
