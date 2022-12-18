using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static AudioClip playerBullet;
    public static AudioClip enemyBullet;
    public static AudioClip death;
    public static AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        death = Resources.Load<AudioClip>("Power Hungry death");
        enemyBullet = Resources.Load<AudioClip>("Power Hungry enemy bullet");
        playerBullet = Resources.Load<AudioClip>("Power Hungry player bullet");
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static void PlaySound(string clip) {
        switch(clip) {
            case "playerBullet":
                audioSource.PlayOneShot(playerBullet);
                break;
            case "enemyBullet":
                audioSource.PlayOneShot(enemyBullet);
                break;
            case "death":
                audioSource.PlayOneShot(death);
                break;
        }
    }
}
