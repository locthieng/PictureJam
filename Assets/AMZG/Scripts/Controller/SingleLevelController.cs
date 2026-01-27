using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.AI;

public struct StageTransform
{
    public string Name;
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;
}

public class SingleLevelController : MonoBehaviour
{
    public GridDataAsset DataAsset;
    [SerializeField] public List<MoveBlock> moveBlocks = new List<MoveBlock>();
    [SerializeField] public List<MoveObstacle> moveObstacles = new List<MoveObstacle>();
    [SerializeField] public List<TargetPicture> pictures = new List<TargetPicture>();

    [SerializeField] public int targetPicture;
    public Transform boardTransform;
    public bool IsHard;
    public virtual void SetUp()
    {
        if (IsHard)
        {
            GameUIController.Instance.ShowLevelHardWarning();
        }
        moveBlocks = BoardController.Instance.MoveBlocks;
        moveObstacles = BoardController.Instance.MoveObstacles;

    }

    public virtual void StartLevel()
    {
    }

    public virtual void ResetLevel()
    {
    }

    public void BlockCanMove(bool isActive)
    {
        for (int i = 0; i < moveBlocks.Count; i++)
        {
            moveBlocks[i].BlockCanMove(isActive);
        }
    }

    public void UnlockIceBlock()
    {
        for (int i = 0; i < moveBlocks.Count; i++)
        {
            if (moveBlocks[i].Type == BlockType.Ice) moveBlocks[i].UnlockIceBlock();
        }    
    }    

    public void CompletedPicture(int id)
    {
        for (int i = 0; i < pictures.Count;i++)
        {
            if (pictures[i].IDPic == id)
            {
                pictures[i].gameObject.SetActive(false);
                break;
            }    
        }    
    }    

    public void CheckWin(bool isActive)
    {
        targetPicture--;

        if (targetPicture == 0)
        {
            Debug.Log("win");
            GameUIController.Instance.SetClock(false);
            StageController.Instance.End(isActive);
        } 
            
    }    

    private void OnDestroy()
    {
    }

    public virtual void ShowSingleFinishEffects()
    {

    }

    public virtual void RefreshCharacterSkins()
    {
    }

    public virtual void SetCharacterSkin(ShopItemData data)
    {
    }
}
