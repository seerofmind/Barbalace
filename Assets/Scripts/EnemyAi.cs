using System.Diagnostics;
using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    
    [Header("Enemy Stats")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Detection & Movement")]
    public float detectionRange = 15f; // Radio para detectar al jugador
    public float attackRange = 10f;    // Distancia a la que empieza a disparar
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;

    [Header("Ranged Attack")]
    public GameObject projectilePrefab; // El prefab del proyectil enemigo
    public Transform firePoint;         // Punto desde donde se dispara el proyectil
    public float fireRate = 2.0f;       // Segundos entre disparos
    private float nextFireTime;

    // Referencia al jugador (usamos la Transform para la posición)
    private Transform playerTransform;
    private bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
        // 1. Encontrar al jugador al inicio (asumiendo que tiene el tag "Player")
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("No se encontró un objeto con el tag 'Player' en la escena.");
        }
    }

    void Update()
    {
        if (isDead || playerTransform == null)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange)
        {
            // 2. Mirar al jugador
            RotateTowardsPlayer();

            if (distanceToPlayer <= attackRange)
            {
                // 3. Atacar si está dentro del rango de ataque
                TryAttack();
            }
            else
            {
                // 4. Moverse hacia el jugador si está en rango de detección pero fuera de ataque
                MoveTowardsPlayer();
            }
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0; // Solo rotar en el plano horizontal (eje Y)
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void MoveTowardsPlayer()
    {
        // Calcular el movimiento para acercarse lentamente
        Vector3 moveDirection = (playerTransform.position - transform.position).normalized;
        moveDirection.y = 0; // Asegurar que no se mueva verticalmente (si es necesario)

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        // Opcional: Si usas un NavMeshAgent, aquí usarías agent.SetDestination(playerTransform.position);
    }

    private void TryAttack()
    {
        if (Time.time > nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            ShootProjectile();
        }
    }

    private void ShootProjectile()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            // Instanciar el proyectil y darle la misma rotación que el punto de disparo (que ya apunta al jugador)
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
        else
        {
            Debug.LogError("Projectile Prefab o Fire Point no asignados al enemigo.");
        }
    }

    // ---------------------- Health and Damage ----------------------

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        Debug.Log($"Enemy took {damageAmount} damage. HP remaining: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Enemy destroyed!");

        // Deshabilitar lógica o componentes inmediatamente
        // GetComponent<Collider>().enabled = false;
        // GetComponent<Renderer>().enabled = false;

        // Destruir el objeto después de un breve momento (para animaciones de muerte, etc.)
        Destroy(gameObject, 0.5f);
    }

}
