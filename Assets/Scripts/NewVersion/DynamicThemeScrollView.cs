using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PlayerPrefs = PreviewLabs.PlayerPrefs;

public class DynamicThemeScrollView : MonoBehaviour
{

    #region PUBLIC_VARIABLES
    public int noOfItems;

    public GameObject item;

    public GridLayoutGroup gridLayout;

    public RectTransform scrollContent;

    public ScrollRect scrollRect;

	public RectTransform unlockPanel, noDiamondPanel;
	public Text requiredText, unlockText;
    #endregion

    #region PRIVATE_VARIABLES
	private bool isDataFilled = false;
    #endregion

    #region UNITY_CALLBACKS
    void OnEnable()
    {
		unlockPanel.gameObject.SetActive(false);
		noDiamondPanel.gameObject.SetActive(false);
		if(!isDataFilled)
		{
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
        float scrollContentHeight = (gridLayout.transform.childCount * gridLayout.cellSize.y) + ((gridLayout.transform.childCount - 1) * gridLayout.spacing.y);
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
        for (int i = 1; i <= noOfItems; i++)
		{
			InitializeNewItem("Theme_" + (i), i);
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
		newItem.GetComponent<Button>().onClick.AddListener(() => ThemeSelected(index));

		//Set theme button
		Text catText =  newItem.transform.Find("Name").transform.GetComponent<Text>();
		if(catText != null)
		{
			catText.text = Reader.Instance.ThemeDataList[index-1].category;
		}

		Transform lockPanel = newItem.transform.Find("LockPanel") ;
		int puzzleCount = Reader.Instance.ThemeDataList[index-1].puzzleCount;
		Text detailText =  newItem.transform.Find("Details").transform.GetComponent<Text>();

		if(index > 5)
		{
			string keyForThemeLock = Utility.KeyForThemeLock(index);
			bool isThemeLocked = PlayerPrefs.GetBool(keyForThemeLock, true);
			if(isThemeLocked)
			{
				if(lockPanel != null)
				{
					lockPanel.gameObject.SetActive(true);
				}
				if(detailText != null)
				{
					detailText.text = puzzleCount + " Diamonds needed to unlock";
				}
			}
			else
			{
				if(lockPanel != null)
				{
					lockPanel.gameObject.SetActive(false);
				}
				if(detailText != null)
				{
					detailText.text = puzzleCount + " Puzzles";
				}
			}

		}
		else
		{
			if(detailText != null)
			{
				detailText.text = puzzleCount + " Puzzles";
				
			}
			if(lockPanel != null)
				lockPanel.gameObject.SetActive(false);

		}

		Image iconImg =  newItem.transform.Find("Icon").transform.GetComponent<Image>();
		if(iconImg != null)
		{
			iconImg.sprite = Resources.Load <Sprite> ("Textures/Themes/" + Reader.Instance.ThemeDataList[index-1].iconImg) as Sprite;
		}


		Image diamondIcon =  newItem.transform.Find("Diamond").transform.GetComponent<Image>();
		Text diamondCount =  newItem.transform.Find("DCStatus").transform.GetComponent<Text>();

		string diamondKey = Utility.KeyForDiamondsOfTheme(index);
		int themeDiamondCnt = PlayerPrefs.GetInt(diamondKey, 0);

		if(themeDiamondCnt != 0)
		{
			if(diamondIcon != null)
			{
				diamondIcon.gameObject.SetActive(true);
			}
			if(diamondCount != null)
			{
				diamondCount.text = "" + themeDiamondCnt;
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

	int themeToUnlock,  priceToUnlock;
	void ThemeSelected(int themeID)
	{
//		Debug.Log("themeID = " + themeID);
		if(unlockPanel.gameObject.activeSelf)
		{
			unlockPanel.gameObject.SetActive(false);
			return;
		}
		if(noDiamondPanel.gameObject.activeSelf)
		{
			noDiamondPanel.gameObject.SetActive(false);
			return;
		}

		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayThemeSound();
		}
		if(themeID > 5)
		{
			//Locked themes
			string keyForThemeLock = Utility.KeyForThemeLock(themeID);
			bool isThemeLocked = PlayerPrefs.GetBool(keyForThemeLock, true);
			if(isThemeLocked)
			{
				priceToUnlock = Reader.Instance.ThemeDataList[themeID-1].puzzleCount;
				if(MainDriver.Instance.currentDiamondCnt >= priceToUnlock)
				{
					//Show confirmation alert to unlock theme
					themeToUnlock = themeID;
					unlockText.text = "Would you like to unlock theme for " +priceToUnlock + " diamonds";
					unlockPanel.gameObject.SetActive(true);
				}
				else
				{
					//Show insufficient diamond alert
					requiredText.text = "Need " +(priceToUnlock - MainDriver.Instance.currentDiamondCnt)
												+ " more Diamonds to unlock this theme";
					noDiamondPanel.gameObject.SetActive(true);
				}
			}
			else
			{
				//Theme is unlocked 
				GameData.mainThemeIndex = themeID;
				GameEventManager.TriggerThemeSelection();
			}
		}
		else
		{
			//Unlocked themes
			GameData.mainThemeIndex = themeID;
			GameEventManager.TriggerThemeSelection();
		}

	}

	public void ScrollTouched()
	{
		if(unlockPanel.gameObject.activeSelf)
		{
			unlockPanel.gameObject.SetActive(false);
			return;
		}
		if(noDiamondPanel.gameObject.activeSelf)
		{
			noDiamondPanel.gameObject.SetActive(false);
			return;
		}
	}

	public void UnlockYes()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		unlockPanel.gameObject.SetActive(false);
		GameObject unlockItem = gridLayout.transform.GetChild(themeToUnlock-1).gameObject;
		if(unlockItem != null)
		{
			Transform lockPanel = unlockItem.transform.Find("LockPanel");
			if(lockPanel != null)
			{
				lockPanel.gameObject.SetActive(false);
			}
			string keyForThemeLock = Utility.KeyForThemeLock(themeToUnlock);
			PlayerPrefs.SetBool(keyForThemeLock, false);
			MainDriver.Instance.unlockedThemes.Add(themeToUnlock);

			MainDriver.Instance.currentDiamondCnt -= priceToUnlock;
			PlayerPrefs.SetInt(Constants.KEY_CURRENT_DIAMONDS, MainDriver.Instance.currentDiamondCnt);

			if(!MainDriver.Instance.questChecker.QUEST_UNLOCK_ALL)
			{
				MainDriver.Instance.unlockedThemeCnt++;
				PlayerPrefs.SetInt(Constants.UNLOCKED_THEMES_COUNT, MainDriver.Instance.unlockedThemeCnt);
				//16 themes are locked initially, 12 added in new version, so total 28
				if(MainDriver.Instance.unlockedThemeCnt == 28)
				{
					PlayerPrefs.SetBool(Constants.QUEST_UNLOCK_ALL, true);
					MainDriver.Instance.questChecker.UpdateQuestStatus();
					MainDriver.Instance.PostAchievement(Constants.QUEST_UNLOCK_ALL_ID);
					MainDriver.Instance.AwardDiamonds(Constants.QUEST_AWARD);
				}

			}

			PlayerPrefs.Flush();
				
		}

		//Unlock theme here 
	}

	public void UnlockNO()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		unlockPanel.gameObject.SetActive(false);
	}

	public void LessGotIt()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		noDiamondPanel.gameObject.SetActive(false);
	}
}
