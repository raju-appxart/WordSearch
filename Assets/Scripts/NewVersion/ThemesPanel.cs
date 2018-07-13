using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Heyzap;
using System;

public class ThemesPanel : MonoBehaviour {

	public RectTransform mainMenuPanel, themePanel, themesTab, achivementTab, diamondTab;
	public RectTransform mainCatPanel, subCatPanel, settingPanel;
	public Image themeBtnBg, achvBtnBg, diamongBtnBg;
	public Image themeIcon;
	public Text themeTitle;
	public Text diamondCount, achievmentStatus;
	public RectTransform unlockPanel, noDiamondPanel;
	public Image acivementIcon;
	public Sprite playIcon, achivIcon;
	public Button rewardBtn;
	public Text rewardText, timerText;
	public RectTransform rewardPanel;

	public static event Action HasWatchedVideo;

	Color selectedColor, unselectColor;

	void OnEnable()
	{
		GameEventManager.ThemeSetected += SelectTheme;
		GameEventManager.PuzzleSelected += SelectedPuzzle;
		GameEventManager.StartVideoTimer += HandleStartVideoTimer;
		GameEventManager.VideoTimerFinished += HandleVideoTimerFinished;
		ThemesPanel.HasWatchedVideo += AddMsg;
		if(Social.localUser.authenticated)
		{
			acivementIcon.sprite = achivIcon;
		}
		else
		{
			acivementIcon.sprite = playIcon;
		}
		Invoke ("SetCount", 0.25f);
	}

	void SetCount()
	{
		achievmentStatus.text =  DynamicQuestScrollView.questDiamondCnt +"/150";
		diamondCount.text = "" + MainDriver.Instance.currentDiamondCnt;
	}
	
	void OnDisable()
	{
		GameEventManager.ThemeSetected -= SelectTheme;
		GameEventManager.PuzzleSelected -= SelectedPuzzle;
		GameEventManager.StartVideoTimer -= HandleStartVideoTimer;
		GameEventManager.VideoTimerFinished -= HandleVideoTimerFinished;
		ThemesPanel.HasWatchedVideo -= AddMsg;
	}

	void HandleStartVideoTimer ()
	{
		timerText.gameObject.SetActive(true);
		rewardBtn.gameObject.SetActive(false);
		rewardText.text = "VIDEO WILL BE AVAILABLE SOON";
		rewardPanel.gameObject.SetActive(true);
	}

	void AddMsg()
	{
		rewardPanel.gameObject.SetActive(true);
//		GameObject stars = Instantiate(starEffect);
//		Destroy(stars, 1);
//		videoBtn.interactable = false;
//		Main.Instance.starsCnt += Constants.REWARD_STARS;
//		PlayerPrefs.SetInt(Constants.STAR_TOTAL, Main.Instance.starsCnt);
//		Invoke("UpdateText", 0.25f);
	}

	void HandleVideoTimerFinished ()
	{
		timerText.gameObject.SetActive(false);
		if(HZIncentivizedAd.IsAvailable())
		{
			rewardBtn.gameObject.SetActive(true);
			rewardText.text = "WATCH VIDEO GET 3 DIAMONDS";
		}
		else if(!HZIncentivizedAd.IsAvailable())
		{
			rewardBtn.gameObject.SetActive(false);
			timerText.gameObject.SetActive(false);
			rewardText.gameObject.SetActive(false);
		}
	}

	#region Reward Video Delegate
	HZIncentivizedAd.AdDisplayListener videoAdListener = delegate(string adState, string adTag)
	{
		if ( adState.Equals("incentivized_result_complete") ) 
		{
			// The user has watched the entire video and should be given a reward.
			HasWatchedVideo();
			HZIncentivizedAd.Fetch();
			GameEventManager.TriggerStartVideoTimer();
		}
		if ( adState.Equals("incentivized_result_incomplete") ) 
		{
			// The user did not watch the entire video and should not be given a   reward.
			HZIncentivizedAd.Fetch();
		}
	};
	#endregion
	

