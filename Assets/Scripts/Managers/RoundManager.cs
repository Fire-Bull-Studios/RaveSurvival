using UnityEngine;
using RaveSurvival;
using System;
using static RaveSurvival.GameManager;

public class RoundManager : MonoBehaviour
{
    private Difficulty difficulty;
    private GameManager gameManager;
    private int round = 0;
    private EndlessSpawnManager spawnManager = null;
    public static RoundManager CreateComponent(GameObject _gameObject, Difficulty difficulty, GameManager gm)
    {
        RoundManager rm = _gameObject.AddComponent<RoundManager>();
        rm.difficulty = difficulty;
        rm.gameManager = gm;
        DebugManager.Instance.Print($"Difficulty is set to ${rm.difficulty}", DebugManager.DebugLevel.Verbose);
        return rm;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnManager = EndlessSpawnManager.CreateComponent(gameObject, difficulty, gameManager);

        StartNextRound();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private int StartNextRound()
    {
        round++;
        spawnManager.SpawnEnemies(round);
        return round;
    }
}
