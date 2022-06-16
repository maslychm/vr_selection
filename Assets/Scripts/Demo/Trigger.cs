using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public int health = 3;
    public bool isDead = false;

    [Header("Enemy spawning")]
    public List<GameObject> enemies;

    public float spawnEverySeconds = 2f;

    public GameObject endScreen;

    private void Start()
    {
        StartCoroutine(KeepSpawningEnemies());
    }

    private void Update()
    {
        if (health == 0 && !isDead)
        {
            // End screen or something, player loses
            isDead = true;
            print("GAME END");
            KillAllEnemies();

            // Ending Game
            endScreen.SetActive(true);
            Invoke(nameof(ExitGame), 10);
        }
    }

    private void KillAllEnemies()
    {
        DestroyParentOnPlayerHit[] enemyBodies = FindObjectsOfType<DestroyParentOnPlayerHit>();

        foreach (DestroyParentOnPlayerHit e in enemyBodies)
            e.EndGameDeathAfterDelay();
    }

    private IEnumerator KeepSpawningEnemies()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(spawnEverySeconds / 1.01f);

            try
            {
                if (!isDead)
                    SpawnRandomEnemy();
            }
            catch
            {
                print("bugging out in spawning enumerator");
            }
        }

        print("Spawning ended");
    }

    private void SpawnRandomEnemy()
    {
        var randomEnemy = enemies[Random.Range(0, enemies.Count - 1)];

        // extents:
        // x (-12, 12)
        // y = 1
        // z = (5, 12)

        var spawnPosition = new Vector3(Random.Range(-12, 12), 1f, Random.Range(5, 12));

        Instantiate(randomEnemy, spawnPosition, Quaternion.identity);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("enemy"))
        {
            //print($"{other.tag} hit PLAYER");
            HealthDamage();
        }
    }

    private void HealthDamage()
    {
        // Add possible health damage affect
        health--;
    }

    private void ExitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
