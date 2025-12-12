using UnityEngine;

namespace RaveSurvival
{
    public class AmmoBox : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            DebugManager.Instance.Print(other.gameObject.layer.ToString(), DebugManager.DebugLevel.Paul);
            if (other.gameObject.layer == Player.PlayerLayer)
            {

                Player player = other.gameObject.GetComponent<Player>();
                if (player && player.gun.AddAmmo())
                {
                    player.uIManager.SetAmmoText($"{player.gun.magazineAmmo} / {player.gun.totalAmmo}");
                }
            }
        }
    }
}