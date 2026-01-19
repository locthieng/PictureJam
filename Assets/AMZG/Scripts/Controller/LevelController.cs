using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AI;
using UnityEditor.SceneManagement;

#endif
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public enum HoleSize { Size1x1, Size2x3, Size3x2 }
public enum HoleType { Normal, Vertical, Horizontal, Ice, Immovable}

public class LevelController : Singleton<LevelController>
{
    private string levelPrefabPath = "StageLevel/";
    private string levelPrefabPathSpecial = "SpecialLevel/";
    private string levelPrefabBasePath = "Base/";
    private string prefabPath = "/AMZG/Prefabs/Resources/";
    private const string levelPrefix = "Level_";
    public const float CellSpacing = 1.0f;
    [HideInInspector] public int CurrentLevel = 0;
    [HideInInspector] public SingleLevelController LoadedLevel;
    [HideInInspector] public SingleLevelController Level;
    [HideInInspector] public int levelIndex;
    [HideInInspector] public List<LevelAsset> ListLevelSpecials = new List<LevelAsset>();
    //public StoryAsset[] ListStoryAssets;

    [HideInInspector] public int gridWidth = 5;
    [HideInInspector] public int gridHeight = 5;

    private Grid grid;

    public bool[,] gridData;
    public bool[,] gridDataHole;

    [HideInInspector] public bool isHole; 
    [HideInInspector] public HoleSize selectedHoleSize; 
    [HideInInspector] public HoleType selectedHoleType;
    [HideInInspector] public Sprite currentHoleSprite;

    public GameObject cellPrefab;


    protected override void Awake()
    {
        base.Awake();

    }
    public int GetNextLevelInOrder()
    {
        if (GlobalController.Instance.ForTesting) return GlobalController.CurrentLevelIndex + 1;
        DataController.Instance.Data.Levels.Sort(SortLevelsAccending);
        int level = 1;
        for (int i = 0; i < DataController.Instance.Data.Levels.Count; i++)
        {
            level = DataController.Instance.Data.Levels[i] + 1;
            if (!DataController.Instance.Data.Levels.Contains(level))
            {
                return level;
            }
        }
        return level;
    }
    private int SortLevelsAccending(int x, int y)
    {
        if (x < y)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }

    public int GetCurrentLevel()
    {
        levelIndex = GlobalController.CurrentLevelIndex % StageController.Instance.LevelLimit;
        if (levelIndex == 0)
        {
            levelIndex = StageController.Instance.LevelLimit;
        }
        return levelIndex;
    }

    /// <summary>
    /// Load a level map & return the map index
    /// </summary>
    /// <param name="level"></param>
    /// <param name="levelLimit"></param>
    /// <param name="forcedChange"></param>
    /// <returns></returns>
    public int LoadLevel(int level, int levelLimit, bool forcedChange = false)
    {
        Level = transform.GetComponentInChildren<SingleLevelController>();
        if (Level != null)
        {
            if (forcedChange)
            {
                Destroy(Level.gameObject);
            }
            else
            {
                //CurrentLevel = int.Parse(Level.name.Split('_')[1]);
                //LoadedLevel = Level;
                //return CurrentLevel; // For GD Test
            }
        }
        levelIndex = level % levelLimit;
        if (levelIndex == 0)
        {
            levelIndex = levelLimit;
        }
        switch (GlobalController.CurrentLevelType)
        {
            case LevelType.Main:
                LoadedLevel = Resources.Load<SingleLevelController>(levelPrefabPath + levelPrefix + levelIndex);
                break;
            case LevelType.Special:
                LoadedLevel = ListLevelSpecials[GlobalController.CurrentLevelSpecialIndex].Prefab;
                break;
            //case LevelType.Story:
            //    LoadedLevel = ListStoryAssets[(int)GlobalController.CurrentStory].ListLevels[GlobalController.CurrentStoryLevelIndex].Prefab;
            //    break;
            default:
                break;
        }
        CurrentLevel = levelIndex;
        return level;
    }

