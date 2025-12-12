using UnityEngine.InputSystem;
using UnityEngine;
using System.Diagnostics;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // --- Referencias ---
    [Header("References")]
    public PlayerData playerData;        // ScriptableObject para las stats
    public Transform playerCamera;
    public Pistol playerPistol;          // Script del arma para disparar

    // --- Inputs ---
    [Header("Input Setup")]
    public PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;     // 👈 Nuevo: Acción de Salto
    private InputAction shootAction;    // Acción de Disparo
    private InputAction sprintAction;   // Acción de Correr (opcional)

    // --- Componentes ---
    private CharacterController controller;

    // --- Variables de Movimiento ---
    [Header("Runtime Movement")]
    private float verticalVelocity;
    private float gravity = -9.81f;
    private bool canSprint = true;

    // --- Variables de Disparo ---
    private bool isDead = false; // Solo para guardar la lógica de disparo

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (!playerInput)
            playerInput = FindFirstObjectByType<PlayerInput>();

        // 1. Obtener Acciones del Player Input
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];   // Asume que tienes una acción "Jump"
        shootAction = playerInput.actions["Fire"];  // Asume que tienes una acción "Fire"
        // sprintAction = playerInput.actions["Sprint"]; // Descomentar si usas sprint

        // 2. Suscribir Eventos (Disparar y Saltar)
        shootAction.performed += ctx => TryShoot();
        jumpAction.performed += ctx => TryJump();

        // Bloquear el cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Inicializar datos (usando valores de PlayerData)
        if (playerData == null)
            Debug.LogError("Player Data ScriptableObject no asignado.");
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
        // Si el controlador no existe o está deshabilitado, no hagas nada
        if (controller == null || !controller.enabled)
            return;

        HandleGravity();
        HandleMovement();
    }


    // ------------------------------- Movimiento y Gravedad -------------------------------

    private void HandleGravity()
    {
        // Si estamos tocando el suelo, reiniciamos la velocidad vertical
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -2f; // Pequeña fuerza hacia abajo para asegurar que toca el suelo
        }
        else
        {
            // Aplicar gravedad
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        // 1. Leer Input de Movimiento (WASD o Stick)
        Vector2 input = moveAction.ReadValue<Vector2>();

        // 2. Obtener dirección relativa a la Cámara (para FPP)
        Vector3 camForward = playerCamera.forward;
        Vector3 camRight = playerCamera.right;

        // Anular el componente Y para que el movimiento sea solo en el plano XZ
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // Calcular el vector de movimiento deseado
        Vector3 move = camRight * input.x + camForward * input.y;

        // 3. Determinar Velocidad
        float currentSpeed = playerData != null ? playerData.walkSpeed : 5.0f; // Velocidad base

        // (Opcional: Añadir lógica para sprint si descomentas sprintAction)
        // if (sprintAction.ReadValue<float>() > 0f && canSprint)
        //     currentSpeed = playerData.sprintSpeed;


        // 4. Aplicar Movimiento (en XZ) y Gravedad (en Y)
        move = move.normalized * currentSpeed;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

        // 5. Rotación (Opcional: Si quieres que el personaje rote en el eje Y al moverse)
        if (move.sqrMagnitude > 0.01f && controller.isGrounded)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(move.x, 0, move.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, playerData.rotationSpeed * Time.deltaTime);
        }
    }


    // ------------------------------- Salto -------------------------------

    private void TryJump()
    {
        if (controller.isGrounded)
        {
            // La fórmula de salto basada en la gravedad (verticalVelocity = sqrt(height * -2 * gravity))
            float jumpHeight = playerData != null ? playerData.jumpHeight : 1.5f; // Asegúrate de añadir jumpHeight a PlayerData

            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            // Debug.Log("Jump!");
        }
    }

    // ------------------------------- Disparo -------------------------------

    private void TryShoot()
    {
        // Solo permite disparar si no está muerto (puedes añadir otras condiciones)
        if (isDead) return;

        // Llama a la función de disparo de tu pistola (ver script Pistol.cs)
        if (playerPistol != null)
        {
            playerPistol.Shoot();
        }
        else
        {
            Debug.LogError("Player Pistol no asignada.");
        }
    }

    // --- Métodos de Ayuda (puedes moverlos aquí si quieres simplificar PlayerData) ---
    // public void Die() { /* ... */ }
    // public void TakeDamage(int amount) { /* ... */ }
}
