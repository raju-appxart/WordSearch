using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DynamicQuestScrollView : MonoBehaviour
{

    #region PUBLIC_VARIABLES
	public static int questDiamondCnt;

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
		questDiamondCnt = 0;
		InitializeList();
		StartCoroutine(MoveTowardsTarget(0.2f, scrollRect.verticalNormalizedPosition, 1));
//		if(!isDataFilled)
//		{
//			InitializeList();

//		}
       
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
		float ratio = referenceAspect/currentAspect;
		gridLayout.cellSize = new Vector2(800*ratio, 200*ratio);
		isDataFilled = true;
        ClearOldElement();
        for (int i = 0; i < noOfItems; i++)
		{
			InitializeNewItem("Quest_" + (i + 1), i);
		}
           
		SetContentHeight(gridLayout.cellSize.x);
//		StartCoroutine(MoveTowardsTarget(0.2f, scrollRect.verticalNormalizedPosition, noOfItems-1));
    }

    private void InitializeNewItem(string name, int index)
    {
		float fillAmt = 0;
		switch(index+1)
		{
		case 1:
			//Complete First Puzzle
			if(MainDriver.Instance.questChecker.QUEST_FIRST_PUZZLE)
			{
				fillAmt = 1;
				questDiamondCnt += 5;
			}
			break;

		case 2:
			//Login To Facebook
			if(MainDriver.Instance.questChecker.QUEST_FB_LOGIN)
			{
				fillAmt = 1;
				questDiamondCnt += 5;
			}
			break;

		case 3:
			//Share With Friends
			if(MainDriver.Instance.questChecker.QUEST_FRIEND_SHARE)
			{
				fillAmt = 1;
				questDiamondCnt += 5;
			}
			break;

		case 4:
			//Invite Friends On Facebook
			if(MainDriver.Instance.questChecker.QUEST_INVITE_FRIENDS)
			{
				fillAmt = 1;
				questDiamondCnt += 5;
			}
			break;

		case 5:
			//Unlocked All Themes
			if(MainDriver.Instance.questChecker.QUEST_UNLOCK_ALL)
			{
				fillAmt = 1;
				questDiamondCnt += 5;
			}
			else
			{
				fillAmt = MainDriver.Instance.unlockedThemeCnt/16.0f;
			}
			break;

		case 6:
			//Daily Champ, Play 10 Daily Challenges
			if(MainDriver.Instance.questChecker.QUEST_DAILY_CHAMP)
			{
				fillAmt = 1;
				questDiamondCnt += 5;
			}
			else
			{
				fillAmt = MainDriver.Instance.dailyChallangeCnt/10.0f;
			}
			break;

		case 7:
			//Complete 10 Puzzles Under 1:15 each
			if(MainDriver.Instance.questChecker.QUEST_10P_T115)
			{
				fillAmt = 1;
				questDiamondCnt += 5;
			}
			else
			{
				fillAmt = MainDriver.Instance.puzzleIn115Cnt/10.0f;
			}
			break;

		case 8:
			//Complete 10 Puzzles Under 1:30 each
			if(MainDriver.Instance.questChecker.QUEST_10P_T130)
			{
				fillAmt = 1;
				questDiamondCnt += 5;
			}
			else
			{
				fillAmt = MainDriver.Instance.puzzleIn130Cnt/10.0f;
			}
			break;

		case 9:
			//Complete 10 Puzzles Under 1:45 each
			if(MainDriver.Instance.questChecker.QUEST_10P_T145)
			{
				fillAmt = 1;
				questDiamondCnt += 5;
			}
			else
			{
				fillAmt = MainDriver.Instance.puzzleIn145Cnt/10.0f;
			}
			break;

		case 10:
			//Complete 5 Consecutive Puzzles Under 2:45
			if(MainDriver.Instance.questChecker.QUEST_CC5_T245)
			{
				fillAmt = 1;
				questDiamondCnt += 5;
			}
			else
			{
				fillAmt = MainDriver.Instance.ccPuzzle245Cnt/5.0f;
			}
			break;

		case 11:
			//Complete 5 Consecutive Puzzles Under 3:00
			if(MainDriver.Instance.questChecker.QUEST_CC5_T300)
			{
				fillAmt = 1;
				questDiamondCnt += 5;
			}
			else
			{
				fillAmt = MainDriver.Instance.ccPuzzle300Cnt/5.0f;
			}
			break;

		case 12:
			//Earn a diamond on 10 Different Puzzles
			if(MainDriver.Instance.questChecker.QUEST_DIAMOND_PUZZLE_10)
			{
				fillAmt = 1;
				questDiamondCnt += 5;
			}
			else
			{
				fillAmt = MainDriver.Instance.diamondPuzzleCnt/10.0f;
			}
			break;

		case 13:
			//Earn a diamond on 15 Different Puzzles
			if(MainDriver.Instance.questChecker.QUEST_DIAMOND_PUZZLE_15)
			{
				fillAmt = 1;
				questDiamondCnt += 5;
			}
			else
			{
				fillAmt = MainDriver.Instance.diamondPuzzleCnt/15.0f;
			}
			break;

		case 14:
			//Earn a diamond on 20 Different Puzzles
			if(MainDriver.Instance.questChecker.QUEST_DIAMOND_PUZZLE_20)
			{
				fillAmt = 1;
				questDiamondCnt += 5;
			}
			else
			{
				fillAmt = MainDriver.Instance.diamondPuzzleCnt/20.0f;
			}
			break;

		case 15:
			//Earn a diamond on 25 Different Puzzles
			if(MainDriver.Instance.questChecker.QUEST_DIAMOND_PUZZLE_25)
			{
				fillAmt = 1;
				questDiamondCnt += 5;
			}
			else
			{
				fillAmt = MainDriver.Instance.diamondPuzzleCnt/25.0f;
			}
			break;

		case 16:
			//Earn a diamond on 25 Different Puzzles
			if(MainDriver.Instance.questChecker.QUEST_DIAMOND_PUZZLE_50)
			{
				fillAmt = 1;
				questDiamondCnt += 25;
			}
			else
			{
				fillAmt = MainDriver.Instance.diamondPuzzleCnt/50.0f;
			}
			break;

		case 17:
			//Earn a diamond on 25 Different Puzzles
			if(MainDriver.Instance.questChecker.QUEST_DIAMOND_PUZZLE_100)
			{
				fillAmt = 1;
				questDiamondCnt += 50;
			}
			else
			{
				fillAmt = MainDriver.Instance.diamondPuzzleCnt/100.0f;
			}
			break;

		}

        GameObject newItem = Instantiate(item) as GameObject;
        newItem.name = name;
//        newItem.transform.parent = gridLayout.transform;
		newItem.transform.SetParent(gridLayout.transform, false);
        newItem.transform.localScale = Vector3.one;

		Text questDetail =  newItem.transform.Find("Detail").transform.GetComponent<Text>();
		if(questDetail != null)
		{
			questDetail.text = Reader.Instance.QuestDataList[index].details;
		}
		Image fillerImg =  newItem.transform.Find("Filler").transform.GetComponent<Image>();
		if(fillerImg != null)
		{
			fillerImg.fillAmount = fillAmt;//Random.Range(0.0f, 1.0f);
		}

		Image doneImg =  newItem.transform.Find("Done").transform.GetComponent<Image>();
		Image diamondIcon =  newItem.transform.Find("Diamond").transform.GetComponent<Image>();
		Text dcText =  newItem.transform.Find("DCount").transform.GetComponent<Text>();
		if(fillAmt == 1)
		{
			if(doneImg != null)
			{
				doneImg.gameObject.SetActive(true);
			}
			if(diamondIcon != null)
			{
				diamondIcon.gameObject.SetActive(false);
			}
			if(dcText != null)
			{
				dcText.gameObject.SetActive(false);
			}
		}
		else
		{
			if(doneImg != null)
			{
				doneImg.gameObject.SetActive(false);
			}
			if(diamondIcon != null)
			{
				diamondIcon.gameObject.SetActive(true);
			}
			if(dcText != null)
			{
				dcText.text = "" + Reader.Instance.QuestDataList[index].diamondCnt;
				dcText.gameObject.SetActive(true);
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
		Debug.Log("Puzzle = " + puzzleID);
		GameData.puzzleID = puzzleID;
		GameEventManager.TriggerPuzzleSelection();
	}
}
