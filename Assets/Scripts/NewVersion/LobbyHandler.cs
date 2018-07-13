using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using LitJson;
using Heyzap;
using UnityEngine.Analytics;


//class WebViewCallbackTest : Kogarasi.WebView.IWebViewCallback
//{
//	public void onLoadStart( string url )
//	{
//		Debug.Log( "Web view call onLoadStart : " + url );
//	}
//	public void onLoadFinish( string url )
//	{
//		Debug.Log( "Web view call onLoadFinish : " + url );
//	}
//	public void onLoadFail( string url )
//	{
//		Debug.Log( "Web view call onLoadFail : " + url );
//	}
//}

public class LobbyHandler : MonoBehaviour {

	public RectTransform mainMenuPanel, challengePanel, themesPanel, kidsPanel, settingPanel, tutorialPanel;
	public Button newGameBtn;
	public Button themeBtn, chalngeBtn, fbBtn, kidsBtn;
	public RectTransform timeChallangePanel;
	public RectTransform webViewPanel;

	public static RectTransform currentPanel;

	bool isWebViewVisible;
//	WebViewCallbackTest m_callback;
	WebViewBehavior webview;

	void OnEnable()
	{

	}

	// Use this for initialization
	void Start () 
	{
//		Debug.Log("GameData.gameType = " + GameData.gameType);

		newGameBtn.gameObject.SetActive(false);
		webViewPanel.gameObject.SetActive(false);
		switch(GameData.gameType)
		{
			case EGameType.None:
				SetNormalStartOfGame();
				break;

			case EGameType.ThemeSelection:
				Themes();
				GameEventManager.TriggerThemeSelection();
				break;

			case EGameType.QuickGame:
				SetNormalStartOfGame();
				break;

			case EGameType.TimeChallenge:
				SetTimeChallenge();
				break;

			case EGameType.DailyChallenge:
				SetDailyChallenge();
				break;

			case EGameType.KidsGame:
				KidsMode();
				break;
		};



//		m_callback = new WebViewCallbackTest();
//		webview = GetComponent<WebViewBehavior>();
//		if( webview != null )
//		{
//			webview.LoadURL("http://1touchstudios.com/games");
//			webview.SetMargins(0,Mathf.FloorToInt(Screen.height*0.255f), 0,  0);
//			//webview.SetVisibility( true );
//			webview.SetVisibility(false);
//			webview.setCallback( m_callback );
//		}

		//Api response reader
//		string url = "https://ue5mm7vkwa.execute-api.us-west-2.amazonaws.com/test/iosgames";
//		WWW www = new WWW(url);
//		StartCoroutine(WaitForRequest(www));
//		
//		string latestGameUrl = "https://s3-us-west-2.amazonaws.com/imagebucketvoayger/Fightforantman.png";
//		WWW wwwLatestGame = new WWW(latestGameUrl);
//		StartCoroutine(WaitForLatestGame(wwwLatestGame));

//		AdHandler.RequestVideo();
		GameData.isTimeChallenge = false;
	}

	void SetNormalStartOfGame()
	{
		GameData.gameMode = EGameMode.FullMode;
		currentPanel = mainMenuPanel;
		mainMenuPanel.gameObject.SetActive(true);
		tutorialPanel.gameObject.SetActive(false);
		challengePanel.gameObject.SetActive(false);
		themesPanel.gameObject.SetActive(false);
		kidsPanel.gameObject.SetActive(false);
		settingPanel.gameObject.SetActive(false);
		newGameBtn.gameObject.SetActive(false);

	}

	void SetTimeChallenge()
	{
		mainMenuPanel.gameObject.SetActive(false);
		tutorialPanel.gameObject.SetActive(false);
		challengePanel.gameObject.SetActive(false);
		themesPanel.gameObject.SetActive(false);
		kidsPanel.gameObject.SetActive(false);
		settingPanel.gameObject.SetActive(false);
		newGameBtn.gameObject.SetActive(false);

		timeChallangePanel.gameObject.SetActive(true);

	}

