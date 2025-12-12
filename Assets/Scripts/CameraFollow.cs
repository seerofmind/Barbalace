using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target & Speed")]
    public Transform target; // El jugador que queremos seguir (arrastrar aquí el Transform del jugador)
    [Range(0.01f, 1.0f)]
    public float smoothSpeed = 0.125f; // Cuanto menor, más suave y lento será el seguimiento (damping)

    [Header("Offset")]
    public Vector3 offset = new Vector3(0f, 2f, -10f); // Distancia deseada desde el jugador (ej. 10 unidades detrás, 2 unidades arriba)

    [Header("Camera Bounds (Opcional)")]
    public bool useBounds = false; // ¿Limitamos la cámara al nivel?
    public Vector2 minBounds; // El punto más bajo/izquierda que la cámara puede ver (ej. -20, -10)
    public Vector2 maxBounds; // El punto más alto/derecha que la cámara puede ver (ej. 50, 20)

    // Usamos LateUpdate para asegurar que el jugador ha terminado de moverse en Update
    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogError("El objetivo (Target) de la cámara no está asignado.");
            return;
        }

        // 1. Calcular la posición deseada de la cámara
        Vector3 desiredPosition = target.position + offset;

        // 2. Aplicar el suavizado (Lerp)
        // Lerp mueve gradualmente de la posición actual a la posición deseada
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 3. Aplicar límites (Opcional)
        if (useBounds)
        {
            // Limitar la posición X
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minBounds.x, maxBounds.x);

            // Limitar la posición Y
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minBounds.y, maxBounds.y);

            // Nota: El eje Z generalmente no se limita en 2D, pero se mantiene fijo.
        }

        // 4. Mover la cámara a la posición suavizada/limitada
        transform.position = smoothedPosition;

        // Opcional: Asegurar que la cámara siempre mira al jugador (si offset.z es 0)
        // transform.LookAt(target);
    }
}
