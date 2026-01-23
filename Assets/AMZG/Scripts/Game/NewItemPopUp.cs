using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewBoosterPopUp : PopUp
{
    [SerializeField] private Image avatar;

    private BoosterItemData data;

    public void SetUp(BoosterItemData data, string title = null, string content = null)
    {
        this.data = data;
        avatar.sprite = BoosterController.Instance.boostersData[(int)data.Type].AvatarSprite;
        txtTitle.text = BoosterController.Instance.boostersData[(int)data.Type].Name;
        txtContent.text = BoosterController.Instance.boostersData[(int)data.Type].Description;
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
