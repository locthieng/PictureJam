using System;
using UnityEngine;
public enum BlockType
{
    Normal,
    Horizontal,
    Vertical,
    Ice,
    Glued,
    Nailed
}

[Serializable]
public class BlockData
{
    public BlockType Type;
    public string Name;
    public string Description;
    public int UnlockLevel;
    public Sprite AvatarSprite;
    public bool IsIntroduced;
    public string tutorialText;
}

public class BlockController : MonoBehaviour
{
    public static BlockController Instance;
    [SerializeField] public Color[] blockColors;
    [SerializeField] public BlockData[] blockDatas;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Color GetColorByID(int id)
    {
        if (blockColors.Length <= id)
        {
            return blockColors[0];
        }
        return blockColors[id];
    }
}
