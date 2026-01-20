using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] public int targetPicture;

    public virtual void SetUp()
    {
    }

    public virtual void StartLevel()
    {
    }

    public virtual void ResetLevel()
    {
    }

    public void CheckWin(bool isActive)
    {
        targetPicture--;

        if (targetPicture == 0)
        {
            Debug.Log("win");
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