    public void SetUpLevel()
    {
        if (LoadedLevel != null && Level == null)
        {
            Level = Instantiate(LoadedLevel, transform);
        }
    }

#if UNITY_EDITOR
    public void EditorAddLevel()
    {
        Instance = this;
        Level = transform.GetComponentInChildren<SingleLevelController>();
        if (Level != null)
        {
            Object prefabParent = PrefabUtility.GetCorrespondingObjectFromSource(Level.gameObject);
            if (prefabParent != null)
            {
                PrefabUtility.SaveAsPrefabAsset(
                    Level.gameObject,
                    AssetDatabase.GetAssetPath(prefabParent));
            }
            DestroyImmediate(Level.gameObject);
            AssetDatabase.Refresh();
        }
        Level = Instantiate(Resources.Load<SingleLevelController>(levelPrefabBasePath + "Level"), transform);
        CurrentLevel = Resources.LoadAll<SingleLevelController>(levelPrefabPath).Length;
        CurrentLevel++;
        Level.name = levelPrefix + CurrentLevel;
        PrefabUtility.SaveAsPrefabAsset(
                    Level.gameObject,
                    Application.dataPath + prefabPath + levelPrefabPath + Level.name + ".prefab");
    }

    public void EditorLoadLevel(int level)
    {
        Instance = this;
        Level = transform.GetComponentInChildren<SingleLevelController>();
        if (Level != null)
        {
            Object prefabParent = PrefabUtility.GetCorrespondingObjectFromSource(Level.gameObject);
            if (prefabParent != null)
            {
                PrefabUtility.SaveAsPrefabAsset(
                    Level.gameObject,
                    AssetDatabase.GetAssetPath(prefabParent));
            }
            DestroyImmediate(Level.gameObject);
            AssetDatabase.Refresh();
        }
        GameObject g = PrefabUtility.InstantiatePrefab(Resources.Load(levelPrefabPath + levelPrefix + level), transform) as GameObject;
        if (g == null)
        {
            Debug.LogError("No prefabs found for level " + CurrentLevel + ". Switching back to Level 1.");
            CurrentLevel = 1;
            EditorLoadLevel(CurrentLevel);
            return;
        }
        Level = g.GetComponent<SingleLevelController>();
        Level.name = levelPrefix + level;
        if (gridData == null || gridData.GetLength(0) != gridWidth || gridData.GetLength(1) != gridHeight)
        {
            gridData = new bool[gridWidth, gridHeight];
        }

        if (gridDataHole == null || gridDataHole.GetLength(0) != gridWidth || gridDataHole.GetLength(1) != gridHeight)
        {
            gridDataHole = new bool[gridWidth, gridHeight];
        }

        Grid existingGrid = Level.GetComponentInChildren<Grid>();
        if (existingGrid == null)
        {
            GameObject gridObj = new GameObject("Grid");
            gridObj.transform.SetParent(Level.transform);
            gridObj.transform.localPosition = Vector3.zero;
            grid = gridObj.AddComponent<Grid>();
        }
        else
        {
            grid = existingGrid;
        }

        LoadGridDataFromAsset();
    }

    public void EditorLoadPrevLevel()
    {
        CurrentLevel--;
        EditorLoadLevel(CurrentLevel);
    }


    public void EditorLoadNextLevel()
    {
        CurrentLevel++;
        EditorLoadLevel(CurrentLevel);
    }

