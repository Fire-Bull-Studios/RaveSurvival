using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using System.Linq;

namespace RaveSurvival
{
    public class GameManager : NetworkBehaviour
    {
        // Singleton instance of the GameManager
        public static GameManager Instance = null;

        // List of all players in the game
        public List<Player> players;

        public enum GameType
        {
            SinglePlayer,
            LocalMultiplayer,
            OnlineMultiplayer,
            Endless
        };

        public enum Difficulty
        {
            Peaceful,
            Easy,
            Medium,
            Hard,
            Excision
        }

        public GameType gameType;
        public Difficulty difficulty;

        public string starterScene;

        public GameObject[] enemyPrefabs;
        public GameObject playerPrefab;

        private RoundManager roundManger = null;

        /// <summary>
        /// Unity's Awake method, called when the script instance is being loaded.
        /// Ensures there is only one instance of the GameManager (singleton pattern).
        /// </summary>
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                SceneManager.sceneLoaded += OnSceneLoaded; //add callback function on scene transistion
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        void OnDestroy()
        {
            if (Instance == this)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            DebugManager.Instance.Print($"Scene name: {scene.name}; Starter name: {starterScene}", DebugManager.DebugLevel.Verbose);
            if (scene.name == starterScene)
            {
                if (gameType == GameType.Endless)
                {
                    roundManger = RoundManager.CreateComponent(gameObject, difficulty, this);
                }
                else if (gameType == GameType.SinglePlayer)
                {
                    SpawnManager.Instance.FindSpawnPoints();
                    SpawnManager.Instance.SpawnPlayers(gameType);
                    if (difficulty != Difficulty.Peaceful)
                    {
                        StartCoroutine(SpawnManager.Instance.SpawnEnemies());
                    }

                    //Music
                    MusicManager.Instance.SetSong(MusicManager.Instance.tracks.ToArray()[0]);
                    MusicManager.Instance.FindSpeakers();
                }
                else
                {
                    DebugManager.Instance.Print($"Everything other than Single Player and Endless will not work currently (L)", DebugManager.DebugLevel.Minimal);
                }
            }

            //Clean up bootstraper if it exists
            GameBootstraper gameBootstraper = FindFirstObjectByType<GameBootstraper>();
            if (gameBootstraper != null)
            {
                Destroy(gameBootstraper.gameObject);
            }
        }

        /// <summary>
        /// Populates the list of players by finding all Player objects in the scene.
        /// </summary>
        public void SetPlayerList()
        {
            // Find all Player objects in the scene and add them to the players list
            players = new List<Player>(FindObjectsByType<Player>(FindObjectsSortMode.None));
        }

        public GameObject[] GetEnemyPrefabs()
        {
            Debug.Log($"BRUHHHHH What is the prefab name: {enemyPrefabs.First().name}");
            return enemyPrefabs;
        }

        public GameObject GetPlayerPrefab()
        {
            return playerPrefab;
        }

        /// <summary>
        /// Sets the local camera for each player.
        /// Enables the camera only for the local player.
        /// </summary>
        public void SetLocalCamera()
        {
            foreach (Player player in players)
            {
                // Enable the camera for the local player and disable it for others
                player.gameObject.transform.GetChild(1).GetComponent<Camera>().enabled = isLocalPlayer;
            }
        }

        public void SelectGameTypeAndLoad(int type)
        {
            gameType = (GameType)type;
            if (gameType == GameType.SinglePlayer)
            {
                MenuManager.Instance.OnSinglePlayerClicked();
            }
        }
    }
}