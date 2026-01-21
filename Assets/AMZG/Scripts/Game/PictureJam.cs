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
        //CheckPoint();
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

    [ContextMenu("Setup True Points From Texture")]
    public void SetupTruePoints()
    {
        if (childBlocks == null || childBlocks.Length == 0)
        {
            Debug.LogError("Em ơi, hãy gán các mảnh ghép vào childBlocks trước nhé!");
            return;
        }

        // 1. Tạo hoặc tìm Object cha
        GameObject pointsParent = GameObject.Find("Level_TruePoints_" + gameObject.name);
        if (pointsParent != null) DestroyImmediate(pointsParent);
        pointsParent = new GameObject("Level_TruePoints_" + gameObject.name);
        pointsParent.transform.SetParent(this.transform.parent);

        truePoints = new Transform[childBlocks.Length];

        for (int i = 0; i < childBlocks.Length; i++)
        {
            // 2. Lấy Material của mảnh ghép
            Renderer renderer = childBlocks[i].GetComponent<Renderer>();
            if (renderer == null) continue;

            // Lấy thông số Offset và Tiling từ Shader (Thường là _MainTex hoặc tên texture trong shader)
            // Dựa vào hình em gửi, tên thuộc tính thường là "_MainTex" hoặc "_ParticleTexture"
            Vector2 offset = renderer.sharedMaterial.GetTextureOffset("_MainTex");
            // Nếu code trên không chạy, hãy thử đổi thành "_ParticleTexture" theo tên trong hình em chụp

            // 3. Tính toán vị trí dựa trên Offset
            // Nếu Offset Y là âm (texture bị đẩy xuống), thì Position Y phải dương (vật thể phải đưa lên)
            Vector3 calculatedPos = new Vector3(
                this.transform.position.x - offset.x,
                this.transform.position.y, // Giữ nguyên Y nếu em chỉ muốn khớp theo chiều ngang
                this.transform.position.z - offset.y  // Thường trong 3D, chiều dọc texture ứng với trục Z hoặc Y
            );

            // Lưu ý: Nếu game của em là 2D/3D dọc, em hãy đổi trục phù hợp:
            // calculatedPos.y = this.transform.position.y - offset.y;

            // 4. Tạo Object điểm đích
            GameObject tp = new GameObject("TruePoint_" + childBlocks[i].name);
            tp.transform.SetParent(pointsParent.transform);
            tp.transform.position = calculatedPos;

            truePoints[i] = tp.transform;
        }

        Debug.Log("Thầy đã tính toán xong vị trí dựa trên Offset của Texture cho em rồi nhé!");

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

}


