using UnityEngine;

namespace RaveSurvival
{
    public class AmmoBox : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            DebugManager.Instance.Print(other.gameObject.layer.ToString(), DebugManager.DebugLevel.Paul);
            if (other.gameObject.tag.ToLower() == "player")
            {

                Player player = other.gameObject.GetComponent<Player>();
                if (player)
                {
                    foreach (var g in player.guns)
                    {
                        var hasAmmo = g.AddAmmo();
                        if (!hasAmmo)
                        {
                            break;
                        }
                    }
                    player.uIManager.SetAmmoText($"{player.gun.magazineAmmo} / {player.gun.totalAmmo}");
                }
            }
        }
    }
}