	// Use this for initialization
	void Start () 
	{
		selectedColor = new Color(1,205.0f/255,106.0f/255,1);
		unselectColor = new Color(1,1,1,0);
		themeBtnBg.GetComponent<Outline>().effectColor = selectedColor;
		achvBtnBg.GetComponent<Outline>().effectColor = unselectColor;
		diamongBtnBg.GetComponent<Outline>().effectColor = unselectColor;

		themesTab.gameObject.SetActive(true);
		achivementTab.gameObject.SetActive(false);
		diamondTab.gameObject.SetActive(false);
		settingPanel.gameObject.SetActive(false);
		rewardPanel.gameObject.SetActive(false);

		if(GameData.gameType == EGameType.ThemeSelection)
		{
			mainCatPanel.gameObject.SetActive(false);
			subCatPanel.gameObject.SetActive(true);
		}
		else
		{
			if(!mainCatPanel.gameObject.activeSelf)
			{
				mainCatPanel.gameObject.SetActive(true);
				subCatPanel.gameObject.SetActive(false);
			}

		}
		HZIncentivizedAd.SetDisplayListener(videoAdListener);
		HZIncentivizedAd.Fetch();
	}


	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape) && !rewardPanel.gameObject.activeSelf) 
		{
			BackToMenu();
		}

		timerText.text = MainDriver.Instance.timerText;
	}

	#region Theme Panel Actions
	public void BackToMenu()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
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
		if(!themesTab.gameObject.activeSelf)
		{
			//Directly Back to home page
			mainMenuPanel.gameObject.SetActive(true);
			themePanel.gameObject.SetActive(false);
			LobbyHandler.currentPanel = mainMenuPanel;
		}
		else
		{
			if(mainCatPanel.gameObject.activeSelf)
			{
				//Back to home page
				mainMenuPanel.gameObject.SetActive(true);
				themePanel.gameObject.SetActive(false);
				LobbyHandler.currentPanel = mainMenuPanel;
			}
			else
			{
				//Back to main themes
				mainCatPanel.gameObject.SetActive(true);
				subCatPanel.gameObject.SetActive(false);
			}
		}

	}

	public void ThemesTab()
	{

		if(!themesTab.gameObject.activeSelf)
		{
			if(GameData.isMusicON)
			{
				MainDriver.Instance.PlayButtonSound();
			}
			themesTab.gameObject.SetActive(true);
			achivementTab.gameObject.SetActive(false);
			diamondTab.gameObject.SetActive(false);

			themeBtnBg.GetComponent<Outline>().effectColor = selectedColor;
			achvBtnBg.GetComponent<Outline>().effectColor = unselectColor;
			diamongBtnBg.GetComponent<Outline>().effectColor = unselectColor;
		}
	}

	public void AchievementTab()
	{
		if(!achivementTab.gameObject.activeSelf)
		{
			if(GameData.isMusicON)
			{
				MainDriver.Instance.PlayButtonSound();
			}
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


			themesTab.gameObject.SetActive(false);
			achivementTab.gameObject.SetActive(true);
			diamondTab.gameObject.SetActive(false);
			themeBtnBg.GetComponent<Outline>().effectColor = unselectColor;
			achvBtnBg.GetComponent<Outline>().effectColor = selectedColor;
			diamongBtnBg.GetComponent<Outline>().effectColor = unselectColor;

			achievmentStatus.text =  DynamicQuestScrollView.questDiamondCnt +"/150";
		}
	}


	public void DiamondTab()
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
		if(!diamondTab.gameObject.activeSelf)
		{
			if(GameData.isMusicON)
			{
				MainDriver.Instance.PlayButtonSound();
			}
			int currentDiamonds = MainDriver.Instance.currentDiamondCnt;
			diamondCount.text = "" + currentDiamonds;
			themesTab.gameObject.SetActive(false);
			achivementTab.gameObject.SetActive(false);
			diamondTab.gameObject.SetActive(true);
			themeBtnBg.GetComponent<Outline>().effectColor = unselectColor;
			achvBtnBg.GetComponent<Outline>().effectColor = unselectColor;
			diamongBtnBg.GetComponent<Outline>().effectColor = selectedColor;

			if(HZIncentivizedAd.IsAvailable() && MainDriver.Instance.shallShowVideo)
			{
				rewardBtn.gameObject.SetActive(true);
				rewardText.text = "WATCH VIDEO GET 3 DIAMONDS";
			}
			else if(HZIncentivizedAd.IsAvailable() && !MainDriver.Instance.shallShowVideo)
			{
				timerText.gameObject.SetActive(true);
				rewardBtn.gameObject.SetActive(false);
				rewardText.text = "VIDEO WILL BE AVAILABLE SOON";
			}
			else if(!HZIncentivizedAd.IsAvailable())
			{
				rewardBtn.gameObject.SetActive(false);
				timerText.gameObject.SetActive(false);
				rewardText.gameObject.SetActive(false);
			}

		}
	}

	public void GoogleAchievements()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		if(Social.localUser.authenticated)
		{
			MainDriver.Instance.ShowAcheivementsUI();
		}
		else
		{
			#if UNITY_ANDROID
			Social.localUser.Authenticate((bool success) => {
				// handle success or failure
				if(success)
				{
					//Post best score of user to leaderboard
					
					MainDriver.Instance.questChecker.PostAchievements();
					acivementIcon.sprite = achivIcon;
//					Debug.Log("LOG_IN  Play Service Google");
				}
				else
				{
//					Debug.Log("LOG_IN FALED Play Service Google");
					acivementIcon.sprite = playIcon;
					
				}

			});
			#endif
		}
	}

	public void GetReward()
	{
		MainDriver.Instance.AwardDiamonds(3);
		diamondCount.text = "" + MainDriver.Instance.currentDiamondCnt;
//		AdHandler.RequestVideo();
		rewardPanel.gameObject.SetActive(false);
	}

	public void Settings()
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
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		settingPanel.gameObject.SetActive(true);
		themePanel.gameObject.SetActive(false);
	}

	public void WatchVideo()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}

		if(MainDriver.Instance.shallShowVideo)
		{
			if (HZIncentivizedAd.IsAvailable()) 
			{
				HZIncentivizedAd.Show();
			}
		}

	}

	void SelectTheme()
	{
//		Debug.Log("Slecting Theme");
		themeIcon.sprite = Resources.Load <Sprite> ("Textures/Themes/" + Reader.Instance.ThemeDataList[GameData.mainThemeIndex-1].iconImg) as Sprite;
		themeTitle.text = Reader.Instance.ThemeDataList[GameData.mainThemeIndex-1].category;
//		Debug.Log("themeTitle.text = " + themeTitle.text + Reader.Instance.ThemeDataList[GameData.mainThemeIndex-1].puzzleCount);
		subCatPanel.transform.GetComponent<DynamicPuzzleScrollView>().noOfItems = Reader.Instance.ThemeDataList[GameData.mainThemeIndex-1].puzzleCount;
//		Debug.Log("Puzzle cnt = " + subCatPanel.transform.GetComponent<DynamicPuzzleScrollView>().noOfItems);
		subCatPanel.transform.gameObject.SetActive(true);
		mainCatPanel.transform.gameObject.SetActive(false);
	}



	void SelectedPuzzle ()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayThemeSound();
		}
		GameData.gameMode = EGameMode.FullMode;
		GameData.gameType = EGameType.ThemeSelection;
		GameData.puzzleFileName = "P_"+ GameData.mainThemeIndex + "_"+ GameData.puzzleID;
		GameData.gamePuzzleName = Reader.Instance.SubLevelDetails[GameData.mainThemeIndex-1][GameData.puzzleID-1];
		Application.LoadLevelAsync("GamePlay");
	}
	#endregion
}
