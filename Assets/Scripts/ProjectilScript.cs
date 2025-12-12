using UnityEngine;

// Adjuntar esto al Prefab del proyectil
public class ProjectileScript : MonoBehaviour
{
    public float lifespan = 3f; // Tiempo antes de que el proyectil se autodestruya
    public int damageAmount = 1; // Daño que inflige

    void Start()
    {
        // Destruye el proyectil después de su tiempo de vida
        Destroy(gameObject, lifespan);
    }

    // Manejar la colisión
    private void OnTriggerEnter(Collider other)
    {
        // Ejemplo: Si golpea a un enemigo (asumiendo que tiene un script EnemyHealth)
        // Puedes refinar esta lógica según tu juego.

        //EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();//
        //f (enemyHealth != null)
        {
            // Opcional: llama a la función de daño del enemigo
            // enemyHealth.TakeDamage(damageAmount); 
            Destroy(gameObject); // Destruye el proyectil al golpear
            return;
        }

        // Si golpea cualquier otra cosa que no sea el jugador o un proyectil
        if (!other.CompareTag("Player") && !other.isTrigger)
        {
            Destroy(gameObject); // Destruye el proyectil
        }
    }
}
