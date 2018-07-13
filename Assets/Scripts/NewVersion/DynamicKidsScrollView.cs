using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using PlayerPrefs = PreviewLabs.PlayerPrefs;

public class DynamicKidsScrollView : MonoBehaviour
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
		if(!isDataFilled)
		{
			noOfItems = Reader.Instance.kidsLevelDetail.Count;
			InitializeList();
			StartCoroutine(MoveTowardsTarget(0.2f, scrollRect.verticalNormalizedPosition, 1));
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
//		Debug.Log("gridLayout.transform.childCount = " + gridLayout.transform.childCount);
//        float scrollContentHeight = (gridLayout.transform.childCount * gridLayout.cellSize.y) + ((gridLayout.transform.childCount - 1) * gridLayout.spacing.y);
		float scrollContentHeight = (noOfItems * gridLayout.cellSize.y) + ((gridLayout.transform.childCount - 1) * gridLayout.spacing.y);
		//		scrollContent.sizeDelta = new Vector2(400, scrollContentHeight);
//		Debug.Log("scrollContentHeight = " + scrollContentHeight);
//		scrollContent.sizeDelta = new Vector2(Screen.width, scrollContentHeight);
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
			InitializeNewItem("Kids_" + (i ), i);
		}
           
		SetContentHeight(gridLayout.cellSize.x);
//		Debug.Log("scrollRect.verticalNormalizedPosition = " + scrollRect.verticalNormalizedPosition);
//		StartCoroutine(MoveTowardsTarget(0.2f, scrollRect.verticalNormalizedPosition, noOfItems-1));
    }

    private void InitializeNewItem(string name, int index)
    {
		string bestTimeKey  = Utility.KeyForBestTimeOfKidsPuzzle(index);
		int bestTime = PlayerPrefs.GetInt(bestTimeKey, 0);
        GameObject newItem = Instantiate(item) as GameObject;
        newItem.name = name;
//        newItem.transform.parent = gridLayout.transform;
		newItem.transform.SetParent(gridLayout.transform, false);
        newItem.transform.localScale = Vector3.one;
		newItem.GetComponent<Button>().onClick.AddListener(() => PuzzleSlected(index));

		//Set theme button
		Text catText =  newItem.transform.Find("Name").transform.GetComponent<Text>();
		if(catText != null)
		{
			catText.text = Reader.Instance.kidsLevelDetail[index-1];
		}

		Text bestTimeText =  newItem.transform.Find("BestTime").transform.GetComponent<Text>();
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
		
//		Image iconImg =  newItem.transform.FindChild("Icon").transform.GetComponent<Image>();
//		{
//			if(iconImg != null)
//			{
//				iconImg.sprite = Resources.Load <Sprite> ("Textures/Themes/" + Reader.Instance.ThemeDataList[index].iconImg) as Sprite;
//			}
//		}
		

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

   
	void PuzzleSlected(int puzzleID)
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayThemeSound();
		}
		GameData.gameMode = EGameMode.KidsMode;
		GameData.gameType = EGameType.KidsGame;
//		Debug.Log("Level ID = " + puzzleID);
		GameData.kidsIndex = puzzleID;
		GameData.gamePuzzleName = Reader.Instance.kidsLevelDetail[puzzleID-1];
		GameData.puzzleFileName = "K_" + GameData.kidsIndex;
		Application.LoadLevelAsync("GamePlay");
//		Debug.Log("GameData.puzzleFileName = " + GameData.puzzleFileName);

	}
}
