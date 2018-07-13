using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PlayerPrefs = PreviewLabs.PlayerPrefs;
using System;

public class DynamicPuzzleScrollView : MonoBehaviour
{

    #region PUBLIC_VARIABLES
    public int noOfItems;

    public GameObject item;

    public GridLayoutGroup gridLayout;

    public RectTransform scrollContent;

    public ScrollRect scrollRect;
    #endregion

    #region PRIVATE_VARIABLES
	private bool isDataFilled = false;
    #endregion

    #region UNITY_CALLBACKS
    void OnEnable()
    {
//		if(!isDataFilled)
//			Debug.Log("noOfItems = " + noOfItems);
        InitializeList();
		StartCoroutine(MoveTowardsTarget(0.2f, scrollRect.verticalNormalizedPosition, 1));
		Mask scrollMask =  scrollRect.GetComponent<Mask>();
		if(scrollMask != null)
		{
//			Debug.Log("scrollMaskscrollMaskscrollMask");
			scrollMask.enabled = false;
			scrollMask.enabled = true;
		}
    }
    #endregion

    #region PUBLIC_METHODS
    #endregion

    #region PRIVATE_METHODS
    private void ClearOldElement()
    {
        for (int i = 0; i < gridLayout.transform.childCount; i++)
        {
//			Debug.Log("Destroy");
            Destroy(gridLayout.transform.GetChild(i).gameObject);
        }
    }



	public void SetContentHeight(float width)
    {
//        float scrollContentHeight = (gridLayout.transform.childCount * gridLayout.cellSize.y) + ((gridLayout.transform.childCount - 1) * gridLayout.spacing.y);
		float scrollContentHeight = (noOfItems * gridLayout.cellSize.y) + ((gridLayout.transform.childCount - 1) * gridLayout.spacing.y);
//		scrollContent.sizeDelta = new Vector2(400, scrollContentHeight);
		scrollContent.sizeDelta = new Vector2(width, scrollContentHeight);
    }

    private void InitializeList()
    {
		float referenceAspect = 1.6f;
		float currentAspect = ((float)Screen.height/Screen.width);
//		Debug.Log("currentAspect = " + currentAspect);
		float ratio = referenceAspect/currentAspect;
//		Debug.Log("ratio = " + ratio);
		gridLayout.cellSize = new Vector2(800*ratio, 200*ratio);
//		gridLayout.cellSize = new Vector2(Screen.width, 200);
		isDataFilled = true;
        ClearOldElement();
        for (int i = 1; i <= noOfItems; i++)
		{
			InitializeNewItem("Puzzle_" + (i), i);
		}
           
		SetContentHeight(gridLayout.cellSize.x);
//		Debug.Log("scrollRect.verticalNormalizedPosition = " + scrollRect.verticalNormalizedPosition);
//		StartCoroutine(MoveTowardsTarget(0.2f, scrollRect.verticalNormalizedPosition, noOfItems-1));
    }

    private void InitializeNewItem(string name, int index)
    {
        GameObject newItem = Instantiate(item) as GameObject;
        newItem.name = name;
//        newItem.transform.parent = gridLayout.transform;
		newItem.transform.SetParent(gridLayout.transform, false);
        newItem.transform.localScale = Vector3.one;
		newItem.GetComponent<Button>().onClick.AddListener(() => PuzzleSelected(index));

		Text puzzleName =  newItem.transform.Find("Name").transform.GetComponent<Text>();
		if(puzzleName != null)
		{
			puzzleName.text = Reader.Instance.SubLevelDetails[GameData.mainThemeIndex-1][index-1];
		}
		Image iconImg =  newItem.transform.Find("Icon").transform.GetComponent<Image>();
		if(iconImg != null)
		{
			iconImg.sprite = Resources.Load <Sprite> ("Textures/Themes/" + Reader.Instance.ThemeDataList[GameData.mainThemeIndex-1].iconImg) as Sprite;
		}

		string diamondKey = Utility.KeyForDiamondsOfPuzzle(GameData.mainThemeIndex,index);
//		Debug.Log("diamondKey = " + diamondKey);
		int puzzleDiamondCnt = PlayerPrefs.GetInt(diamondKey, 0);

		string bestTimeKey  = Utility.KeyForBestTimeOfPuzzle(GameData.mainThemeIndex,index) ;
		int bestTime = PlayerPrefs.GetInt(bestTimeKey, 0);


		Image diamondIcon =  newItem.transform.Find("Diamond").transform.GetComponent<Image>();
		Text diamondCount =  newItem.transform.Find("Count").transform.GetComponent<Text>();
		Text bestTimeText =  newItem.transform.Find("BestTime").transform.GetComponent<Text>();

		if(puzzleDiamondCnt != 0)
		{
			if(diamondIcon != null)
			{
				diamondIcon.gameObject.SetActive(true);
			}
			if(diamondCount != null)
			{
				diamondCount.text = "" + puzzleDiamondCnt;
			}
		}
		else
		{
			if(diamondIcon != null)
			{
				diamondIcon.gameObject.SetActive(false);
			}
			if(diamondCount != null)
			{
				diamondCount.gameObject.SetActive(false);
			}
		}

		if(bestTime == 0)
		{
			if(bestTimeText != null)
			{
				bestTimeText.text = "";
			}
		}
		else
		{
			if(bestTimeText != null)
			{
				bestTimeText.text = "Best Time - " + String.Format("{0:D2}:{1:D2}", bestTime/60, bestTime%60);;
			}
		}


        newItem.SetActive(true);
    }
    #endregion

    #region PRIVATE_COROUTINES
    private IEnumerator MoveTowardsTarget(float time,float from,float target) {
        float i = 0;
        float rate = 1 / time;
        while(i<1)
		{
            i += rate * Time.deltaTime;
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(from,target,i);
            yield return 0;
        }
    }
    #endregion

   
	void PuzzleSelected(int puzzleID)
	{
//		Debug.Log("Puzzle = " + puzzleID);
		GameData.puzzleID = puzzleID;
		GameEventManager.TriggerPuzzleSelection();
	}
}
