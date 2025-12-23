using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState { Idle, Patrol, Chase, Attack }

    public EnemyState currentState = EnemyState.Patrol;
    public float patrolSpeed = 2.5f;
    public float chaseSpeed = 5.0f;
    public float noiseThreshold = 60f;
    public float attackDistance = 3.0f;
    public float wanderRadius = 15f;
    private Animator _animator;

    public float damage = 2000f;
    public float attackCooldown = 2.0f;
    private float _lastAttackTime;

    public Transform player;
    public AudioSource audioSource;
    public AudioClip screamClip;

    private NavMeshAgent _agent;
    private float _idleTimer;
    private bool _hasScreamed = false;

    public AudioSource footstepSource;
    public AudioClip footstepLoopClip;

    [Range(0.5f, 3.0f)] public float walkPitch = 0.8f;
    [Range(0.5f, 3.0f)] public float runPitch = 1.3f;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (player == null && GameObject.FindGameObjectWithTag("Player"))
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (footstepSource && footstepLoopClip)
        {
            footstepSource.clip = footstepLoopClip;
            footstepSource.loop = true;
            footstepSource.playOnAwake = false;
        }

        ChangeState(EnemyState.Patrol);
    }

    private void Update()
    {
        UpdateAnimations();
        if (!_agent.isOnNavMesh) return;

        HandleFootstepsLoop();
        switch (currentState)
        {
            case EnemyState.Idle: HandleIdle(); break;
            case EnemyState.Patrol: HandlePatrol(); break;
            case EnemyState.Chase: HandleChase(); break;
            case EnemyState.Attack: HandleAttack(); break;
        }

        CheckNoiseLevel();
    }

    private void UpdateAnimations()
    {
        if (_animator == null) return;

        if (currentState == EnemyState.Attack)
        {
            _animator.SetFloat("Speed", 0f);
            return;
        }


        float currentSpeed = _agent.velocity.magnitude;

        float animValue = currentSpeed / chaseSpeed;
        _animator.SetFloat("Speed", animValue, 0.1f, Time.deltaTime);
    }

    private void HandleIdle() { _idleTimer -= Time.deltaTime; if (_idleTimer <= 0) ChangeState(EnemyState.Patrol); }
    private void HandlePatrol() { _agent.speed = patrolSpeed; if (!_agent.pathPending && _agent.remainingDistance < 0.5f) { Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1); _agent.SetDestination(newPos); } }

    private void HandleChase()
    {
        _agent.speed = chaseSpeed;

        if (player != null)
        {
            _agent.SetDestination(player.position);

            float dist = Vector3.Distance(transform.position, player.position);
            if (dist <= attackDistance)
            {
                ChangeState(EnemyState.Attack);
            }
        }
    }

    private void HandleAttack()
    {
        Vector3 targetPostition = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(targetPostition);

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > attackDistance + 1.0f)
        {
            ChangeState(EnemyState.Chase);
            return;
        }
        if (Time.time > _lastAttackTime + attackCooldown)
        {
            PerformStrike();
            _lastAttackTime = Time.time;
        }
    }

    private void PerformStrike()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Attack");
        }

        var carScript = player.GetComponent<CarHandler>();
        if (carScript != null)
        {
            carScript.TakeDamage(damage);
        }
    }

    private void CheckNoiseLevel()
    {
        if (currentState == EnemyState.Attack) return;
        if (NoiseManager.Instance != null)
        {
            if (NoiseManager.Instance.currentNoiseLevel > noiseThreshold)
            {
                if (currentState != EnemyState.Chase) ChangeState(EnemyState.Chase);
            }
            else
            if (currentState == EnemyState.Chase && Vector3.Distance(transform.position, player.position) > 20f)
            {
                ChangeState(EnemyState.Patrol);
            }
        }
    }

    private void ChangeState(EnemyState newState)
    {
        currentState = newState;
        if (!_agent.isOnNavMesh) return;

        switch (newState)
        {
            case EnemyState.Idle: _agent.isStopped = true; _idleTimer = 2.0f; break;
            case EnemyState.Patrol: _agent.isStopped = false; _hasScreamed = false; _agent.SetDestination(RandomNavSphere(transform.position, wanderRadius, -1)); break;

            case EnemyState.Chase:
                _agent.isStopped = false;
                if (!_hasScreamed && audioSource && screamClip) { audioSource.PlayOneShot(screamClip); _hasScreamed = true; }
                break;

            case EnemyState.Attack:
                _agent.isStopped = true;
                break;
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask); return navHit.position;
    }


    private void HandleFootstepsLoop()
    {
        if (footstepSource == null) return;
        bool isMoving = _agent.velocity.sqrMagnitude > 0.1f && currentState != EnemyState.Attack;

        if (isMoving)
        {
            if (!footstepSource.isPlaying)
            {
                footstepSource.Play();
            }
            float targetPitch = (currentState == EnemyState.Chase) ? runPitch : walkPitch;

            footstepSource.pitch = Mathf.Lerp(footstepSource.pitch, targetPitch, Time.deltaTime * 5f);
        }
        else
        {
            if (footstepSource.isPlaying)
            {
                footstepSource.Pause();
            }
        }
    }
}