using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAiWaypoint : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform targetPoint;
    public List<Transform> waypoint;
    public List<Transform> players;
    public int waypointsReached;
    //public Transform player;
    public Transform nearestPlayer;
    public Transform spawnProjectile;

    ///public LayerMask whatIsGround, whatIsPlayer;

    public float health;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;
    public GameManager gameManager;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        agent = GetComponent<NavMeshAgent>();
        getActiveEnemies();
        foreach (Transform element in gameManager.Waypoints)
        {
            waypoint.Add(element);
        }
    }

    private void getActiveEnemies()
    {
        players.Clear();
        if (gameManager.Enemies.Count > 1)
        {
            foreach (Transform element in gameManager.Enemies)
            {
                if (element.name == this.gameObject.transform.name)
                {
                    continue;
                }
                else
                {
                    players.Add(element);
                }
            }
            nearestPlayer = players[0];            
        } else
        {
            nearestPlayer = null;
        }        
    }

    private void getNearestPlayer()
    {
        if (players != null)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (Vector3.Distance(transform.position, nearestPlayer.position) > Vector3.Distance(transform.position, players[i].position))
                {
                    nearestPlayer = players[i];
                }
            }
        }
    }

    private void Update()
    {
        getActiveEnemies();
        // Check for sight and attack range
        //playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        //Debug.Log(Vector3.Distance(transform.position, player.position));
        //Debug.Log(sightRange);
        //playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        /*for (int i = 0; i < players.Length; i++)
        {
            if (nearestPlayer != null)
            {
                if (Vector3.Distance(transform.position, nearestPlayer.position) < Vector3.Distance(transform.position, players[0].position))
                {
                    nearestPlayer = players[0];
                }
                if (Vector3.Distance(transform.position, nearestPlayer.position) < sightRange) playerInSightRange = true;
            }
            else
            {
                for (int j = 0; j < players.Length; j++)
                {

                }
            }
        }*/
        try
        {
            if (players.Count != 0)
            {
                getNearestPlayer();
                if (Vector3.Distance(transform.position, nearestPlayer.position) < sightRange)
                {
                    playerInSightRange = true;
                }
                else
                {
                    playerInSightRange = false;
                }
                if (Vector3.Distance(transform.position, nearestPlayer.position) < attackRange)
                {
                    playerInAttackRange = true;
                }
                else
                {
                    playerInAttackRange = false;
                }
            }
            else
            {
                playerInSightRange = false;
                playerInAttackRange = false;
            }
        }   catch (MissingReferenceException e)
        {
            Debug.Log("Objeto ja destruido: " + e);
        }

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Se o NPC tentou um número máximo de vezes e ainda não encontrou um ponto válido, ele tenta novamente
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        //Calculate random point in range
        //float randomZ = Random.Range(-walkPointRange, walkPointRange);
        //float randomX = Random.Range(-walkPointRange, walkPointRange);

        //walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        //Debug.Log(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround));
        //if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        //    walkPointSet = true;
        if (waypointsReached == waypoint.Count)
        {
            waypointsReached = 0;
            SearchWalkPoint();
        } else
        {
            walkPointSet = true;
            walkPoint = waypoint[waypointsReached].position;
            waypointsReached++;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(nearestPlayer.position);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);
        //agent.speed = 0;
        //agent.acceleration = 0;

        transform.LookAt(nearestPlayer);

        if (!alreadyAttacked)
        {
            /// Attack code here
            Rigidbody rb = Instantiate(projectile, spawnProjectile.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up, ForceMode.Impulse);

            ///
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), .5f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
        gameManager.removeEnemy(this.name);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