    public void EditorCloneLevel(int level)
    {
        Instance = this;
        Level = transform.GetComponentInChildren<SingleLevelController>();
        if (Level != null)
        {
            Object prefabParent = PrefabUtility.GetCorrespondingObjectFromSource(Level.gameObject);
            string basePath = AssetDatabase.GetAssetPath(prefabParent);
            basePath = basePath.Substring(0, basePath.Length - (levelPrefix + level).Length - ".prefab".Length);
            PrefabUtility.SaveAsPrefabAsset(
                Level.gameObject,
                AssetDatabase.GetAssetPath(prefabParent));
            SingleLevelController[] levels = Resources.LoadAll<SingleLevelController>(levelPrefabPath);
            CurrentLevel = levels.Length + 1;
            AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(prefabParent), basePath + (levelPrefix + CurrentLevel) + ".prefab");
            DestroyImmediate(Level.gameObject);
            AssetDatabase.Refresh();
            GameObject g = PrefabUtility.InstantiatePrefab(Resources.Load(levelPrefabPath + levelPrefix + CurrentLevel), transform) as GameObject;
            Level = g.GetComponent<SingleLevelController>();
            Level.name = levelPrefix + CurrentLevel;
        }
    }

    public void EditorCreateGrid()
    {
        if (Level == null) return;

        EditorClearGrid();

        if (grid == null)
        {
            GameObject gridObj = new GameObject("Grid");
            gridObj.transform.SetParent(Level.transform);
            gridObj.transform.localPosition = Vector3.zero;
            grid = gridObj.AddComponent<Grid>();
        }

        gridData = new bool[gridWidth, gridHeight];
        gridDataHole = new bool[gridWidth, gridHeight];

        float spacing = 1f;

        Vector3 originOffset = new Vector3(
            -(gridWidth - 1) * spacing * 0.5f,
            0f,
            -(gridHeight - 1) * spacing * 0.5f
        );

        for (int z = 0; z < gridHeight; z++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector3 pos = new Vector3(
                    x * spacing,
                    0f,
                    z * spacing
                ) + originOffset;

                GameObject cell = (GameObject)PrefabUtility.InstantiatePrefab(cellPrefab, grid.transform);
                cell.transform.localPosition = pos;
                cell.name = $"Cell_{x}_{z}";

                gridData[x, z] = true;
                gridDataHole[x, z] = true;
            }
        }

        EditorUtility.SetDirty(this);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    public void EditorClearGrid()
    {
        if (grid != null)
        {
            Object.DestroyImmediate(grid.gameObject);
            grid = null;
        }

        Transform cellRoot = Level.transform.Find("Grid");
        if (cellRoot != null)
            Object.DestroyImmediate(cellRoot.gameObject);

        if (gridData != null)
            gridData = new bool[gridWidth, gridHeight];

        if (gridDataHole != null)
            gridDataHole = new bool[gridWidth, gridHeight];

        EditorUtility.SetDirty(this);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }    


    public void EditorSaveLevel()
    {
        Instance = this;
        if (Level == null)
        {
            Level = transform.GetComponentInChildren<SingleLevelController>();
        }
        if (Level != null)
        {
            GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(Level.gameObject);
            if (prefabRoot != null)
            {
                PrefabUtility.UnpackPrefabInstance(prefabRoot, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
            }

            // Recheck all of level's objects
            SaveGridDataToAsset();
            DoSaveAssetDatabase();
        }
    }

    private void DoSaveAssetDatabase()
    {
        Object instanceRoot = PrefabUtility.GetCorrespondingObjectFromSource(Level.gameObject);
        GameObject prefabParent = PrefabUtility.GetOutermostPrefabInstanceRoot(Level.gameObject);
        if (instanceRoot == null)
        {
            PrefabUtility.SaveAsPrefabAssetAndConnect(
                Level.gameObject,
                Application.dataPath + prefabPath + levelPrefabPath + Level.name + ".prefab", InteractionMode.AutomatedAction);
        }
        else
        {
            PrefabUtility.SaveAsPrefabAsset(
                Level.gameObject,
                AssetDatabase.GetAssetPath(instanceRoot));
            PrefabUtility.RevertPrefabInstance(prefabParent, InteractionMode.AutomatedAction);
        }
        AssetDatabase.Refresh();
        EditorApplication.update -= DoSaveAssetDatabase;
    }

    public void EditorUpdateNumObstacle(int type)
    {
        Instance = this;

    }

    public void EditorUpdateNumFloor()
    {
        Instance = this;

    }

    public void EditorUpdateNumLoot(int type)
    {
        Instance = this;

    }

    public void EditorUpdateNumTrap(int type)
    {
        Instance = this;

    }

    public void EditorRelinkAllObstacles()
    {
        Instance = this;
        EditorUnpackPrefab(Level.gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);

    }

    public void EditorSaveAll()
    {
        Instance = this;

        int totalLevels = Resources.LoadAll<SingleLevelController>(levelPrefabPath).Length;
        for (int i = 1; i < totalLevels + 1; i++)
        {
            EditorLoadLevel(i);
            // DO SOMETHING WITH THE CURRENT LEVEL
            // -----------------------------------
            EditorSaveLevel();
        }
    }

    public void EditorUnpackPrefab(GameObject currentObject, PrefabUnpackMode unpackMode, InteractionMode interactionMode)
    {
        EditorSaveLevel();
        GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(currentObject);
        if (prefabRoot != null)
        {
            PrefabUtility.UnpackPrefabInstance(prefabRoot, unpackMode, interactionMode);
        }
    }

    public void RemoveSpecialLevel(int i)
    {
        ListLevelSpecials.RemoveAt(i);
    }

    public void AddSpecialLevel()
    {
        ListLevelSpecials.Add(new LevelAsset()
        {
            ID = ListLevelSpecials.Count
        });
    }

    public void EditorDeleteCell(int x, int y)
    {
        if (Level == null) return;
        if (grid == null) return;

        Transform t = grid.transform.Find($"Cell_{x}_{y}");
        if (t != null)
        {
            Object.DestroyImmediate(t.gameObject);
            gridData[x, y] = false;
            gridDataHole[x, y] = false; // đồng bộ
        }
    }

    public void EditorCreateCell(int x, int y)
    {
        if (Level == null) return;
        if (grid == null || cellPrefab == null) return;
        Transform exist = grid.transform.Find($"Cell_{x}_{y}");
        if (exist != null) return;

        Vector3 originOffset = new Vector3(
            -(gridWidth - 1) * CellSpacing * 0.5f,
            -(gridHeight - 1) * CellSpacing * 0.5f,
            0
        );
        Vector3 pos = new Vector3(x * CellSpacing, y * CellSpacing, 0) + originOffset;
        GameObject newCell = (GameObject)PrefabUtility.InstantiatePrefab(cellPrefab, grid.transform);
        newCell.transform.localPosition = pos;
        newCell.name = $"Cell_{x}_{y}";
        gridData[x, y] = true;
        gridDataHole[x, y] = true; // đồng bộ
    }

    private void LoadGridDataFromAsset()
    {
        if (Level == null) return;

        string path = $"GridData/{Level.name}_GridData";
        GridDataAsset data = Resources.Load<GridDataAsset>(path);
        if (data == null)
        {
            Debug.LogWarning($"⚠️ Grid data asset not found: {path}");
            return;
        }

        Level.DataAsset = data;

        gridWidth = data.width;
        gridHeight = data.height;
        gridData = new bool[gridWidth, gridHeight];
        gridDataHole = new bool[gridWidth, gridHeight];

        int index = 0;
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                gridData[x, y] = data.grid[index];
                gridDataHole[x, y] = data.gridHole[index];
                index++;
            }
        }

    }
    private void SaveGridDataToAsset()
    {
        if (Level == null) return;

        string folderPath = "Assets/Resources/GridData";
        if (!System.IO.Directory.Exists(folderPath))
            System.IO.Directory.CreateDirectory(folderPath);

        GridDataAsset data = ScriptableObject.CreateInstance<GridDataAsset>();
        data.width = gridWidth;
        data.height = gridHeight;
        data.grid = new List<bool>();
        data.gridHole = new List<bool>();

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                data.grid.Add(gridData != null && gridData[x, y]);
                data.gridHole.Add(gridDataHole != null && gridDataHole[x, y]);
            }
        }
        string path = $"{folderPath}/{Level.name}_GridData.asset";
        UnityEditor.AssetDatabase.CreateAsset(data, path);
        UnityEditor.AssetDatabase.SaveAssets();
    }

    internal void ShowHint()
    {
    }

    //public void AddStoryLevel(int storyID)
    //{
    //    if (ListStoryAssets[storyID].ListLevels == null)
    //    {
    //        ListStoryAssets[storyID].ListLevels = new List<LevelAsset>();
    //    }
    //    ListStoryAssets[storyID].ListLevels.Add(new LevelAsset()
    //    {
    //        ID = ListStoryAssets[storyID].ListLevels.Count
    //    });
    //}

    //public void RemoveStoryLevel(int storyID, int levelID)
    //{
    //    if (ListStoryAssets[storyID].ListLevels != null)
    //    {
    //        ListStoryAssets[storyID].ListLevels.RemoveAt(levelID);
    //    }
    //}
#endif
}
