using UnityEngine;

// Asegúrate de que el Prefab tenga un Rigidbody y un Collider marcado como Is Trigger
public class ProjectileScript : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float lifespan = 3f;      // Tiempo antes de que el proyectil se autodestruya
    public int damageAmount = 1;     // Daño que inflige

    // **CLAVE:** Determina quién disparó el proyectil.
    // Esto se debe asignar en el script del arma (Pistol o RangedEnemy) al instanciar el proyectil.
    public bool isPlayerProjectile = true;

    void Start()
    {
        // Destruye el proyectil después de su tiempo de vida
        Destroy(gameObject, lifespan);
    }

    // Manejar la colisión
    private void OnTriggerEnter(Collider other)
    {
        // Si golpea un objeto con un Collider que no es un trigger
        if (!other.isTrigger)
        {
            // Intentamos obtener componentes de salud en el objeto golpeado

            // ----------------------------------------------------
            // 1. LÓGICA DEL PROYECTIL DEL JUGADOR (isPlayerProjectile = true)
            // ----------------------------------------------------
            if (isPlayerProjectile)
            {
                // Solo intenta dañar a los Enemigos
                // Asumimos que los enemigos tienen el tag "Enemy" y el script RangedEnemy
                if (other.CompareTag("Enemy"))
                {
                    EnemyAi enemyHealth = other.GetComponent<EnemyAi>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(damageAmount);
                        Destroy(gameObject); // Destruye el proyectil al golpear
                        return;
                    }
                }
            }

            // ----------------------------------------------------
            // 2. LÓGICA DEL PROYECTIL DEL ENEMIGO (isPlayerProjectile = false)
            // ----------------------------------------------------
            else // Es un proyectil de enemigo
            {
                // Solo intenta dañar al Jugador
                // Asumimos que el jugador tiene el tag "Player" y el script PlayerStats/Controller
                if (other.CompareTag("Player"))
                {
                    // Nota: Usamos PlayerController. Si usas PlayerStats, cámbialo.
                    PlayerController playerHealth = other.GetComponent<PlayerController>();

                    if (playerHealth != null)
                    {
                        // Necesitas añadir un método TakeDamage(int) al PlayerController
                        // playerHealth.TakeDamage(damageAmount); 
                        Debug.Log("Proyectil enemigo golpeó al jugador. Daño: " + damageAmount);
                        Destroy(gameObject); // Destruye el proyectil al golpear
                        return;
                    }
                }
            }

            // ----------------------------------------------------
            // 3. LÓGICA DE DESTRUCCIÓN POR IMPACTO GENERAL
            // ----------------------------------------------------

            // Si golpea cualquier cosa (paredes, suelo, etc.) que no sea el atacante o un trigger
            // Destruimos el proyectil.
            Destroy(gameObject);
        }
    }
}
