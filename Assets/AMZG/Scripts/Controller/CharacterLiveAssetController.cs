using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLiveAssetController : MonoBehaviour
{
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private Mesh[] skinMeshes;
    [SerializeField] private SkinnedMeshRenderer skinRenderer;
    [SerializeField] private List<int> attachedWeapons = new List<int>();
    private int lastActiveWeaponID;

    private void OnValidate()
    {
        if (attachedWeapons.Count < skinMeshes.Length)
        {
            for (int i = 0; i < skinMeshes.Length - attachedWeapons.Count; i++)
            {
                attachedWeapons.Add(-1);
            }
        }
    }

    public void ChangeWeapon(int id)
    {
        lastActiveWeaponID = id;
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                weapons[i].SetActive(i == id);
            }
        }
    }

    public void ChangeSkin(int id)
    {
        if (weapons.Length > 0)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i] != null)
                {
                    weapons[i].SetActive(false);
                }
            }
            if (attachedWeapons[id] > -1)
            {
                weapons[attachedWeapons[id]].SetActive(true);
            }
            else if (weapons.Length > lastActiveWeaponID && lastActiveWeaponID > -1 && weapons[lastActiveWeaponID] != null)
            {
                weapons[lastActiveWeaponID].SetActive(true);
            }
        }
        if (skinRenderer != null && skinMeshes.Length > id)
        {
            skinRenderer.sharedMesh = skinMeshes[id];
        }
    }
}
