using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    // --- VARIABLES DE ATAQUE MELEE ---
    [Header("Melee Attack Settings")]
    public float meleeAttackRange = 1.5f; // Rango muy corto para golpear
    public int damageAmount = 50;
    public float attackRate = 1.5f;        // Segundos entre ataques melee
    private float nextAttackTime;          // Control de cadencia

    // --- Variables de Enemy Stats ---
    [Header("Enemy Stats")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Detection & Movement")]
    public float detectionRange = 15f;
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;

    // --- Ranged Attack (Removido) ---
    // REMOVIDO: public Transform firePoint;
    // REMOVIDO: public GameObject projectilePrefab;

    // Referencia al jugador
    private Transform playerTransform;
    private bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("No se encontró un objeto con el tag 'Player' en la escena.");
        }

        // REMOVIDO: Lógica de LineRenderer y LayerMask
    }

    void Update()
    {
        if (isDead || playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange)
        {
            // Siempre mira al jugador en el rango de detección
            RotateTowardsPlayer();

            if (distanceToPlayer <= meleeAttackRange) // 👈 Usamos el rango Melee
            {
                // 3. Atacar si está dentro del rango de ataque cuerpo a cuerpo
                TryAttack();
            }
            else
            {
                // 4. Moverse hacia el jugador para acortar distancia
                MoveTowardsPlayer();
            }
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void MoveTowardsPlayer()
    {
        // Se mueve hasta estar justo al borde del rango de ataque melee
        Vector3 targetPosition = playerTransform.position - (playerTransform.position - transform.position).normalized * meleeAttackRange;
        targetPosition.y = transform.position.y; // Mantener la altura del enemigo

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    private void TryAttack()
    {
        // Control de cadencia de ataque
        if (Time.time > nextAttackTime)
        {
            nextAttackTime = Time.time + attackRate;
            PerformMeleeAttack(); // 👈 Nuevo método de ataque
        }
    }

    private void PerformMeleeAttack()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) <= meleeAttackRange)
        {
            // 2. Obtener el script del jugador
            PlayerController playerHealth = playerTransform.GetComponent<PlayerController>();

            if (playerHealth != null)
            {
                // 3. ¡Llamar al método!
                playerHealth.TakeDamage(damageAmount);
            }
            else
            {
                // Esto es CRUCIAL para depurar
                Debug.LogError("El PlayerController no se encontró en el objeto del jugador.");
            }
        }

        // Opcional: Aquí puedes activar una animación de golpe.
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

        Destroy(gameObject, 0.5f);
    }
}
