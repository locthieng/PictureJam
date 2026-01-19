using UnityEngine;

public class BlockController : MonoBehaviour
{
    public static BlockController Instance;
    [SerializeField] public Material[] materials;
    [SerializeField] public Color[] blockColors;


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
