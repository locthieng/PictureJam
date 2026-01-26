using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BlockParent : MonoBehaviour, IMoveable
{
    public MoveHandler MoveHandler { get; private set; }
    public IMoveable Parent { get; set; }
    private IMoveable[] children;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        children = GetMoveableChildren();
        MoveHandler = new MoveHandler(rb, new ParentMoveChecker(children));
    }

    private IMoveable[] GetMoveableChildren()
    {
        var children = GetComponentsInChildren<IMoveable>();
        var result = new List<IMoveable>();
        foreach (var child in children)
        {
            if (child == this) continue;
            child.Parent = this;
            result.Add(child);
        }

        return result.ToArray();
    }

    public void StartMove()
    {
        foreach (var child in children)
        {
            child.StartMove();
        }
    }   
    
    public void EndMove()
    {
        foreach (var child in children)
        {
            child.EndMove();
        }
    }    
}