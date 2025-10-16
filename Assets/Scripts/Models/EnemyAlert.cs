using RaveSurvival;
using UnityEngine;

public class EnemyAlert: MonoBehaviour
{
  public float alertDistance;
  public LayerMask layers;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    layers = gameObject.layer;
  }

  public void AlertNearEnemies(Transform player)
  {
    Collider[] cols = Physics.OverlapSphere(transform.position, alertDistance, layers, QueryTriggerInteraction.Collide);
    foreach (Collider col in cols)
    {
      Enemy enemy = col.gameObject.GetComponent<Enemy>();
      if (enemy != null)
      {
        enemy.PlayerSpotted(player);
      }
    }
  }
  private void OnDrawGizmos()
  {
    Gizmos.DrawWireSphere(transform.position, alertDistance);
    Gizmos.color = Color.darkOrange;
  }
}
