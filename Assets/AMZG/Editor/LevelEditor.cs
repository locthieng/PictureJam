using Codice.CM.Client.Differences;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(LevelController))]
public class LevelEditor : Editor
{
    private LevelController levelController;
    private SerializedProperty enableSnapProp;

    //[HideInInspector] public bool isHole;
    private void OnEnable()
    {
        levelController = (LevelController)target;
        enableSnapProp = serializedObject.FindProperty("enableSnapInEditor");
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (levelController == null || !levelController.EnableSnapInEditor) return;

        foreach (MoveBlock t in levelController.Level.moveBlocks)
        {
            if (t == null) continue;

            // Snap only if it's selected and moved
            if (Selection.transforms.Contains(t.transform) && GUIUtility.hotControl == 0)
            {
                Vector3 oldPos = t.transform.position;
                Vector3 snappedPos = levelController.GetSnappedPosition(oldPos);

                if (Vector3.Distance(oldPos, snappedPos) > 0.01f)
                {
                    Undo.RecordObject(t.transform, "Snap MoveBlock to Grid");
                    t.transform.position = snappedPos;
                }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        levelController = (LevelController)target;

        GUIStyle headerStyle = new GUIStyle();
        headerStyle.richText = true;
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.normal.textColor = Color.cyan;

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("LEVEL", headerStyle);
        EditorGUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();

        levelController.CurrentLevel = EditorGUILayout.IntField(levelController.CurrentLevel, GUILayout.Width(50), GUILayout.Height(25));

        if (GUILayout.Button("<<", GUILayout.Width(40), GUILayout.Height(25)))
        {
            levelController.EditorLoadPrevLevel();
        }

        if (GUILayout.Button(">>", GUILayout.Width(40), GUILayout.Height(25)))
        {
            levelController.EditorLoadNextLevel();
        }

        if (GUILayout.Button("LOAD", GUILayout.Width(60), GUILayout.Height(25)))
        {
            levelController.EditorLoadLevel(levelController.CurrentLevel);
        }

        EditorGUILayout.EndHorizontal();

        // Toggle

        /*GUILayout.BeginVertical("BOX");

        // Dòng 1: Nút bật tắt Hole
        GUILayout.BeginHorizontal();
        levelController.isHole = EditorGUILayout.Toggle("Hole", levelController.isHole);
        GUILayout.EndHorizontal();

        // Dòng 2: Nếu isHole được tích chọn (true), thì hiện danh sách lựa chọn
        if (levelController.isHole)
        {
            EditorGUI.indentLevel++;

            levelController.selectedHoleSize = (HoleSize)EditorGUILayout.EnumPopup("Size:", levelController.selectedHoleSize);

            // 2. Tự động tạo đường dẫn dựa trên Size đang chọn
            // Ví dụ: "Holes/Size1x1"
            string folderPath = "AMZG/Holes/" + levelController.selectedHoleSize.ToString();

            // 3. Lục tìm tất cả Sprite trong folder đó
            Sprite[] loadedSprites = Resources.LoadAll<Sprite>(folderPath);

            if (loadedSprites != null && loadedSprites.Length > 0)
            {
                EditorGUILayout.LabelField($"Tìm thấy {loadedSprites.Length} Sprite trong {folderPath}:");

                // Vẽ bảng chọn hình ảnh
                int selectedIndex = System.Array.IndexOf(loadedSprites, levelController.currentHoleSprite);

                Texture[] textures = new Texture[loadedSprites.Length];
                for (int i = 0; i < loadedSprites.Length; i++) textures[i] = loadedSprites[i].texture;

                // Hiển thị Grid hình ảnh (4 cột)
                int newIndex = GUILayout.SelectionGrid(selectedIndex, textures, 4, GUILayout.Width(250));

                if (newIndex >= 0)
                {
                    levelController.currentHoleSprite = loadedSprites[newIndex];
                }
            }
            else
            {
                EditorGUILayout.HelpBox($"Không tìm thấy Sprite nào tại: Resources/{folderPath}", MessageType.Info);
            }
            levelController.selectedHoleType = (HoleType)EditorGUILayout.EnumPopup("Type:", levelController.selectedHoleType);

            EditorGUI.indentLevel--;
        }

        GUILayout.EndVertical();*/



        // Grid
        // Grid size

        EditorGUILayout.Space(10);

        /*levelController.gridWidth = EditorGUILayout.IntField("Grid Width", levelController.gridWidth);
        levelController.gridHeight = EditorGUILayout.IntField("Grid Height", levelController.gridHeight);

        if (levelController.gridData == null ||
            levelController.gridData.GetLength(0) != levelController.gridWidth ||
            levelController.gridData.GetLength(1) != levelController.gridHeight)
        {
            levelController.gridData = new bool[levelController.gridWidth, levelController.gridHeight];
            levelController.gridDataHole = new bool[levelController.gridWidth, levelController.gridHeight];
        }*/

        EditorGUILayout.Space(10);

        // =========================
        // === DRAW 2 GRIDS SIDE BY SIDE ===
        // =========================
        /*GUILayout.BeginHorizontal();

        // --- MAIN GRID ---
        GUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("BACK GRID", EditorStyles.boldLabel);
        for (int y = levelController.gridHeight - 1; y >= 0; y--)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < levelController.gridWidth; x++)
            {
                bool hasCell = levelController.gridData[x, y];
                Color prevColor = GUI.backgroundColor;
                GUI.backgroundColor = hasCell ? new Color(0.3f, 1f, 0.3f) : new Color(0.6f, 0.6f, 0.6f);

                if (GUILayout.Button(hasCell ? "X" : "", GUILayout.Width(25), GUILayout.Height(25)))
                {
                    if (hasCell)
                        levelController.EditorDeleteCell(x, y);
                    else
                        levelController.EditorCreateCell(x, y);

                    EditorUtility.SetDirty(levelController);
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }

                GUI.backgroundColor = prevColor;
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        GUILayout.Space(20);

        // --- HOLE GRID ---
        GUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("HOLE GRID", EditorStyles.boldLabel);
        if (levelController.gridDataHole == null ||
            levelController.gridDataHole.GetLength(0) != levelController.gridWidth ||
            levelController.gridDataHole.GetLength(1) != levelController.gridHeight)
        {
            levelController.gridDataHole = new bool[levelController.gridWidth, levelController.gridHeight];
        }

        for (int y = levelController.gridHeight - 1; y >= 0; y--)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < levelController.gridWidth; x++)
            {
                bool hasCell = levelController.gridDataHole[x, y];
                Color prevColor = GUI.backgroundColor;
                GUI.backgroundColor = hasCell ? new Color(1f, 0.8f, 0.3f) : new Color(0.4f, 0.4f, 0.4f);

                if (GUILayout.Button(hasCell ? "X" : "", GUILayout.Width(25), GUILayout.Height(25)))
                {
                    EditorUtility.SetDirty(levelController);
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }

                GUI.backgroundColor = prevColor;
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();*/

        EditorGUILayout.Space(15);

        // =========================
        // === CREATE / CLEAR BUTTONS ===
        // =========================
        GUILayout.BeginHorizontal();
        /*if (GUILayout.Button("Create Grid", GUILayout.Height(30)))
        {
            levelController.EditorCreateGrid();
            EditorUtility.SetDirty(levelController);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        if (GUILayout.Button("Clear Grid", GUILayout.Height(30)))
        {
            levelController.EditorClearGrid();
            EditorUtility.SetDirty(levelController);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }*/


        EditorGUILayout.EndHorizontal(); 

        EditorGUILayout.Space(10);

        if (GUILayout.Button("SAVE LEVEL", GUILayout.Width(150), GUILayout.Height(40)))
        {
            levelController.EditorSaveLevel();
        }

        EditorGUILayout.EndVertical();

        GUILayout.Space(10);

        EditorGUILayout.PropertyField(enableSnapProp, new GUIContent("Enable Snap in Editor"));

        if (GUILayout.Button("Refresh MoveBlocks Cache"))
        {
            levelController.RefreshMoveBlocks();
        }

        if (GUILayout.Button("Snap All MoveBlocks"))
        {
            foreach (MoveBlock t in levelController.Level.moveBlocks)
            {
                if (t == null) continue;
                Undo.RecordObject(t.transform, "Snap MoveBlock to Grid");
                t.transform.position = levelController.GetSnappedPosition(t.transform.position);
            }
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Grid Visual and Walls"))
        {
            levelController.GenerateGridVisual(); // Tạo grid và tường
        }

        if (GUILayout.Button("Clear Grid Visual and Walls"))
        {
            levelController.ClearGeneratedVisuals(); // Xóa tất cả các tile visual và tường
        }

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(levelController);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(levelController);
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(levelController.gameObject.scene);
            }
        }
    }
}