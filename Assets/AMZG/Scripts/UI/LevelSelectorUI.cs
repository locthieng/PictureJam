using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectorUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject content;
    [SerializeField] private Transform grid;
    [SerializeField] private Sprite[] levelAvatars;
    [SerializeField] private LevelItem levelItemPrefab;
    //[SerializeField] private NativeAdsObject nativeAdsObjectPrefab;
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private int numColumns = 1;
    [SerializeField] private int nativeAppearanceRate = 3;
    private List<LevelItem> listLevels = new List<LevelItem>();
    //private List<NativeAdsObject> listNativeAdsObject = new List<NativeAdsObject>();
    //private NativeAdsObject nativeAdsObject;
    //private bool isNativeAvailable;

    public void SetUp()
    {
        listLevels.Clear();
        for (int i = 0; i < grid.childCount; i++)
        {
            Destroy(grid.GetChild(i).gameObject);
        }
        //GoogleAdsController.Instance.RequestNativeAdForLevelSelector();
        //listNativeAdsObject.Clear();
        //isNativeAvailable = !DataController.Instance.Stats.NoAds;// GoogleAdsController.Instance.nativeAdForLevelSelector != null;
        for (int i = 0; i < StageController.Instance.LevelLimit; i++)
        {
            LevelItem level = Instantiate(levelItemPrefab, grid);
            level.SetUp(i + 1, levelAvatars[i]);
            level.buttonScript.draggableRoot = scroll;
            listLevels.Add(level);
            //if (i % nativeAppearanceRate == 0 && isNativeAvailable)
            //{
            //    nativeAdsObject = Instantiate(nativeAdsObjectPrefab, grid);
            //    GoogleAdsController.Instance.ParseNativeAdToLevelSelector(nativeAdsObject);
            //    listNativeAdsObject.Add(nativeAdsObject);
            //}
        }
    }

    IEnumerator CoReposition()
    {
        yield return null;
        LeanTween.moveLocal(scroll.content.gameObject, scroll.GetSnapToPositionToBringChildIntoView(listLevels[LevelController.Instance.GetCurrentLevel() - 1].GetComponent<RectTransform>()), 0.4f);
    }

    private int nativeLoadedCount = 0;
    private void Update()
    {
        //if (listNativeAdsObject.Count > nativeLoadedCount && GoogleAdsController.Instance.nativeAdForLevelSelector != null)
        //{
        //    GoogleAdsController.Instance.ParseNativeAdToLevelSelector(listNativeAdsObject[nativeLoadedCount]);
        //    nativeLoadedCount++;
        //}
    }


    public void Show()
    {
        if (listLevels.Count == 0)
        {
            SetUp();
        }
        gameObject.SetActive(true);
        LeanTween.alphaCanvas(canvasGroup, 1, 0.1f);
        content.transform.localScale = Vector3.one * 0.7f;
        LeanTween.scale(content, Vector3.one, 0.2f);
        canvasGroup.blocksRaycasts = true;
        Refresh();
        StartCoroutine(CoReposition());
    }

    public void Refresh()
    {
        for (int i = 0; i < listLevels.Count; i++)
        {
            listLevels[i].Refresh();
        }
    }

    public void Hide()
    {
        //LeanTween.alphaCanvas(canvasGroup, 0, 0.2f);
        //LeanTween.scale(content, Vector3.one * 0.7f, 0.2f).setOnComplete(() =>
        //{
        //});
        gameObject.SetActive(false);
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
}
