using TMPro.Examples;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MoveBlock : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask moveBlockMask;
    [SerializeField] public BlockType Type;

    private Rigidbody rb;
    private Camera cam;
    private Vector3 targetPosition;
    private float yHeight;
    private bool isDragging = false;
    private Bounds combinedBounds;
    private Vector3 dragOffset;
    public Collider[] colliders;
    BoardController board => BoardController.Instance;

    [SerializeField] public int numIceBlock;

    public BlockParent Parent { get; set; }
    public int SizeX { get; private set; }
    public int SizeZ { get; private set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        //cam = Camera.main;
        cam = CameraController.Instance.GameCamera;
        yHeight = transform.position.y;
        colliders = GetComponentsInChildren<Collider>();
        combinedBounds = GetCombinedBounds();
        CalculateSizes();
    }

    private void CalculateSizes()
    {
        float tileSize = BoardController.Instance.tileSize;
        Vector3 size = combinedBounds.size;

        SizeX = Mathf.RoundToInt(size.x / tileSize);
        SizeZ = Mathf.RoundToInt(size.z / tileSize);
    }


    void OnMouseDown()
    {
        if (Parent != null)
        {
            Parent.OnChildMouseDown(); // Gọi sang cha
            return;
        }
        GameUIController.Instance?.StartClock();

        isDragging = true;
        SetCollidersLayer(LayerMaskToLayer(moveBlockMask));

        Plane plane = new Plane(Vector3.up, Vector3.up * yHeight);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            // Offset = Vị trí vật thể - Điểm chuột chạm vào
            dragOffset = transform.position - hitPoint;
        }
    }

    void OnMouseUp()
    {
        if (Parent != null)
        {
            Parent.OnChildMouseUp(); // Gọi sang cha
            return;
        }
        isDragging = false;

        // Snap block vào lưới
        if (BoardController.Instance != null)
        {
            Vector3 snappedPos = BoardController.Instance.GetSnappedPosition(transform.position);
            snappedPos.y = yHeight;
            rb.MovePosition(snappedPos);
        }

        SetCollidersLayer(LayerMaskToLayer(obstacleMask));
        BoardController.Instance?.ClearShadows();
    }


    private void SetCollidersLayer(int layer)
    {
        foreach (var col in colliders)
        {
            col.gameObject.layer = layer;
        }
    }

    private int LayerMaskToLayer(LayerMask mask)
    {
        int layer = mask.value;
        for (int i = 0; i < 32; i++)
        {
            if ((layer & (1 << i)) != 0)
                return i;
        }
        return 0;
    }



    void OnMouseDrag()
    {
        if (Parent != null)
        {
            Parent.OnChildMouseDrag(); // Gọi sang cha
            return;
        }

        Plane plane = new Plane(Vector3.up, Vector3.up * yHeight);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        //Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
        if (plane.Raycast(ray, out float distance))
        {
            targetPosition = ray.GetPoint(distance);
            targetPosition += dragOffset;
            targetPosition.y = yHeight;
        }
    }

    private Bounds GetCombinedBounds()
    {
        Bounds bounds = colliders[0].bounds;
        for (int i = 1; i < colliders.Length; i++)
        {
            bounds.Encapsulate(colliders[i].bounds);
        }
        return bounds;
    }


    private void FixedUpdate()
    {
        if (!isDragging) return;

        Vector3 current = transform.position;
        Vector3 desired = targetPosition;
        Vector3 direction = desired - current;
        direction.y = 0f;

        float moveStep = moveSpeed * Time.fixedDeltaTime;

        Vector3 nextPosition = current;

        // Di chuyển X
        float moveX = ComputeAxisMovement(Vector3.right * Mathf.Sign(direction.x), Mathf.Abs(direction.x), moveStep);
        nextPosition.x += moveX;

        // Di chuyển Z
        float moveZ = ComputeAxisMovement(Vector3.forward * Mathf.Sign(direction.z), Mathf.Abs(direction.z), moveStep);
        nextPosition.z += moveZ;

        
        // ✅ Kiểm tra tổng thể sau khi tính cả X & Z
        Vector3 moveDelta = nextPosition - current;
        if (moveDelta != Vector3.zero)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                Vector3 center = colliders[i].bounds.center;
                Vector3 halfExtents = colliders[i].bounds.extents * 0.95f;

                if (Physics.BoxCast(center, halfExtents, moveDelta.normalized, out RaycastHit hit, Quaternion.identity, moveDelta.magnitude, obstacleMask))
                {
                    float safeDistance = Mathf.Max(0f, hit.distance - 0.01f);
                    nextPosition = current + moveDelta.normalized * safeDistance;
                    break;
                }
            }
        }
        if (isDragging && board != null)
        {
            Vector3[] childPositions = new Vector3[colliders.Length];
            for (int i = 0; i < colliders.Length; i++)
            {
                Vector3 localOffset = colliders[i].transform.position - transform.position;
                childPositions[i] = nextPosition + localOffset;
            }

            board.ShowShadows(childPositions);
        }
        else if (board != null)
        {
            board.ClearShadows();
        }


        nextPosition.y = yHeight;
        rb.MovePosition(nextPosition);

    }

    private float ComputeAxisMovement(Vector3 dir, float totalDistance, float step)
    {
        float move = Mathf.Min(totalDistance, step);
        for (int i = 0; i < colliders.Length; i++)
        {
            Vector3 center = colliders[i].bounds.center;
            Vector3 halfExtents = colliders[i].bounds.extents * 0.95f;

            if (Physics.BoxCast(center, halfExtents, dir, out RaycastHit hit, Quaternion.identity, move, obstacleMask))
            {
                move = Mathf.Min(move, Mathf.Max(0f, hit.distance - 0.01f));
            }
        }

        return dir.normalized.x * move + dir.normalized.z * move;
    }

    public void BlockCanMove(bool isActive)
    {
        // false : cannot move, true : can move
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].gameObject.SetActive(isActive);
        }
    }    

    public void UnlockIceBlock()
    {
        if (numIceBlock == 0)
        {
            //move normal
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(combinedBounds.center, combinedBounds.size);
    }

}
