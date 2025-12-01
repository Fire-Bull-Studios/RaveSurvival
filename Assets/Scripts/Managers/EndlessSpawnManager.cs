using System;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.BaseCommands.Differences;
using RaveSurvival;
using UnityEngine;
using UnityEngine.Animations;
using static RaveSurvival.GameManager;
using static Spawn;

public class EndlessSpawnManager : MonoBehaviour
{
    private GameObject enemyPrefab;
    private GameObject playerPrefab;

    private Difficulty difficulty;
    private GameManager gameManager;
    public List<Spawn> playerSpawns = new();
    public List<Spawn> enemySpawns = new();
    public List<Spawn> bossSpawns = new();
    public static EndlessSpawnManager CreateComponent(GameObject _gameObject, Difficulty difficulty, GameManager gm)
    {
        EndlessSpawnManager spawnManager = _gameObject.AddComponent<EndlessSpawnManager>();
        spawnManager.difficulty = difficulty;
        spawnManager.gameManager = gm;
        return spawnManager;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
    //     enemyPrefab = gameManager.GetEnemyPrefabs().First();
    //     playerPrefab = gameManager.GetPlayerPrefab();
    //     GetSpawnPoints();
    //     SpawnPlayers();
    // }

    public void GetSpawnPoints()
    {
        GameObject parentSpawn = GameObject.FindGameObjectWithTag("SpawnPointParent");
        Debug.Log($"Spawn Point Parent: {parentSpawn}");
        foreach (Spawn spawn in parentSpawn.transform.GetComponentsInChildren<Spawn>())
        {
            if (spawn.GetSpawnUser() == SpawnUser.player)
            {
                playerSpawns.Add(spawn);
            }
            else if (spawn.GetSpawnUser() == SpawnUser.enemy)
            {
                enemySpawns.Add(spawn);
            }
            else if (spawn.GetSpawnUser() == SpawnUser.boss)
            {
                bossSpawns.Add(spawn);
            }
            else
            {
                DebugManager.Instance.Print($"Invalid spawn user type", DebugManager.DebugLevel.Production);
            }
        }
        SpawnPlayers();
    }

    public int SpawnEnemies(int round)
    {
        if (round == 1)
        {
            enemyPrefab = gameManager.GetEnemyPrefabs().First();
            playerPrefab = gameManager.GetPlayerPrefab();
            GetSpawnPoints();
        }
        if (difficulty == Difficulty.Peaceful)
        {
            return 0;
        }
        int enemyCount = 5 + (round * 4);
        for (int i = 0; i < enemyCount; i++)
        {
            int randomSpawn = UnityEngine.Random.Range(0, enemySpawns.Count - 1);
            float randomDelay = UnityEngine.Random.Range(0, 5.0f);
            GameObject[] temp = { enemyPrefab };
            Debug.Log($"Random Spawn: {randomSpawn}\tRandom Delay: {randomDelay}\ttemp: {enemyPrefab?.name}");
            enemySpawns[randomSpawn].SpawnCharacter(temp, randomDelay);
        }
        return enemyCount;
    }

    public void SpawnPlayers()
    {
        int rand = UnityEngine.Random.Range(0, playerSpawns.Count - 1);
        GameObject[] temp = { playerPrefab };
        Debug.Log($"Random Spawn: {rand}");
        playerSpawns[rand].SpawnCharacter(temp);
    }
}
