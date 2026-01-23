using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewBlockPopUp : PopUp
{
    [SerializeField] private Image avatar;

    private BlockData data;

    public void SetUp(BlockData data, string title = null, string content = null)
    {
        this.data = data;
        avatar.sprite = BlockController.Instance.blockDatas[(int)data.Type].AvatarSprite;
        txtTitle.text = BlockController.Instance.blockDatas[(int)data.Type].Name;
        txtContent.text = BlockController.Instance.blockDatas[(int)data.Type].Description;
    }

    public void OnClaim()
    {
        Hide();
        //BoosterItemController.Instance.UseItem(data.Type);
        /*GlobalSDKController.Instance.LogTutorialAction("claim_" + data.Name.ToLower());
        GlobalSDKController.Instance.LogEarnResource("booster", data.Name, "1", "claim_at_level_" + LevelController.Instance.GetCurrentLevel());
        UIScrewJamController.Instance.RefreshUIBoosterItem();
        GameUIController.Instance.SetActiveTutorialUnlockItem(true);*/
    }
}
