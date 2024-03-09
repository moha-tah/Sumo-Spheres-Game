using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject superEnemyPrefab;
    public GameObject bossPrefab;
    public GameObject miniBossPrefab;

    public GameObject gameOverRects;
    public GameObject roundRect;

    public TextMeshProUGUI roundText;
    public GameObject recordText;

    public GameObject[] powerupPrefabs;

    public GameObject pauseText;

    public float rangeX;
    public float rangeZ;

    public float spawnDelay = 10;

    public int superEnemyProbability = 10;

    public int enemiesCount = 0;

    public bool gameOver = true;

    public bool roundFinish = false;

    public bool musicPlayed = false;

    private GameObject player;

    private AudioSource audioSource;

    public AudioClip suspensSound;
    public AudioClip winSound;
    public AudioClip looseSound;
    public AudioClip newRoundSound;

    public Color bossColor;
    public Color roundColor;

    public Scene multiplayer;

    // Start is called before the first frame update
    public void Start()
    {
        CancelInvoke();

        player = GameObject.Find("Player Sphere");

        audioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();

        roundText.text = "Meilleur score : " + PlayerPrefs.GetInt("Highest Score");
    }

    public void LaunchGame()
    {
        gameOver = false;

        InvokeRepeating("SpawnPowerup", 2, spawnDelay);

        GameObject.Find("Main Menu").SetActive(false);
        GameObject.Find("Slider").SetActive(false);
        GameObject.Find("Invisible Wall").SetActive(false);

        roundText.text = "Round n°1";

        pauseText.SetActive(true);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.y < -5 && !musicPlayed)
        {
            GameOver();
            audioSource.PlayOneShot(looseSound, 1f);
            musicPlayed = true;
            checkHighestScore();
        }

        if (countEnemiesZero() == 0 && !roundFinish && !gameOver && enemiesCount > 1)
        {
            audioSource.PlayOneShot(winSound, 0.8f);
            roundFinish = true;
        }

        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0 && !gameOver)
        {
            enemiesCount++;
            SpawnEnemies();
            roundFinish = false;

            if (enemiesCount > 1)
                audioSource.PlayOneShot(newRoundSound, 1f);
        }
    }

    Vector3 randomSpawnPosition()
    {
        float x = Random.Range(-rangeX, rangeX);
        float z = Random.Range(-rangeZ, rangeZ);

        return new Vector3(x, 2, z);
    }

    void SpawnEnemies()
    {
        CancelInvoke("SpawnMiniBoss");

        GameObject enemy;

        if (enemiesCount % 5 == 0)
        {
            enemy = bossPrefab;
            audioSource.PlayOneShot(suspensSound, 0.8f);

            Instantiate(enemy, randomSpawnPosition(), enemy.transform.rotation);

            InvokeRepeating("SpawnMiniBoss", 1, 2);

            roundText.text = "Boss n°" + (enemiesCount%5 + 1);

            roundRect.GetComponent<Image>().color = bossColor; 
        }
        else
        {
            for (int i = 0; i < Mathf.Min(6, enemiesCount); i++)
            {
                int seed = Random.Range(0, Mathf.Max(2, superEnemyProbability - enemiesCount % 3));

                if (seed != 0)
                {
                    enemy = enemyPrefab;
                }
                else
                {
                    enemy = superEnemyPrefab;
                }

                GameObject spawnedEnemy = Instantiate(enemy, randomSpawnPosition(), enemy.transform.rotation);

                spawnedEnemy.GetComponent<EnemyController>().impulseForce += (enemiesCount * 3);

                if (enemiesCount >= 6)
                    spawnedEnemy.GetComponent<EnemyController>().impulseForce += (enemiesCount * 2);
            }

            roundText.text = "Round n°" + enemiesCount;
            roundRect.GetComponent<Image>().color = roundColor;
        }


    }

    void SpawnPowerup()
    {
        if (GameObject.Find("Player Sphere").GetComponent<PlayerController>().powerup == "Null")
        {
            int index = Random.Range(0, powerupPrefabs.Length);
            Instantiate(powerupPrefabs[index],
                        randomSpawnPosition(),
                        powerupPrefabs[index].transform.rotation);
        }

        if (gameOver)
            CancelInvoke("SpawnPowerup");
    }

    void SpawnMiniBoss()
    {
        Instantiate(miniBossPrefab, randomSpawnPosition(), miniBossPrefab.transform.rotation);
        Instantiate(miniBossPrefab, randomSpawnPosition(), miniBossPrefab.transform.rotation);
    }

    int countEnemiesZero()
    {
        int compteur = 0;

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (enemy.transform.position.y > 0)
                compteur++;
        }

        return compteur;
    }

    void checkHighestScore()
    {
        if (enemiesCount > PlayerPrefs.GetInt("Highest Score"))
        {
            PlayerPrefs.SetInt("Highest Score", enemiesCount);
            recordText.SetActive(true);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = GameObject.Find("Pause Manager").GetComponent<PauseManager>().timeScale;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMultiplayer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void GameOver()
    {
        gameOver = true;
        gameOverRects.SetActive(true);
    }
}