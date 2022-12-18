using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterControls : MonoBehaviour
{
    //Values for controlling health
    public HealthBarScript healthBar;
    public int maxHealth = 20;
    public int currentHealth;
    //Values for shooting a bullet.
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletForce = 10f;
    
    private void Start() {
        currentHealth = maxHealth;   
        healthBar.SetMaxHealth(maxHealth); 
        StartCoroutine(DecreaseHealth());
    }
    private void Update() {
        if(Input.GetButtonDown("Fire2")) {
            Shoot();
        } 
    }

    private void Shoot() {
        SoundManager.PlaySound("playerBullet");
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb =  bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
    }

    //Coroutine decreases health after a constant time
    IEnumerator DecreaseHealth() {
        while(currentHealth > 0) {
            currentHealth--;
            healthBar.SetHealth(currentHealth);
            yield return new WaitForSeconds(2f);
        }
        SoundManager.PlaySound("death");
        SceneManager.LoadScene(2);
        Destroy(gameObject);
    }
}
