using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawning : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        CheckEnemyCount();
    }
    private void CheckEnemyCount() {
        if(GameObject.FindGameObjectsWithTag("Enemy").Length <= 0) {
            SceneManager.LoadScene(3);
        }
    }
}
