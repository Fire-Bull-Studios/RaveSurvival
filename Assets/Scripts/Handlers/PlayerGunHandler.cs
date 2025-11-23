using UnityEngine;

namespace RaveSurvival
{
    public class PlayerGunHandler : MonoBehaviour
    {
        public Gun gun;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                gun.Reload();
            }
        }
    }

}

