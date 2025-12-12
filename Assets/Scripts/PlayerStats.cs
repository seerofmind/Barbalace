using UnityEngine.InputSystem;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    /// --- Referencias ---
    [Header("References")]
    public Transform playerCamera;
    public Pistol playerPistol; // Script del arma para disparar

    // --- VARIABLES DE CONFIGURACIÓN (Reemplazando PlayerData) ---
    [Header("Player Stats")]
    public float walkSpeed = 5.0f;
    public float sprintSpeed = 8.0f;
    public float rotationSpeed = 10.0f;
    public float jumpHeight = 1.5f; // Altura máxima que el jugador puede saltar

    // --- Inputs ---
    [Header("Input Setup")]
    public PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction shootAction;
    private InputAction sprintAction;

    // --- Componentes ---
    private CharacterController controller;

    // --- Variables de Movimiento ---
    [Header("Runtime Movement")]
    private float verticalVelocity;
    private float gravity = -9.81f;
    private bool canSprint = true;

    // --- Variables de Disparo ---
    private bool isDead = false;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (!playerInput)
            playerInput = FindFirstObjectByType<PlayerInput>();

        // 1. Obtener Acciones del Player Input
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        shootAction = playerInput.actions["Fire"];
        // sprintAction = playerInput.actions["Sprint"]; // Descomentar si usas sprint

        // 2. Suscribir Eventos (Disparar y Saltar)
        shootAction.performed += ctx => TryShoot();
        jumpAction.performed += ctx => TryJump();

        // Bloquear el cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // **NOTA:** La línea de error sobre 'playerData' ha sido eliminada.
    }

    void OnDestroy()
    {
        // Limpiar suscripciones para evitar errores
        if (shootAction != null)
            shootAction.performed -= ctx => TryShoot();
        if (jumpAction != null)
            jumpAction.performed -= ctx => TryJump();
    }

    void Update()
    {
        if (controller == null || !controller.enabled)
            return;

        HandleGravity();
        HandleMovement();
    }


    // ------------------------------- Movimiento y Gravedad -------------------------------

    private void HandleGravity()
    {
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;
        }
        else
        {
            // Aplicar gravedad
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        // Usamos las direcciones de la cámara para que la entrada sea relativa al ángulo de la cámara.
        Vector3 camForward = playerCamera.forward;
        Vector3 camRight = playerCamera.right;

        // Aplanar las direcciones al plano XZ (importante para evitar vuelo al mirar hacia arriba/abajo)
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // Calcular el vector de movimiento en el plano XZ
        Vector3 move = camRight * input.x + camForward * input.y;

        // 3. Determinar Velocidad
        float currentSpeed = walkSpeed;

        // 4. Aplicar Movimiento (en XZ) y Gravedad (en Y)
        move = move.normalized * currentSpeed;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

        // ----------------------------------------------------
        // 5. ROTACIÓN MODIFICADA: MIRAR SOLO EN EL EJE Y (Izquierda/Derecha)
        // ----------------------------------------------------

        // Solo rotamos si hay movimiento horizontal significativo
        if (Mathf.Abs(input.x) > 0.01f)
        {
            // Determinamos la dirección horizontal global que el jugador debe enfrentar.
            // Si input.x > 0, mira hacia adelante de la cámara (derecha).
            // Si input.x < 0, mira hacia la izquierda.

            // Creamos la rotación deseada
            Quaternion targetRotation;

            if (input.x > 0)
            {
                // Mira hacia la derecha (dirección forward de la cámara, aplanada)
                targetRotation = Quaternion.LookRotation(camForward);
            }
            else
            {
                // Mira hacia la izquierda (dirección opuesta)
                targetRotation = Quaternion.LookRotation(-camForward);
            }

            // Aplicamos la rotación suavemente, forzando la rotación solo en el eje Y
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.Euler(0, targetRotation.eulerAngles.y, 0), // Ignoramos pitch (X) y roll (Z)
                rotationSpeed * Time.deltaTime
            );
        }
    }


    // ------------------------------- Salto -------------------------------

    private void TryJump()
    {
        if (controller.isGrounded)
        {
            // La fórmula de salto basada en la gravedad (verticalVelocity = sqrt(height * -2 * gravity))
            // 👈 Usamos la variable local jumpHeight
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    // ------------------------------- Disparo -------------------------------

    private void TryShoot()
    {
        if (isDead) return;

        if (playerPistol != null)
        {
            playerPistol.Shoot();
        }
        else
        {
            Debug.LogError("Player Pistol no asignada.");
        }
    }
}