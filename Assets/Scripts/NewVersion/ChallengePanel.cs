using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Facebook.Unity;

public class ChallengePanel : MonoBehaviour {

	public RectTransform mainMenuPanel, challengePanel, timeChallengePanel;
	public Text fbBtnText;

	void OnEnable()
	{
		GameEventManager.FacebookLoggedIn += FBLoginCompleted;
		if(!FB.IsLoggedIn)
		{
			fbBtnText.text = "FACEBOOK";
		}
		else
		{
			fbBtnText.text = "INVITE";
		}


	}
	
	void OnDisable()
	{
		GameEventManager.FacebookLoggedIn -= FBLoginCompleted;
	}


	// Use this for initialization
	void Start () 
	{
		timeChallengePanel.gameObject.SetActive(false);
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape)) 
		{
			BackToMenu();
		}
	}


	#region Challenge Panel Actions

	public void Facebook()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		if(fbBtnText.text == "INVITE")
		{
			MainDriver.Instance.FacbookInviteFrnds();
		}
		else
		{
			MainDriver.Instance.FacebookLogin();
		}

	}


	public void TimeChallenge()
	{
		if(!timeChallengePanel.gameObject.activeSelf)
		{
			if(GameData.isMusicON)
			{
				MainDriver.Instance.PlayButtonSound();
			}
			timeChallengePanel.gameObject.SetActive(true);
			challengePanel.gameObject.SetActive(false);

		}
	}

	public void BackToMenu()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		LobbyHandler.currentPanel = mainMenuPanel;
		mainMenuPanel.gameObject.SetActive(true);
		challengePanel.gameObject.SetActive(false);
	}

	public void DailyChallenge()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		GameData.gameMode = EGameMode.FullMode;
		GameData.gameType = EGameType.DailyChallenge;

		int index = UnityEngine.Random.Range(0, MainDriver.Instance.unlockedThemes.Count);
		GameData.mainThemeIndex = MainDriver.Instance.unlockedThemes[index];
		
		int puzzzleId = UnityEngine.Random.Range(0, Reader.Instance.SubLevelDetails[GameData.mainThemeIndex-1].Length);
		GameData.puzzleID = puzzzleId + 1;

		GameData.gamePuzzleName = Reader.Instance.SubLevelDetails[GameData.mainThemeIndex-1][puzzzleId];
		GameData.puzzleFileName = "P_"+ GameData.mainThemeIndex + "_"+ GameData.puzzleID;
//		Debug.Log("subLevelDictionary = " + GameData.puzzleFileName);
		
		Application.LoadLevelAsync("GamePlay");
	}

	#endregion


	void FBLoginCompleted()
	{
		fbBtnText.text = "INVITE";
	}

}