	void SetDailyChallenge()
	{
		mainMenuPanel.gameObject.SetActive(false);
		tutorialPanel.gameObject.SetActive(false);
		challengePanel.gameObject.SetActive(true);
		themesPanel.gameObject.SetActive(false);
		kidsPanel.gameObject.SetActive(false);
		settingPanel.gameObject.SetActive(false);
		newGameBtn.gameObject.SetActive(false);
		
		timeChallangePanel.gameObject.SetActive(false);
		
	}


//	IEnumerator WaitForRequest(WWW www)
//	{
//		yield return www;
//		// check for errors
//		if (www.error == null)
//		{
//			RootObject deserializedClass = JsonMapper.ToObject<RootObject>(www.data);
////			Debug.Log("List count = " + deserializedClass.Items[1].url);
//		}
//		else 
//		{
//			Debug.Log("WWW Error: "+ www.error);
//		}    
//	}
//
//	IEnumerator WaitForLatestGame(WWW www)
//	{
//		yield return www;
//		if (www.error == null)
//		{
//
//			newGameBtn.image.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height),
//			                                        new Vector2(0.5f, 0.5f));
//			newGameBtn.gameObject.SetActive(true);
////			renderer.material.mainTexture = www.texture;
//		}
//		else 
//		{
//			Debug.Log("WWW Error: "+ www.error);
//		}    
//	}


	// Update is called once per frame
//	void Update()
//	{
//		if(Input.GetKeyDown(KeyCode.Escape)) 
//		{
//
//		}
//	}


	#region Main Lobby Actions
	public void Themes()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		currentPanel = themesPanel;
		themesPanel.gameObject.SetActive(true);
		mainMenuPanel.gameObject.SetActive(false);
	}

	public void QuickGame()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		GameData.gameMode = EGameMode.FullMode;
		GameData.gameType = EGameType.QuickGame;

		int index = UnityEngine.Random.Range(0, MainDriver.Instance.unlockedThemes.Count);
		GameData.mainThemeIndex = MainDriver.Instance.unlockedThemes[index];

		int puzzzleId = UnityEngine.Random.Range(0, Reader.Instance.SubLevelDetails[GameData.mainThemeIndex-1].Length);
		GameData.puzzleID = puzzzleId + 1;

		GameData.gamePuzzleName = Reader.Instance.SubLevelDetails[GameData.mainThemeIndex-1][puzzzleId];
		GameData.puzzleFileName = "P_"+ GameData.mainThemeIndex + "_"+ GameData.puzzleID;
//		Debug.Log("subLevelDictionary = " + GameData.puzzleFileName);

		Application.LoadLevelAsync("GamePlay");
	}


	public void Challenges()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		challengePanel.gameObject.SetActive(true);
		mainMenuPanel.gameObject.SetActive(false);
	}


	public void KidsMode()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		kidsPanel.gameObject.SetActive(true);
		LobbyHandler.currentPanel = mainMenuPanel;
		mainMenuPanel.gameObject.SetActive(false);
	}


	public void MoreGames()
	{
//		Debug.Log("MoreGames");

//		if( !isWebViewVisible && webview != null )
//		{
//			if(GameData.isMusicON)
//			{
//				MainDriver.Instance.PlayButtonSound();
//			}
//			webview.SetVisibility(true);
//			webViewPanel.gameObject.SetActive(true);
//			isWebViewVisible = true;
//		}

		Application.OpenURL("http://onelink.to/wsnplay");
	}

	public void ViewLatestGame()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
	}

	public void Settings()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		settingPanel.gameObject.SetActive(true);
		mainMenuPanel.gameObject.SetActive(false);
	}

	public void LeaderBoard()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		MainDriver.Instance.ShowLeaderBoard();
	}

	public void Achievements()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		MainDriver.Instance.ShowAcheivementsUI();

	}

	public void RateMe()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		UniRate.Instance.RateIfNetworkAvailable();
	}

	
	public void DisableWebView()
	{
		if(webview != null)
		{
			webview.SetVisibility(false);
		}
		webViewPanel.gameObject.SetActive(false);
		isWebViewVisible = false;
	}


	#endregion

//	#region Tutorial Actions
//	public void StartTutorial()
//	{
//		if(GameData.gameMode == EGameMode.TutorialMode)
//		{
//			GameData.puzzleFileName = "Tutorial";
//		}
//		Application.LoadLevelAsync("GamePlay");
//	}
//
//	public void StartMainGame()
//	{
//		SetNormalStartOfGame();
//	}
//	#endregion

}
