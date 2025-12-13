
using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.GlobalIllumination;

public class Pistol : MonoBehaviour
{
    [Header("Raycast Shot Settings")]
    public Transform firePoint;         // Punto desde donde se origina el rayo (ej. la punta del arma/cámara)
    public float fireRate = 0.5f;       // Tasa de disparo (segundos entre disparos)
    public int damageAmount = 1;        // Daño que inflige el rayo
    public float range = 50f;           // Alcance máximo del rayo

    [Header("Visuals (Opcional)")]
    public GameObject hitEffectPrefab;  // Prefab para mostrar el impacto (ej. una chispa)
    public LayerMask hitMask;
    
    
   

    [Header("Line Renderer Visuals")]
    public LineRenderer lineRenderer;
    public float lineDisplayTime = 0.05f;

    [Header("Automatic LR Styling")]
    public Material lineMaterial; // Asigna un material brillante (ej. Unlit Color) en el Inspector
    public Color lineColor = Color.blue;
    public float lineWidth = 0.05f;

    private float nextFireTime = 0f;

    void Awake()
    {
        // 1. Obtener o Añadir el Line Renderer
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                // Si no existe, lo añadimos
                lineRenderer = gameObject.AddComponent<LineRenderer>();
                Debug.LogWarning("Line Renderer añadido automáticamente a " + gameObject.name);
            }
        }

        // 2. Configurar el Line Renderer por Código
        if (lineRenderer != null)
        {
            // Ajustar la geometría
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;

            // Ajustar el grosor
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth * 0.5f; // Lo hacemos más delgado al final

            // Ajustar el material y el color
            if (lineMaterial != null)
            {
                lineRenderer.material = lineMaterial;
                lineRenderer.startColor = lineColor;
                lineRenderer.endColor = lineColor;
            }
            else
            {
                // Si no se asigna un material, usa uno por defecto (menos ideal para láser)
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                lineRenderer.startColor = lineColor;
                lineRenderer.endColor = lineColor;
                Debug.LogWarning("Asigna un material 'Unlit' al Line Renderer para mejor efecto láser.");
            }

            // Ajustar el renderizado (para efectos láser/rayos)
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;

            // Asegurarse de que está deshabilitado al inicio
            lineRenderer.enabled = false;
        }
    }
    public void Shoot()
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + fireRate;

        Vector3 origin = firePoint.position;
        Vector3 direction = -firePoint.forward;
        Vector3 endPoint;
        RaycastHit hit;

        if (Physics.Raycast(origin, direction, out hit, range, hitMask))
        {
            // El rayo golpeó algo
            endPoint = hit.point;

            // Lógica de daño
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyAi enemyHealth = hit.collider.GetComponent<EnemyAi>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damageAmount);
                }
            }
        }
        else
        {
            // El rayo no golpeó nada
            endPoint = origin + direction * range;
        }

        // Mostrar la Línea de Disparo
        if (lineRenderer != null)
        {
            StopAllCoroutines();
            StartCoroutine(RenderLine(origin, endPoint));
        }
    }

    private IEnumerator RenderLine(Vector3 startPos, Vector3 endPos)
    {
        lineRenderer.enabled = true;

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        yield return new WaitForSeconds(lineDisplayTime);

        lineRenderer.enabled = false;
    }
}



