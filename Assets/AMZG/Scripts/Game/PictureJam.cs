using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PictureJam : MonoBehaviour
{
    [SerializeField] private bool isParent;
    [SerializeField] private Transform[] truePoints;
    [SerializeField] private MoveBlock[] childBlocks;
    private LevelController levelController;

    public bool isCheck = false;
    // Start is called before the first frame update
    void Start()
    {
        levelController = LevelController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("check" + CheckPoint());
        CheckPoint();
    }

    private bool CheckPoint()
    {
        if (!isParent) return false;

        if (childBlocks == null || truePoints == null)
            return false;

        if (childBlocks.Length != truePoints.Length)
            return false;

        if (isCheck) return false;
        const float offset = 0.1f;

        for (int i = 0; i < childBlocks.Length; i++)
        {
            Vector3 blockPos = childBlocks[i].transform.position;
            Vector3 truePos = truePoints[i].position;

            blockPos.y = 0f;
            truePos.y = 0f;

            if (Vector3.Distance(blockPos, truePos) > offset)
            {
                return false;
            }
        }
        isCheck = true;
        levelController.Level.CheckWin(true);
        return true;
    }
}


