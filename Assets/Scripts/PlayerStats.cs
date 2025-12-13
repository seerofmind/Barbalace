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
    public int maxHealth = 10;
    private int currentHealth;
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
        currentHealth = maxHealth;
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
        if (isDead)
            return;

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

        // --- Movimiento (Usa la Cámara para la Dirección de Desplazamiento) ---
        // Mantenemos la lógica de movimiento relativa a la cámara para que WASD/Stick funcionen bien
        // incluso si la cámara tiene un ligero ángulo.
        Vector3 camForward = playerCamera.forward;
        Vector3 camRight = playerCamera.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camRight * input.x + camForward * input.y;

        // 3. Determinar Velocidad
        float currentSpeed = walkSpeed;

        // 4. Aplicar Movimiento (en XZ) y Gravedad (en Y)
        move = move.normalized * currentSpeed;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

        // ----------------------------------------------------
        // 5. ROTACIÓN CORREGIDA: SOLO MIRAR IZQUIERDA O DERECHA
        // ----------------------------------------------------

        // Solo rotamos si hay movimiento horizontal significativo (input.x)
        if (Mathf.Abs(input.x) > 0.01f)
        {
            Quaternion targetRotation;

            // 1. Determinar la dirección de rotación
            if (input.x > 0)
            {
                // El jugador se mueve a la derecha. La rotación es 0 grados (o la dirección positiva en Y).
                // Si quieres que la derecha sea 0 grados (forward), usa el vector forward del mundo.
                targetRotation = Quaternion.identity; // Rotación por defecto (0, 0, 0)
            }
            else
            {
                // El jugador se mueve a la izquierda. La rotación es 180 grados.
                targetRotation = Quaternion.Euler(0, 180f, 0);
            }

            // 2. Aplicar la rotación suavemente
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
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

    public void TakeDamage(int damageAmount)
    {
        if (isDead)
        {
            return; // Ignorar daño si ya está muerto
        }

        currentHealth -= damageAmount;

        Debug.Log($"Jugador recibió {damageAmount} de daño. Vida restante: {currentHealth}");

        // Opcional: Implementar efectos visuales (flash rojo, sonido de impacto)
        // StartCoroutine(VisualDamageFeedback()); 

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        // Deshabilitar la entrada y el movimiento
        controller.enabled = false;

        // Si tienes un Renderer o un Collider que quieres apagar, hazlo aquí.
        // GetComponent<Renderer>().enabled = false; 

        Debug.Log("¡Jugador ha muerto!");
        
        if (controller != null)
        {
            controller.enabled = false;
        }

        // Opcional: Deshabilitar el script PlayerController si es necesario
        // enabled = false; 

        // Opcional: Deshabilitar la entrada si usas PlayerInput component
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }
    }
}