using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health;

    // Patrolling
    //public Vector3 walkPoint;

    // private bool walkPointSet;
    // public float walkPointRange;

    // Attacking
    // public float timeBetweenAttacks;

    private bool alreadyAttacked;

    // States
    public float sightRange, attackRange;

    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Main Camera").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Check if player is in sight and attack range
        // playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // if (!playerInSightRange && !playerInAttackRange) Patrolling();
        // if (playerInSightRange && !playerInAttackRange) 
        ChasePlayer();
        if (playerInAttackRange) AttackPlayer();
    }

    // private void Patrolling()
    // {
    //     if (!walkPointSet) SearchWalkPoint();

    //     if (walkPointSet)
    //         agent.SetDestination(walkPoint);

    //     Vector3 distanceToWalkPoint = transform.position - walkPoint;

    //     // Walkpoint reached
    //     if (distanceToWalkPoint.magnitude < 1f)
    //         walkPointSet = false;
    // }

    // private void SearchWalkPoint()
    // {
    //     // Calculate random point in range
    //     float randomZ = Random.Range(-walkPointRange, walkPointRange);
    //     float randomX = Random.Range(-walkPointRange, walkPointRange);

    //     walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

    //     if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
    //     {
    //         //walkPoint = true;
    //     }
    // }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            // Attack code here
            

            DestroyEnemy();

            // alreadyAttacked = true;
            // Invoke(nameof(ResetAttack));
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamge(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void Temp()
    {
        
    }

}