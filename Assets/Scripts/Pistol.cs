using System.Diagnostics;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    [Header("Megaman Shot Settings")]
    public GameObject projectilePrefab; // Prefab del proyectil (el "lemon shot")
    public Transform firePoint;         // Punto donde se instancia el proyectil (ej. la punta del arma/cámara)
    public float fireRate = 0.5f;       // Tasa de disparo (ej. 0.5 segundos entre disparos)
    public float projectileSpeed = 20f; // Velocidad del proyectil

    private float nextFireTime = 0f;

    // ----------------------------------------------------
    // La función llamada por PlayerStats
    // ----------------------------------------------------
    public void Shoot()
    {
        // Control de cadencia (Fire Rate)
        if (Time.time < nextFireTime)
        {
            return; // No puede disparar aún
        }

        // Actualiza el tiempo del próximo disparo
        nextFireTime = Time.time + fireRate;

        // 1. Instanciar el proyectil en el punto de disparo
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            // 2. Darle velocidad
            // Obtenemos el Rigidbody o el componente del proyectil
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Dispara hacia adelante del FirePoint
                rb.velocity = firePoint.forward * projectileSpeed;
            }
            // Si usas CharacterController (como MegamanX) y no Rigidbody:
            // projectile.GetComponent<ProjectileScript>()?.Launch(firePoint.forward, projectileSpeed);
        }
        else
        {
            Debug.LogError("Projectile Prefab o Fire Point no asignados en Pistol.");
        }
    }

    // Método que requiere PlayerStats al respawn
    public void ResetAmmoOnStart()
    {
        // En un arma Megaman, esto realmente no hace nada
        // ya que la munición es infinita, pero lo dejamos
        // para cumplir con la llamada de PlayerStats.
    }
}