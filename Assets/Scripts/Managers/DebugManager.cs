using UnityEngine;

namespace RaveSurvival
{
    public class DebugManager : MonoBehaviour
    {

        public enum DebugLevel
        {
            Production, //no print statements
            Minimal, //crucial print statements
            Verbose, //crucial + major print statements
            Paul //tells you everything thats wrong
        };

        public static DebugManager Instance = null;
        public DebugLevel debugLevel;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Print(string message, DebugLevel debugLevel)
        {
            if (this.debugLevel == DebugLevel.Production)
                return;

            if ((int)debugLevel <= (int)this.debugLevel)
            {
                Debug.Log(message);
            }
        }
    }
}