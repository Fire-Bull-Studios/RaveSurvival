using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

namespace RaveSurvival
{
	public class Enemy : NetworkBehaviour
	{
		public float health = 50f;
		public float range = 10f;
		private NavMeshAgent agent;
		private Transform target = null;
    private IEnumerator behaviorCo = null;
    private bool hitObstacle = false;
    private EnemyState enemyState = EnemyState.IDLE;
    public Gun gun;

		public enum EnemyState
		{
			IDLE,
			WANDER,
			CHASE,
			ATTACK,
			DEAD
		};

    public void Start()
    {
      agent = GetComponent<NavMeshAgent>();
      StartAction();
    }

    public void ChangeState(EnemyState state)
    {
      if (state == enemyState)
      {
        return;
      }
      if (behaviorCo != null)
      {
        StopCoroutine(behaviorCo);
        behaviorCo = null;
      }
      enemyState = state;
      StartAction();
    }
    
    public void StartAction()
    {
      switch (enemyState)
      {
        case EnemyState.IDLE:
          behaviorCo = BecomeIdle();
          StartCoroutine(behaviorCo);
          break;
        case EnemyState.WANDER:
          behaviorCo = Wander();
          StartCoroutine(behaviorCo);
          break;
        case EnemyState.CHASE:
          MoveToPlayer(target);
          break;
        case EnemyState.ATTACK:
          behaviorCo = AttackPlayer(target);
          StartCoroutine(behaviorCo);
          break;
        case EnemyState.DEAD:
          Die();
          break;
        default:
          Debug.LogError($"Invalid state passed ({enemyState}). Kinda cringe if you ask me.");
          break;
      }
    }

		public void PlayerSpotted(Transform player)
		{
			if (target != player && behaviorCo == null)
      {
				target = player;
        ChangeState(EnemyState.ATTACK);
			}
		}

		public IEnumerator AttackPlayer(Transform player)
		{
			float wait = 0.25f;
			while (true)
			{
				if (Vector3.Distance(player.position, transform.position) > range)
				{
					MoveToPlayer(player);
				}
				else
				{
					ShootPlayer(player);
				}
				yield return wait;
			}
		}
    public void NoPlayerFound()
    {
      if (behaviorCo != null)
      {
        IEnumerator delay = DelayedStop(2f);
        StartCoroutine(delay);
      }

    }
    public void HitObstacle(bool x)
    {
      hitObstacle = x;
      if (x)
      {
        Debug.Log($"{name} hit obstacle");
      }
      else
      {
        Debug.Log($"{name} did not hit obstacle");
      }
    }

		private void MoveToPlayer(Transform player)
		{
			enemyState = EnemyState.CHASE;
			transform.LookAt(player);
			agent.SetDestination(player.position);
		}

		private void ShootPlayer(Transform player)
		{
			enemyState = EnemyState.ATTACK;
			agent.ResetPath();
			transform.LookAt(player);
			if (GameManager.Instance.gameType == GameManager.GameType.OnlineMultiplayer)
			{
				gun.OnlineShoot(true);
			}
			else
			{
				gun.SinglePlayerShoot(true);
			}
		}

		public void TakeDamage(float dmg, Transform bulletDirection)
		{
			transform.LookAt(bulletDirection);
			health -= dmg;
			if (health <= 0f)
			{
				Die();
			}
		}

		void Die()
		{
			Destroy(gameObject);
		}

    private IEnumerator DelayedStop(float seconds)
    {
      yield return new WaitForSeconds(seconds);
      ChangeState(EnemyState.IDLE);
    }

    private IEnumerator BecomeIdle()
    {
      Debug.Log($"{gameObject.name} become idle");
      yield return new WaitForSeconds(5f);
      ChangeState(EnemyState.WANDER);
    }

    private IEnumerator Wander()
    {
      Debug.Log($"{gameObject.name} start wandering");
      Vector3 destination = setPath();
      while (gameObject.transform.position != destination)
      {
        if (hitObstacle)
        {
          setPath();
        }
        yield return null;
      }
      ChangeState(EnemyState.IDLE);
      yield return null;
    }
    
    private Vector3 setPath()
    {
      transform.Rotate(transform.forward, 90f);
      int rand = UnityEngine.Random.Range(10, 30);
      Vector3 destination = transform.localPosition + (transform.forward * rand);
      agent.SetDestination(destination);
      return destination;
    }
	}
}

