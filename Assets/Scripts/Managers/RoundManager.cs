using UnityEngine;
using RaveSurvival;
using System;
using static RaveSurvival.GameManager;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance = null;
    private Difficulty difficulty;
    private GameManager gameManager;
    private int round = 0;
    private EndlessSpawnManager spawnManager = null;
    private int enemyCount = 0;
    public static RoundManager CreateComponent(GameObject _gameObject, Difficulty difficulty, GameManager gm)
    {
        if (Instance == null)
        {

            Instance = _gameObject.AddComponent<RoundManager>();
            Instance.difficulty = difficulty;
            Instance.gameManager = gm;
            DebugManager.Instance.Print($"Difficulty is set to ${Instance.difficulty}", DebugManager.DebugLevel.Verbose);
            return Instance;
        }
        else
        {
            return Instance;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnManager = EndlessSpawnManager.CreateComponent(gameObject, difficulty, gameManager);

        StartNextRound();
    }

    public void SubscribeToEnemy(Enemy enemy)
    {
        Debug.Log($"Subscribe to enemy");
        enemy.DeathEvent += HandleDeathEvent;
    }

    private void HandleDeathEvent(object sender, EventArgs e)
    {
        enemyCount--;
        if (enemyCount <= 0)
        {
            StartNextRound();
        }
    }

    private int StartNextRound()
    {
        round++;
        enemyCount = spawnManager.SpawnEnemies(round);
        return round;
    }
}
