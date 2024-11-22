using UnityEngine;

[RequireComponent(typeof(PlayerInputManager))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent (typeof(PlayerScript))]
public class PlayerMovement : MonoBehaviour
{
    private PlayerInputManager playerInputManager;
    private Vector2 movementAxis = Vector2.zero;
    private CharacterController characterController;
    private Camera mainCamera;
    private PlayerScript playerScript;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        playerScript = GetComponent<PlayerScript>();

        playerInputManager = GetComponent<PlayerInputManager>();
        playerInputManager.OnMovementEvent += SetMovement;
        playerInputManager.OnLookEvent += HandleRotation;
    }

    private void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (movementAxis == Vector2.zero) return;
        var move = new Vector3(movementAxis.x, 0, movementAxis.y);
        move *= playerScript.Speed * Time.deltaTime;
        characterController.Move(move);
    }

    private void HandleRotation(Vector2 mousePosition)
    {
        Vector3 mousePos = new Vector3(mousePosition.x, mousePosition.y, 0);

        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if(groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 hitPoint = ray.GetPoint(rayDistance);
            Vector3 lookDir = hitPoint - transform.position;
            lookDir.y = 0;

            if(lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDir);
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
    }
}
