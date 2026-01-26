using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region MOVE HANDLER

public class MoveHandler
{
    public const float Spacing = 0.01f;

    private readonly Rigidbody rb;
    public IMoveChecker MoveChecker { get; }

    public MoveHandler(Rigidbody rb, IMoveChecker moveChecker)
    {
        this.rb = rb;
        MoveChecker = moveChecker;
    }

    public void Move(Vector3 targetPos, float distance, Transform requesterTransform = null)
    {
        AdjustTargetPos(ref targetPos, requesterTransform);
        Vector3 current = rb.position;
        Vector3 desired = targetPos;
        Vector3 dir = desired - current;
        dir.y = 0f;

        Vector3 next = current;

        // X
        if (Mathf.Abs(dir.x) > 0.001f)
        {
            float sign = Mathf.Sign(dir.x);
            float stepX = Mathf.Min(distance, Mathf.Abs(dir.x));
            float moveX = MoveChecker.GetMoveDistance(Vector3.right * sign, stepX);
            next.x += moveX * sign;
            Physics.SyncTransforms();
        }

        // Z
        if (Mathf.Abs(dir.z) > 0.001f)
        {
            float sign = Mathf.Sign(dir.z);
            float stepZ = Mathf.Min(distance, Mathf.Abs(dir.z));
            float moveZ = MoveChecker.GetMoveDistance(Vector3.forward * sign, stepZ);
            next.z += moveZ * sign;
            Physics.SyncTransforms();
        }

        rb.MovePosition(next);
    }

    public void Snap(Vector3 targetPos, Transform requesterTransform = null)
    {
        AdjustTargetPos(ref targetPos, requesterTransform);
        rb.transform.position = targetPos;
    }

    private void AdjustTargetPos(ref Vector3 targetPos, Transform requesterTransform)
    {
        targetPos = requesterTransform != null
            ? rb.transform.position + targetPos - requesterTransform.position
            : targetPos;
    }
}

#endregion

#region MOVE CHECKER INTERFACES

public interface IMoveChecker
{
    float GetMoveDistance(Vector3 direction, float moveStep);
}

#endregion

#region MOVE CHECKER ROOT

public class MoveChecker : IMoveChecker
{
    private readonly List<IMoveChecker> monoCheckers = new();

    public MoveChecker(
        Transform holeTransform,
        LayerMask obstacleMask,
        Collider[] colliders,
        int ID)
    {
        foreach (var col in colliders)
        {
            if (col == null) continue;
            monoCheckers.Add(new MonoMoveChecker(holeTransform, obstacleMask, col, ID));
        }
    }

    public float GetMoveDistance(Vector3 direction, float moveStep)
    {
        return monoCheckers.Min(c => c.GetMoveDistance(direction, moveStep));
    }
}

#endregion

#region MONO MOVE CHECKER (BOXCAST 3D)

public class MonoMoveChecker : IMoveChecker
{
    private readonly Transform _Transform;
    private readonly LayerMask obstacleMask;
    private readonly Collider col;
    private readonly Vector3 halfExtents;
    private readonly int ID;

    private const float SkinWidth = 0.05f;

    public MonoMoveChecker(
        Transform holeTransform,
        LayerMask obstacleMask,
        Collider col,
        int ID)
    {
        this._Transform = holeTransform;
        this.obstacleMask = obstacleMask;
        this.col = col;
        this.ID = ID;

        Vector3 size = col.bounds.size;
        halfExtents = new Vector3(
            Mathf.Max(0.01f, size.x - SkinWidth) * 0.5f,
            Mathf.Max(0.01f, size.y - SkinWidth) * 0.5f,
            Mathf.Max(0.01f, size.z - SkinWidth) * 0.5f
        );
    }

    public float GetMoveDistance(Vector3 direction, float moveStep)
    {
        RaycastHit[] hits = Physics.BoxCastAll(
            col.bounds.center,
            halfExtents,
            direction,
            Quaternion.identity,
            moveStep,
            obstacleMask
        );

        if (hits.Length == 0)
            return Mathf.Max(0, moveStep - MoveHandler.Spacing);

        float minDist = Mathf.Max(0, moveStep - MoveHandler.Spacing);

        foreach (var h in hits)
        {
            Collider hitCol = h.collider;
            if (hitCol == col) continue;

            if (hitCol.transform.parent != null)
            {
                Transform parent = hitCol.transform.parent;

                if (parent == _Transform) continue;

                if (parent.TryGetComponent<IMoveable>(out var hitMoveable)
                    && _Transform.TryGetComponent<IMoveable>(out var selfMoveable)
                    && IMoveable.HasCommonAncestor(hitMoveable, selfMoveable))
                    continue;
            }

            float dist = Mathf.Max(0, h.distance - MoveHandler.Spacing);

            if (dist < minDist)
                minDist = dist;
        }

        return minDist;
    }

}



#endregion

#region MOVEABLE INTERFACE + UTILS

public interface IMoveable
{
    IMoveable Parent { get; set; }
    MoveHandler MoveHandler { get; }

    void StartMove() { }
    void EndMove() { }

    public static IMoveable GetLowestCommonAncestor(IMoveable a, IMoveable b)
    {
        var ancestorsA = new HashSet<IMoveable>();
        var cur = a;

        while (cur != null)
        {
            ancestorsA.Add(cur);
            cur = cur.Parent;
        }

        var lca = b;
        while (lca != null)
        {
            if (ancestorsA.Contains(lca))
                return lca;

            lca = lca.Parent;
        }

        return null;
    }

    public static bool HasCommonAncestor(IMoveable a, IMoveable b)
    {
        return GetLowestCommonAncestor(a, b) != null;
    }
}

#endregion

#region PARENT MOVE CHECKER

public class ParentMoveChecker : IMoveChecker
{
    private readonly IMoveable[] moveableChildren;

    public ParentMoveChecker(IMoveable[] moveableChildren)
    {
        this.moveableChildren = moveableChildren;
    }

    public float GetMoveDistance(Vector3 direction, float moveStep)
    {
        return moveableChildren.Min(
            c => c.MoveHandler.MoveChecker.GetMoveDistance(direction, moveStep)
        );
    }
}

#endregion