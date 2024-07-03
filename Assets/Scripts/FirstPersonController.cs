using System.Collections;
using System.Collections.Generic;
using Mirror.Examples.Basic;
using Mirror.Examples.Benchmark;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class FirstPersonController : NetworkBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float sprintSpeed = 10f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] float crouchSpeed = 2.5f;
    [SerializeField] float crouchHeight = 1f;
    [SerializeField] float standHeight = 2f;

    public Transform groundCheck;
    public LayerMask groundLayer;
    private Vector2 moveInput;
    private bool isSprinting = false;
    private bool isCrouching = false;
    bool isGrounded;
    bool jumpInput;

    [Header("MouseLook Parameters")]
    [SerializeField] float mouseSensitivity = 100f;
    Transform playerCameraTransform;
    private Vector2 lookInput;
    float xRotation;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private PlayerInputActions playerInputActions;
    private AudioListener audioListener;
    private Camera playerCamera;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        playerCameraTransform = GetComponentInChildren<Camera>().transform;
        audioListener = GetComponentInChildren<AudioListener>();
        playerCamera = GetComponentInChildren<Camera>();
        playerInputActions = new PlayerInputActions();
    }

    void Start()
    {
        InitializePlayer();
    }

    [Client]
    private void InitializePlayer()
    {
        // Check if the current instance is the local player
        if (isLocalPlayer)
        {
            // Enable camera and audio listener for local player
            playerCamera.enabled = true;
            audioListener.enabled = true;

            // Additional setup for the local player (e.g., input handlers, UI)
            //SetupLocalPlayer();
        }
        else
        {
            // Disable camera and audio listener for remote players
            playerCamera.enabled = false;
            audioListener.enabled = false;

            // Configure remote player specifics if necessary
            //SetupRemotePlayer();
        }
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerInputActions.Player.Enable();

        // Movement Input
        playerInputActions.Player.Movement.performed += HandleMovement;
        playerInputActions.Player.Movement.canceled += HandleMovement;

        // Sprint input
        playerInputActions.Player.Sprint.performed += ctx => isSprinting = true;
        playerInputActions.Player.Sprint.canceled += ctx => isSprinting = false;

        // Crouch input
        playerInputActions.Player.Crouch.performed += ctx => StartCrouch();
        playerInputActions.Player.Crouch.canceled += ctx => StopCrouch();

        playerInputActions.Player.Jump.performed += HandleJump;
        playerInputActions.Player.MouseLook.performed += HandleMouseLook;
        playerInputActions.Player.MouseLook.canceled += HandleMouseLook;
    }

    private void OnDisable()
    {
        if (isLocalPlayer)
        {
            //playerInputActions.Player.Movement.performed -= HandleMovement;
            //playerInputActions.Player.Movement.canceled -= HandleMovement;
            //playerInputActions.Player.Jump.performed -= HandleJump;
            //playerInputActions.Player.MouseLook.performed -= HandleMouseLook;
            //playerInputActions.Player.MouseLook.canceled -= HandleMouseLook;
            playerInputActions.Player.Disable();
        }
    }

    private void HandleMovement(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        moveInput = context.ReadValue<Vector2>();
    }

    private void HandleJump(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        if (isGrounded)
        {
            jumpInput = true;
        }
    }

    private void HandleMouseLook(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        lookInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        // Calculate movement direction based on camera orientation
        Vector3 forward = playerCameraTransform.forward;
        Vector3 right = playerCameraTransform.right;

        // Keep movement in the horizontal plane
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;
        float trueSpeed = walkSpeed;

        if (isSprinting) // Is the player sprinting
        {
            trueSpeed = sprintSpeed;
        }
        else if (isCrouching) // Is the player crouching
        {
            trueSpeed = crouchSpeed;
        }

        rb.MovePosition(rb.position + moveDirection * trueSpeed * Time.fixedDeltaTime);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (jumpInput && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpInput = false;
        }
    }

    private void StartCrouch()
    {
        isCrouching = true;
        capsuleCollider.height = crouchHeight;
    }

    private void StopCrouch()
    {
        isCrouching = false;
        capsuleCollider.height = standHeight;
    }

    private void Update()
    {
        // Mouse look
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}