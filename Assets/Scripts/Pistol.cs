
using UnityEngine;

public class Pistol : MonoBehaviour
{
    [Header("Megaman Shot Settings")]
    public GameObject projectilePrefab; // Prefab del proyectil (el "lemon shot")
    public Transform firePoint;         // Punto donde se instancia el proyectil (ej. la punta del arma/c�mara)
    public float fireRate = 0.5f;       // Tasa de disparo (ej. 0.5 segundos entre disparos)
    public float projectileSpeed = 20f; // Velocidad del proyectil

    private float nextFireTime = 0f;

    // ----------------------------------------------------
    // La funci�n llamada por PlayerStats
    // ----------------------------------------------------
    public void Shoot()
    {
        // Control de cadencia (Fire Rate)
        if (Time.time < nextFireTime)
        {
            return; // No puede disparar a�n
        }

        // Actualiza el tiempo del pr�ximo disparo
        nextFireTime = Time.time + fireRate;

        // 1. Instanciar el proyectil en el punto de disparo
        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // OBTENER Y ASIGNAR EL ORIGEN:
        ProjectileScript projectileScript = projectileGO.GetComponent<ProjectileScript>();
        if (projectileScript != null)
        {
            projectileScript.isPlayerProjectile = true; // 👈 Proyectil del jugador
        }
        // Si usas CharacterController (como MegamanX) y no Rigidbody:
        // projectile.GetComponent<ProjectileScript>()?.Launch(firePoint.forward, projectileSpeed);
    }
      
    

    
}