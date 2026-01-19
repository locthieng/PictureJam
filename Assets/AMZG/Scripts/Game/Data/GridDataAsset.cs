using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GridData", menuName = "Level/Grid Data", order = 1)]
public class GridDataAsset : ScriptableObject
{
    public int width;
    public int height;
    public List<bool> grid;
    public List<bool> gridHole;

}