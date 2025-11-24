using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Mirror;
using UnityEditor.Build;

namespace RaveSurvival
{
    public class Enemy : Entity
    {
        public float shootRange;
        public float walkRange;
        private NavMeshAgent agent;
        private Transform target = null;
        private bool playerSpotted = false;
        private IEnumerator behaviorCo = null;
        private bool hitObstacle = false;
        private EnemyState enemyState = EnemyState.IDLE;
        private EnemyAlert megaphone = null;
        public Gun gun;

        public event EventHandler DeathEvent;

        public enum EnemyState
        {
            IDLE,
            WANDER,
            CHASE,
            ATTACK,
            DEAD
        };

        public override void Start()
        {
            base.Start();
            RoundManager.Instance.SubscribeToEnemy(this);
            agent = GetComponent<NavMeshAgent>();
            megaphone = GetComponent<EnemyAlert>();
            StartAction();
        }

        void OnCollisionEnter(Collision col)
        {
            Debug.Log($"Collision was made!!!! {col.gameObject.name}");
            if (col.gameObject.layer == LayerMask.NameToLayer("Obstruction"))
            {
                hitObstacle = true;
            }
        }

        void OnCollisionExit(Collision col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Obstruction"))
            {
                hitObstacle = false;
            }
        }

        public void ChangeState(EnemyState state)
        {
            if (state == enemyState)
            {
                return;
            }
            if (behaviorCo != null)
            {
                StopAllCoroutines();
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
                    target = null;
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
                    break;
                default:
                    Debug.LogError($"Invalid state passed ({enemyState}). Kinda cringe if you ask me.");
                    break;
            }
        }

        public IEnumerator AttackPlayer(Transform player)
        {
            float wait = 0.25f;
            while (true)
            {
                if (Vector3.Distance(player.position, transform.position) > walkRange)
                {
                    MoveToPlayer(player);
                }
                else
                {
                    if (agent != null && agent.isOnNavMesh)
                    {
                        agent.ResetPath();
                    }
                }

                if (Vector3.Distance(player.position, transform.position) <= shootRange)
                {
                    ShootPlayer(player);
                }
                yield return wait;
            }
        }

        public void PlayerSpotted(Transform player)
        {
            playerSpotted = true;
            if (target != player)
            {
                target = player;
                ChangeState(EnemyState.ATTACK);
                megaphone.AlertNearEnemies(target);
            }
        }

        public void NoPlayerFound()
        {
            playerSpotted = false;
            if (behaviorCo != null)
            {
                IEnumerator delay = DelayedStop(5f);
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
            //enemyState = EnemyState.CHASE;
            transform.LookAt(player);
            agent.SetDestination(player.position);
        }

        private void ShootPlayer(Transform player)
        {
            transform.LookAt(player);
            gun.transform.LookAt(player);
            if (GameManager.Instance.gameType == GameManager.GameType.OnlineMultiplayer)
            {
                gun.OnlineFire(Time.time);
            }
            else
            {
                gun.Fire(Time.time);
            }
        }

        public override void TakeDamage(float dmg, Transform bulletDirection, Vector3 pos, Entity shotBy)
        {
            base.TakeDamage(dmg, bulletDirection, pos, shotBy);
            transform.LookAt(bulletDirection);
            if (agent != null)
            {
                agent.SetDestination(pos);
            }
        }

        protected override void Die(String shotBy)
        {
            base.Die(shotBy);
            DeathEvent?.Invoke(this, EventArgs.Empty);
            ChangeState(EnemyState.DEAD);
            Destroy(gameObject);
        }

        private IEnumerator DelayedStop(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            if (!playerSpotted)
            {
                ChangeState(EnemyState.IDLE);
            }
            yield return null;
        }

        private IEnumerator BecomeIdle()
        {
            yield return new WaitForSeconds(5f);
            ChangeState(EnemyState.WANDER);
        }

        private IEnumerator Wander()
        {
            Vector3 destination = setPath();
            while (gameObject.transform.position != destination)
            {
                if (hitObstacle)
                {
                    destination = setPath();
                }
                yield return null;
            }
            ChangeState(EnemyState.IDLE);
            yield return null;
        }

        private Vector3 setPath()
        {
            float randAngle = UnityEngine.Random.Range(-180f, 180f);
            transform.Rotate(transform.up, randAngle);
            int randDist = UnityEngine.Random.Range(10, 30);
            Vector3 destination = transform.localPosition + (transform.forward * randDist);
            agent.SetDestination(destination);
            return destination;
        }
    }
}

