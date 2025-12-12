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

        Vector3 camForward = playerCamera.forward;
        Vector3 camRight = playerCamera.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camRight * input.x + camForward * input.y;

        // 3. Determinar Velocidad (Usando la variable local walkSpeed)
        float currentSpeed = walkSpeed; // 👈 Usamos la variable local

        // (Opcional: Lógica de sprint usando la variable local sprintSpeed)
        // if (sprintAction != null && sprintAction.ReadValue<float>() > 0f && canSprint)
        //     currentSpeed = sprintSpeed;


        // 4. Aplicar Movimiento (en XZ) y Gravedad (en Y)
        move = move.normalized * currentSpeed;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

        // 5. Rotación (Usando la variable local rotationSpeed)
        if (move.sqrMagnitude > 0.01f && controller.isGrounded)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(move.x, 0, move.z));
            // 👈 Usamos la variable local rotationSpeed
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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