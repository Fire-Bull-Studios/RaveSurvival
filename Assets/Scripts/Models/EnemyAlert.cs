using RaveSurvival;
using UnityEngine;

public class EnemyAlert: MonoBehaviour
{
  public float alertDistance;
  private Collider[] colliders = new Collider[5];
  public LayerMask layers;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    layers = gameObject.layer;
  }

  public void AlertNearEnemies(Transform player)
  {
    int count = Physics.OverlapSphereNonAlloc(transform.position, alertDistance, colliders, layers, QueryTriggerInteraction.Collide);
    for (int i = 0; i < count; i++)
    {
      Debug.Log(colliders[i].gameObject.name);
      Enemy enemy = colliders[i].gameObject.GetComponent<Enemy>();
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
