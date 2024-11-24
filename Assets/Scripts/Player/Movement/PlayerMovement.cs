using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInputManager))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerScript))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    private PlayerInputManager playerInputManager;
    private Vector2 movementAxis = Vector2.zero;
    private CharacterController characterController;
    private Camera mainCamera;
    private PlayerScript playerScript;
    private Animator animator;
    private bool rotationLocked = false;
    private int levelCounter = 0;
    [SerializeField]
    private int levelsNeededForUlt = 10;
    public float LeftToUltRatio
    {
        get
        {
            float left = levelsNeededForUlt - levelCounter;
            left = Mathf.Max(0f, left);
            return left / levelsNeededForUlt;
        }
    }
    public Animator Animator => animator;
    public event Action OnUlt;
    public void SetRotationLocked(bool locked) => rotationLocked = locked;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        playerScript = GetComponent<PlayerScript>();
        playerScript.OnLevelUp += AddLevelToCounter;

        playerInputManager = GetComponent<PlayerInputManager>();
        playerInputManager.OnMovementEvent += SetMovement;
        playerInputManager.OnLookEvent += HandleRotation;
        playerInputManager.OnUltEvent += UseUlt;
    }

    private void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (movementAxis == Vector2.zero)
        {
            animator.SetBool("moving", false);
            return;
        }

        var move = new Vector3(movementAxis.x, 0, movementAxis.y);

        move *= playerScript.Speed * Time.deltaTime;
        animator.SetBool("moving", true);
        characterController.Move(move);
    }

    private void HandleRotation(Vector2 mousePosition)
    {
        if (rotationLocked || Time.timeScale == 0) return;
        Vector3 mousePos = new Vector3(mousePosition.x, mousePosition.y, 0);

        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 hitPoint = ray.GetPoint(rayDistance);
            Vector3 lookDir = hitPoint - transform.position;
            lookDir.y = 0;

            if (lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDir);
            }
        }
    }

    private void AddLevelToCounter()
    {
        levelCounter++;
    }

    private void UseUlt()
    {
        if(levelCounter >= levelsNeededForUlt)
        {
            levelCounter = 0;
        } else
        {
            return;
        }

        Invoke("AttackUlt", 1f);
        animator.SetTrigger("ult");
        OnUlt?.Invoke();
    }

    private void AttackUlt()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 20f);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out IDamagable enemy))
            {
                enemy.Damage(50);
            }
        }
    }

    private void SetMovement(Vector2 axis)
    {
        movementAxis = axis;
    }

    private void OnDestroy()
    {
        playerInputManager.OnLookEvent -= HandleRotation;
        playerInputManager.OnMovementEvent -= SetMovement;
        playerInputManager.OnUltEvent -= UseUlt;
        playerScript.OnLevelUp -= AddLevelToCounter;

    }
}
