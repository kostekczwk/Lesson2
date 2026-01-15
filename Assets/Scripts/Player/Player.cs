using System;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public event EventHandler OnPickSomething;

    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter baseCounter;
    }

    /// <summary>
    /// SerializeField atribute expose field to Unity editor
    /// </summary>
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;


    private bool isWalking = false;
    private Vector3 lastInteractDir;
    private BaseCounter selectedBaseCounter;
    private KitchenObject kitchenObject;

    private void Awake()
    {
        if (Instance != null) 
        {
            Debug.LogError("More than one player instance");
        }

        Instance = this;
    }

    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying())
        {
            return;
        }

        if (selectedBaseCounter != null) 
        {
            selectedBaseCounter.Interact(this);
        }
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying())
        {
            return;
        }

        if (selectedBaseCounter != null)
        {
            selectedBaseCounter.InteractAlternate(this);
        }
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 inputVectorNormalized = gameInput.GetMovmentVectorNormalized();
        Vector3 moveDir = new Vector3(inputVectorNormalized.x, 0f, inputVectorNormalized.y);

        HandleMovement(moveDir);
        HandleInteractions(moveDir);
    }

    private void HandleMovement(Vector3 moveDir)
    {
        this.isWalking = moveDir != Vector3.zero;

        //this.transform.rotation
        //this.transform.eulerAngles
        //this.transform.LookAt
        //Vector3.Lerp - positions
        //Vector3.Slerp - rotations
        float rotateSpeed = 10f;
        this.transform.forward = Vector3.Slerp(this.transform.forward, moveDir, rotateSpeed * Time.deltaTime);

        bool canMove = CanMove(ref moveDir);
        if (canMove)
        {
            this.transform.position += moveDir * moveSpeed * Time.deltaTime;
        }
    }

    private bool CanMove(ref Vector3 moveDir)
    {
        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        float playerHeight = 2f;

        // Cast from player center -> issues while colisions on edges (player is sphere shape)
        //bool canMove = !Physics.Raycast(this.transform.position, moveDir, playerSize); 

        bool canMove = !Physics.CapsuleCast(
            this.transform.position,
            this.transform.position + Vector3.up * playerHeight,
            playerRadius,
            moveDir,
            moveDistance);

        if (!canMove)
        {
            // Cannot move towards modeDir
            // Attempt only X movement

            Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f).normalized;

            canMove = !Physics.CapsuleCast(
                this.transform.position,
                this.transform.position + Vector3.up * playerHeight,
                playerRadius,
                moveDirX,
                moveDistance);

            if (canMove)
            {
                // Can move only X
                moveDir = moveDirX;
            }
            else
            {
                // Cannot move only on X
                // Attempt only Z movement

                Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z).normalized;

                canMove = !Physics.CapsuleCast(
                    this.transform.position,
                    this.transform.position + Vector3.up * playerHeight,
                    playerRadius,
                    moveDirZ,
                    moveDistance);

                if (canMove)
                {
                    // Can move only Z
                    moveDir = moveDirZ;
                }
                else
                {
                    // Cannot move any direction
                }
            }
        }

        return canMove;
    }

    private void HandleInteractions(Vector3 moveDir)
    {
        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;

        if (Physics.Raycast(this.transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                SetSelectedCounter(baseCounter);
                return;
            }
        }

        SetSelectedCounter(null);
    }

    private void SetSelectedCounter(BaseCounter baseCounter)
    {
        if (this.selectedBaseCounter != baseCounter)
        {
            this.selectedBaseCounter = baseCounter;
            OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs { baseCounter = this.selectedBaseCounter });
        }
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    public Transform GetKitchenObjectDestinationTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null)
        {
            OnPickSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject() { return kitchenObject; }

    public void ClearKitchenObject()
    {
        this.kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return this.kitchenObject != null;
    }
}
