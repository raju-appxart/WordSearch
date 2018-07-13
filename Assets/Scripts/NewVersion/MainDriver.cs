using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayerPrefs = PreviewLabs.PlayerPrefs;
using System;
using System.IO;
#if UNITY_ANDROID
using Prime31;
#if !AMAZON
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif
#endif
using UnityEngine.SocialPlatforms.GameCenter;
using System.Runtime.InteropServices;
using Facebook.Unity;
using System.Linq;
using Heyzap;

public class MainDriver : MonoBehaviour {


	public AudioSource audioSource;
	public AudioClip buttonSound, packSound, wordSelectionSound, correctSound, levelWonSound ;

	Texture2D MyImage;

	[HideInInspector]
	public int currentDiamondCnt, lifeTimeDiamondCnt, gameCount, puzzleSolved;
	public QuestCheck questChecker;

	[HideInInspector]
	public int diamondPuzzleCnt, puzzleIn115Cnt, puzzleIn130Cnt, puzzleIn145Cnt, ccPuzzle245Cnt, ccPuzzle300Cnt;
	[HideInInspector]
	public int dailyChallangeCnt, unlockedThemeCnt;

	[HideInInspector]
	public List<int> unlockedThemes;

	[HideInInspector]
	public bool shallShowVideo;
	[HideInInspector]
	public string timerText;
	int time = 0;

	const float splashHoldTime = 2.0f;
	const int videoWait = 180;


	//Android Notifications
	private int _fourHrNotificationId;
	private int _eightHrNotificationId;
	private int _oneDayNotificationId;
	private int _threeDayNotificationId;
	private int _sevenDayNotificationId;
	private int _fourteenDayNotificationId;
	private int _thirtyDayNotificationId;
	
	long fourHr = 14400;
	long eightHr = 28800;
	long oneDay = 86400;
	long threeDay = 259200;
	long sevenDay = 604800;
	long fourteenDay = 1209600;
	long thirtyDay = 2592000;
	
	string msg_h4 = "Beat your fastest time and solve the quests. Play Word Search!";
	string msg_h8 = "Take a break. Play Word Search and solve puzzles!";
	string msg_d1 = "Can you find all the words? There are more Word Search puzzles to be solved!";
	string msg_d3 = "Become a word search expert! Take on new puzzles and get the fastest time.";
	string msg_d7 = "Speed and skill is what it takes to find all the words! Play now!";
	string msg_d14 = "How fast can you find all the words? Play new word search puzzles and find out!";
	string msg_d30 = "We've added new and challenging puzzles! Play Word Search now!";

	private static MainDriver instance = null;
	public static MainDriver Instance 
	{
		get { return instance; }
	}
	
	void Awake() 
	{
		if (instance != null && instance != this) 
		{
			Destroy(this.gameObject);
			return;
		} 
		else 
		{
			instance = this;
		}
		
	}

	void OnEnable()
	{
		GameEventManager.StartVideoTimer += HandleStartVideoTimer;
		GameEventManager.VideoTimerFinished += HandleVideoTimerFinished;
	}



	void OnDisable()
	{
		GameEventManager.StartVideoTimer -= HandleStartVideoTimer;
		GameEventManager.VideoTimerFinished -= HandleVideoTimerFinished;
	}


	void HandleVideoTimerFinished ()
	{
		Instance.shallShowVideo = true;
	}
	
	void HandleStartVideoTimer ()
	{
		Instance.shallShowVideo = false;
		Instance.time = 0;
		Instance.timerText = String.Format("{0:D2}:{1:D2}", videoWait/60, videoWait%60);
		Invoke("CurrentTime",1.0f);
	}

	private void CurrentTime()
	{
		Instance.time++;
		Instance.timerText = String.Format("{0:D2}:{1:D2}",(videoWait-time)/60, (videoWait-time)%60);
		Invoke("CurrentTime", 1);
		if(Instance.time >= videoWait)
		{
			CancelInvoke("CurrentTime");
			GameEventManager.TriggerVideoTimerFinished();
		}

	}

	// Use this for initialization
	void Start () 
	{
		PlayerPrefs.EnableEncryption(true);
		
		GameData.isMusicON = PlayerPrefs.GetBool(Constants.KEY_SOUND, true);
		GameData.isNotifcationON = PlayerPrefs.GetBool(Constants.KEY_NOTIFICATIONS, true);
		MainDriver.Instance.currentDiamondCnt = PlayerPrefs.GetInt(Constants.KEY_CURRENT_DIAMONDS, 0);
		MainDriver.Instance.lifeTimeDiamondCnt = PlayerPrefs.GetInt(Constants.KEY_LIFETIME_DIAMONDS, 0);
		MainDriver.Instance.gameCount = PlayerPrefs.GetInt(Constants.KEY_GAME_COUNT, 0);
		MainDriver.Instance.puzzleSolved = PlayerPrefs.GetInt (Constants.KEY_PUZZLE_SOLVED, 0);
		MainDriver.Instance.unlockedThemeCnt = PlayerPrefs.GetInt(Constants.UNLOCKED_THEMES_COUNT, 0);
		MainDriver.Instance.dailyChallangeCnt = PlayerPrefs.GetInt(Constants.DAILY_CHALLENGE_COUNT, 0);
		MainDriver.Instance.diamondPuzzleCnt = PlayerPrefs.GetInt(Constants.PUZZLE_WITH_DIAMONDS, 0);
		MainDriver.Instance.puzzleIn115Cnt = PlayerPrefs.GetInt(Constants.PUZZLE_UNDER_115, 0);
		MainDriver.Instance.puzzleIn130Cnt = PlayerPrefs.GetInt(Constants.PUZZLE_UNDER_130, 0);
		MainDriver.Instance.puzzleIn145Cnt = PlayerPrefs.GetInt(Constants.PUZZLE_UNDER_145, 0);
		MainDriver.Instance.ccPuzzle245Cnt = PlayerPrefs.GetInt(Constants.PUZZLE_CONSECUTIVE_245, 0);
		MainDriver.Instance.ccPuzzle300Cnt = PlayerPrefs.GetInt(Constants.PUZZLE_CONSECUTIVE_300, 0);
		
		audioSource  = transform.GetComponent<AudioSource>();
		questChecker  = transform.GetComponent<QuestCheck>();
		Instance.shallShowVideo = true;
		
		unlockedThemes = new List<int>();
		for(int themeId = 1; themeId <= Constants.TOTAL_THEMES; themeId++)
		{
			if(themeId <= Constants.FREE_THEMES)
			{
				unlockedThemes.Add(themeId);
			}
			else
			{
				string keyForThemeLock = Utility.KeyForThemeLock(themeId);
				bool isThemeLocked = PlayerPrefs.GetBool(keyForThemeLock, true);
				if(!isThemeLocked)
				{
					unlockedThemes.Add(themeId);
				}
			}
			
			
		}
		
		
		#if UNITY_ANDROID
		#if !AMAZON
		PlayGamesPlatform.Activate();
		#endif
		EtceteraAndroid.cancelNotification( Constants.H4 );
		EtceteraAndroid.cancelNotification( Constants.H8 );
		EtceteraAndroid.cancelNotification( Constants.D1 );
		EtceteraAndroid.cancelNotification( Constants.D3 );
		EtceteraAndroid.cancelNotification( Constants.D7 );
		EtceteraAndroid.cancelNotification( Constants.D14 );
		EtceteraAndroid.cancelNotification( Constants.D30 );
		EtceteraAndroid.cancelAllNotifications();
		#elif UNITY_IOS 
		
		GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
		UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert|UnityEngine.iOS.NotificationType.Badge);
		
		UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
		UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications ();
		
		// clear badge number
		UnityEngine.iOS.LocalNotification temp = new UnityEngine.iOS.LocalNotification();
		temp.fireDate = DateTime.Now;
		temp.applicationIconBadgeNumber = -1;
		temp.alertBody = "";
		UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(temp);
		#endif
		
		//Read all puzzle data
		Reader dataReader =  Reader.Instance;
		
		//Facebook Initialize
		FB.Init(this.OnInitComplete, this.OnHideUnity);
		
		
		HeyzapAds.Start("790edd987b2facf1eb7117dfc97d354b", HeyzapAds.FLAG_NO_OPTIONS);
		
		#if AMAZON
		InitializeGameCircle();
		#else 
		SignIn();
		#endif
		Invoke("Play", splashHoldTime);
		DontDestroyOnLoad(gameObject);
	}
	
	//	void Update()
	//	{
	//		Instance.timerText = String.Format("{0:D2}:{1:D2}", videoWait/60, videoWait%60);
	//	}
	
	void Play()
	{
		Tapdaq.ShowInterstitial();
		Application.LoadLevelAsync(1);
	}
	
	
	void OnApplicationPause(bool pauseStatus) 
	{
		if(pauseStatus)
		{
			if(GameData.isNotifcationON)
			{
				#if UNITY_ANDROID
				_fourHrNotificationId = EtceteraAndroid.scheduleNotification( fourHr, "WordSearch", msg_h4, "WordSearch", "four-hour-note", "small_icon", "large_icon", Constants.H4 );
//				_eightHrNotificationId = EtceteraAndroid.scheduleNotification( eightHr, "WordSearch", msg_h8, "WordSearch", "eight-hour-note", Constants.H8 );
				_oneDayNotificationId = EtceteraAndroid.scheduleNotification( oneDay, "WordSearch", msg_d1, "WordSearch", "one-day-note", "small_icon", "large_icon", Constants.D1 );
				_threeDayNotificationId = EtceteraAndroid.scheduleNotification( threeDay, "WordSearch", msg_d3, "WordSearch", "three-day-note", "small_icon", "large_icon", Constants.D3 );
				_sevenDayNotificationId = EtceteraAndroid.scheduleNotification( sevenDay, "WordSearch", msg_d7, "WordSearch", "seven-day-note", "small_icon", "large_icon", Constants.D7 );
				_fourteenDayNotificationId = EtceteraAndroid.scheduleNotification( fourteenDay, "WordSearch", msg_d14, "WordSearch", "fourteen-day-note", "small_icon", "large_icon", Constants.D14 );
				_thirtyDayNotificationId = EtceteraAndroid.scheduleNotification( thirtyDay, "WordSearch", msg_d30, "WordSearch", "thirty-day-note", "small_icon", "large_icon",Constants.D30 );
				#elif UNITY_IOS 
				UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(msg_h4, 4));
				UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(msg_h8, 8));
				UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(msg_d1, 24));
				UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(msg_d3, 72));
				UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(msg_d7, 168));
				UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(msg_d14, 336));
				UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(msg_d30, 720));
				#endif
			}
		}
		else
		{
			#if UNITY_ANDROID
			EtceteraAndroid.cancelNotification( Constants.H4 );
//			EtceteraAndroid.cancelNotification( Constants.H8 );
			EtceteraAndroid.cancelNotification( Constants.D1 );
			EtceteraAndroid.cancelNotification( Constants.D3 );
			EtceteraAndroid.cancelNotification( Constants.D7 );
			EtceteraAndroid.cancelNotification( Constants.D14 );
			EtceteraAndroid.cancelNotification( Constants.D30 );
			EtceteraAndroid.cancelAllNotifications();
			#elif UNITY_IOS
			UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
			UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications ();
			#endif
		}
		PlayerPrefs.Flush();
	}
	
	void OnApplicationQuit()
	{
		if(GameData.isNotifcationON)
		{
			#if UNITY_ANDROID
			_fourHrNotificationId = EtceteraAndroid.scheduleNotification( fourHr, "WordSearch", msg_h4, "WordSearch", "four-hour-note", "small_icon", "large_icon", Constants.H4 );
//			_eightHrNotificationId = EtceteraAndroid.scheduleNotification( eightHr, "WordSearch", msg_h8, "WordSearch", "eight-hour-note", Constants.H8 );
			_oneDayNotificationId = EtceteraAndroid.scheduleNotification( oneDay, "WordSearch", msg_d1, "WordSearch", "one-day-note", "small_icon", "large_icon", Constants.D1 );
			_threeDayNotificationId = EtceteraAndroid.scheduleNotification( threeDay, "WordSearch", msg_d3, "WordSearch", "three-day-note", "small_icon", "large_icon", Constants.D3 );
			_sevenDayNotificationId = EtceteraAndroid.scheduleNotification( sevenDay, "WordSearch", msg_d7, "WordSearch", "seven-day-note", "small_icon", "large_icon", Constants.D7 );
			_fourteenDayNotificationId = EtceteraAndroid.scheduleNotification( fourteenDay, "WordSearch", msg_d14, "WordSearch", "fourteen-day-note", "small_icon", "large_icon", Constants.D14 );
			_thirtyDayNotificationId = EtceteraAndroid.scheduleNotification( thirtyDay, "WordSearch", msg_d30, "WordSearch", "thirty-day-note", "small_icon", "large_icon", Constants.D30 );
			#elif UNITY_IOS 
			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(msg_h4, 4));
			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(msg_h8, 8));
			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(msg_d1, 24));
			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(msg_d3, 72));
			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(msg_d7, 168));
			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(msg_d14, 336));
			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(getNotification(msg_d30, 720));
			#endif
		}
		PlayerPrefs.Flush();
	}
	
	void OnLevelWasLoaded(int level)
	{
		switch(level)
		{
		case 0:
			break;
			
		case 1:
			HZBannerShowOptions showOptions1 = new HZBannerShowOptions();
			showOptions1.Position = HZBannerShowOptions.POSITION_BOTTOM;
			HZBannerAd.ShowWithOptions(showOptions1);
			break;
			
		case 2:
			
			HZBannerShowOptions showOptions = new HZBannerShowOptions();
			showOptions.Position = HZBannerShowOptions.POSITION_BOTTOM;
			HZBannerAd.ShowWithOptions(showOptions);
			break;
			
		case 3:
			break;
		}
	}


	#region GameCircle plugin functions

	#if AMAZON
	/// <summary>
	/// Initializes the GameCircle plugin.
	/// </summary>
	void InitializeGameCircle() 
	{
	// Step the initialization progress forward.

	// Subscribe to the initialization events so the menu knows when GameCircle is ready (or errors out)
	SubscribeToGameCircleInitializationEvents();
	bool usesLeaderboards = true;
	bool usesAchievements = false;
	bool usesWhispersync = false;
	// Begin GameCircle initialization.
	AGSClient.Init(usesLeaderboards, usesAchievements, usesWhispersync);
	}

	/// <summary>
	/// Subscribes to GameCircle initialization events.
	/// </summary>
	private void SubscribeToGameCircleInitializationEvents() {   
	AGSClient.ServiceReadyEvent += Instance.ServiceReadyHandler;
	AGSClient.ServiceNotReadyEvent += Instance.ServiceNotReadyHandler;
	}

	/// <summary>
	/// Unsubscribes from GameCircle initialization events.
	/// </summary>
	private void UnsubscribeFromGameCircleInitializationEvents() {   
	AGSClient.ServiceReadyEvent -= Instance.ServiceReadyHandler;
	AGSClient.ServiceNotReadyEvent -= Instance.ServiceNotReadyHandler;
	}

	private void ServiceNotReadyHandler(string error) 
	{
	// Once the callback is received, these events do not need to be subscribed to.
	UnsubscribeFromGameCircleInitializationEvents();
	}   

	/// <summary>
	/// Callback when GameCircle is initialized and ready to use.
	/// </summary>
	private void ServiceReadyHandler() 
	{
	// Once the callback is received, these events do not need to be subscribed to.
	UnsubscribeFromGameCircleInitializationEvents();
	// Tell the GameCircle plugin the popup information set here.
	// Calling this after GameCircle initialization is safest.
	//		AGSClient.SetPopUpEnabled(true);
	//		AGSClient.SetPopUpLocation(GameCirclePopupLocation.TOP_CENTER);

	//		if(!Social.localUser.authenticated) 
	{
	AGSClient.ShowSignInPage();
	}
	//		else
	//		{
	//			Instance.PostScoreToLeaderBoard();
	//		}

	}


	private void submitScoreSucceeded(string leaderboardId)
	{
	AGSLeaderboardsClient.SubmitScoreSucceededEvent -= Instance.submitScoreSucceeded;
	AGSLeaderboardsClient.SubmitScoreFailedEvent -= Instance.submitScoreFailed;
	}

	private void submitScoreFailed(string leaderboardId, string error)
	{
	AGSLeaderboardsClient.SubmitScoreSucceededEvent -= Instance.submitScoreSucceeded;
	AGSLeaderboardsClient.SubmitScoreFailedEvent -= Instance.submitScoreFailed;
	}

	private void updateAchievementSucceeded(string achievementId) 
	{
	AGSAchievementsClient.UpdateAchievementSucceededEvent -= Instance.updateAchievementSucceeded;
	AGSAchievementsClient.UpdateAchievementFailedEvent -= Instance.updateAchievementFailed;
	}

	private void updateAchievementFailed(string achievementId, string error) 
	{
	AGSAchievementsClient.UpdateAchievementSucceededEvent -= Instance.updateAchievementSucceeded;
	AGSAchievementsClient.UpdateAchievementFailedEvent -= Instance.updateAchievementFailed;
	}
	#endif
	#endregion





	#region Google Play Services
	public void SignIn()
	{
		#if UNITY_ANDROID
		Social.localUser.Authenticate((bool success) => {
			// handle success or failure
			if(success)
			{
				//Post best score of user to leaderboard
				
				if(questChecker != null)
				{
					questChecker.PostAchievements();
				}
				Debug.Log("LOG_IN  Play Service Google");
			}
			else
			{
				Debug.Log("LOG_IN FALED Play Service Google");
				
			}
			
			
		});
		
		#elif UNITY_IOS
		Social.localUser.Authenticate (Instance.ProcessAuthentication);
		#endif
		
	}
	
	public void PostScoreToLeaderBoard()
	{
		#if AMAZON
		if(AGSClient.IsServiceReady())
		{
			AGSLeaderboardsClient.SubmitScoreSucceededEvent += Instance.submitScoreSucceeded;
			AGSLeaderboardsClient.SubmitScoreFailedEvent += Instance.submitScoreFailed;
			AGSLeaderboardsClient.SubmitScore(Constants.LEADER_BOARD_ID, Instance.puzzleSolved);
		}
		#else
		if (Social.localUser.authenticated) 
		{
			#if UNITY_ANDROID

			Social.ReportScore (Instance.puzzleSolved, Constants.LEADER_BOARD_ID, (bool success) =>
			                    {
				if (success) 
				{
					
				} 
				else 
				{
					//					Debug.Log ("Add Score Fail");
				}
			});
			
			#elif UNITY_IOS
			Social.ReportScore (Instance.puzzleSolved, Constants.LEADER_BOARD_ID, (bool success) =>
			                    {
				if (success) 
				{
					
				} 
				else 
				{
					//					Debug.Log ("Add Score Fail");
				}
			});
			#endif
		} 
		#endif
		
	}
	
	public void PostAchievement(string ID)
	{
		#if AMAZON
		{
			AGSAchievementsClient.UpdateAchievementSucceededEvent += Instance.updateAchievementSucceeded;
			AGSAchievementsClient.UpdateAchievementFailedEvent += Instance.updateAchievementFailed;
			AGSAchievementsClient.UpdateAchievementProgress(ID, 100.0f);
		}
		#else
		Social.ReportProgress(ID, 100.0f, (bool success) => {
			// handle success or failure
			if (success) 
			{
				//				Debug.Log("PostAchievement = " + ID);
			} 
			else 
			{
				//				Debug.Log ("PostAchievement Fail");
			}
		});
		#endif
	}
	
	public void ShowLeaderBoard()
	{
		#if AMAZON
		AGSLeaderboardsClient.ShowLeaderboardsOverlay();
		#else
		if (Social.localUser.authenticated) 
		{
			#if UNITY_ANDROID
			((PlayGamesPlatform)Social.Active).ShowLeaderboardUI(Constants.LEADER_BOARD_ID);
			#elif UNITY_IOS
			Social.ShowLeaderboardUI();
			#endif
		}
		else
		{
			//			Debug.Log("Not signed so login");
			SignIn();
		}
		#endif
	}
	
	public void ShowAcheivementsUI()
	{
		#if AMAZON
		AGSAchievementsClient.ShowAchievementsOverlay();
		#else
		Social.ShowAchievementsUI();
		#endif
	}
	
	#endregion
	
	
	#if UNITY_IOS
	UnityEngine.iOS.LocalNotification getNotification(string notif, int time)
	{
		//		string[] bodies = {
		//			"We are missing you, lets find some words", 
		//			"We are missing you, lets find some words",
		//			"We are missing you, lets find some words"
		//		};
		
		//		int index = UnityEngine.Random.Range (0, bodies.Length);
		UnityEngine.iOS.LocalNotification notification = new UnityEngine.iOS.LocalNotification();
		notification.fireDate = DateTime.Now.AddHours(time);
		//		notification.fireDate = DateTime.Now.AddMinutes (hour);
		//		notification.fireDate = DateTime.Now.AddSeconds(30);
		//		notification.applicationIconBadgeNumber = 1;
		notification.alertBody = notif;
		//		notification.repeatInterval = CalendarUnit.Minute;
		//		notification.soundName = "bing";
		notification.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
		
		return notification;
	}
	
	void ProcessAuthentication (bool success) 
	{
		if (success) 
		{
			Debug.Log ("Authenticated, checking achievements");
			//Post best score of user to leaderboard
			
			if(questChecker != null)
			{
				questChecker.PostAchievements();
			}
		}
		else
			Debug.Log ("Failed to authenticate");
	}
	
	#endif
	
	
	
	#region Facebook Functions
	public void FacebookLogin()
	{
		if(!FB.IsLoggedIn)
		{
			FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, this.LoginCallBack);
		}
		
	}
	
	
	
	public void FacbookInviteFrnds()
	{
		try
		{
			List<object> filter = new List<object>() { "app_non_users" };
			FB.AppRequest("Invite Friends", null, filter, null, 0, string.Empty, string.Empty, this.InviteCallBack);
		}
		catch (Exception e)
		{
			//			status = e.Message;
		}
	}
	
	
	
	#endregion
	
	#region Facebook Delegates
	private void OnInitComplete()
	{
		if (FB.IsInitialized) {
			// Signal an app activation App Event
			FB.ActivateApp();
			// Continue with Facebook SDK
			// ...
		} 
		else 
		{
			Debug.Log("Failed to Initialize the Facebook SDK");
		}
	}
	
	private void OnHideUnity(bool isGameShown)
	{
		//		this.Status = "Success - Check logk for details";
		//		this.LastResponse = string.Format("Success Response: OnHideUnity Called {0}\n", isGameShown);
		//		LogView.AddLog("Is game shown: " + isGameShown);
	}
	
	void LoginCallBack(ILoginResult result)
	{
		if (FB.IsLoggedIn) 
		{
			// AccessToken class will have session details
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
			// Print current access token's User ID
			Debug.Log(aToken.UserId);
			// Print current access token's granted permissions
			foreach (string perm in aToken.Permissions) 
			{
				Debug.Log(perm);
			}
			GameEventManager.TriggerFBLogin();
			//Quest check
			if(!questChecker.QUEST_FB_LOGIN)
			{
				PlayerPrefs.SetBool(Constants.QUEST_FB_LOGIN, true);
				questChecker.UpdateQuestStatus();
				MainDriver.Instance.PostAchievement(Constants.QUEST_FB_LOGIN_ID);
				MainDriver.Instance.AwardDiamonds(Constants.QUEST_AWARD);
			}
		} 
		else 
		{
			Debug.Log("User cancelled login");
		}
	}
	
	void InviteCallBack(IResult result)
	{
		
		
		if (!string.IsNullOrEmpty(result.Error))
		{
			//			this.Status = "Error - Check log for details";
			//			this.LastResponse = "Error Response:\n" + result.Error;
			//			LogView.AddLog(result.Error);
		}
		else if (result.Cancelled)
		{
			//			this.Status = "Cancelled - Check log for details";
			//			this.LastResponse = "Cancelled Response:\n" + result.RawResult;
			//			LogView.AddLog(result.RawResult);
		}
		else if (!string.IsNullOrEmpty(result.RawResult))
		{
			//			this.Status = "Success - Check log for details";
			//			this.LastResponse = "Success Response:\n" + result.RawResult;
			//			LogView.AddLog(result.RawResult);
			Debug.Log(result.RawResult);
			//Quest Check
			if(!questChecker.QUEST_INVITE_FRIENDS)
			{
				PlayerPrefs.SetBool(Constants.QUEST_INVITE_FRIENDS, true);
				questChecker.UpdateQuestStatus();
				MainDriver.Instance.PostAchievement(Constants.QUEST_INVITE_FRIENDS_ID);
				MainDriver.Instance.AwardDiamonds(Constants.QUEST_AWARD);
			}
			
		}
		else
		{
			//			this.LastResponse = "Empty Response\n";
			//			LogView.AddLog(this.LastResponse);
		}
	}
	#endregion
	
	
	public void AwardDiamonds(int dc)
	{
		MainDriver.Instance.currentDiamondCnt += dc;
		MainDriver.Instance.lifeTimeDiamondCnt += dc;
		PlayerPrefs.SetInt(Constants.KEY_CURRENT_DIAMONDS, MainDriver.Instance.currentDiamondCnt);
		PlayerPrefs.SetInt(Constants.KEY_LIFETIME_DIAMONDS, MainDriver.Instance.lifeTimeDiamondCnt);
		PlayerPrefs.Flush();
	}
	
	#region General Share Delegates
	
	#if UNITY_IPHONE
	
	[DllImport("__Internal")]
	private static extern void MediaShareIos (string iosPath, string message);
	
	[DllImport("__Internal")]
	private static extern void TextShareIos (string message);
	
	[DllImport("__Internal")]
	private static extern void GotoFacebookPage(string pageID);
	#endif

	string pathToShareImg;
	string subject = "Come & Play this Awesome Game with Me!";
	string appUrl = "http://onelink.to/k9ercm";
	string body = "" ;
	
	public  void ShareUniversal(string time)
	{
		MainDriver.Instance.body = "I completed \"" + GameData.gamePuzzleName + "\" puzzle in " +time+ " #WordSearch - Can You Beat Me? @1touchstudio " ;
		MainDriver.Instance.body += MainDriver.Instance.appUrl;
		StartCoroutine(MainDriver.Instance.SaveScreenShot());
		
	}
	
	public void ShareWhenTimeChallengeFailed(int wordCnt, string timeChallengeType )
	{
		MainDriver.Instance.body = "I found " + wordCnt + " words in \"" + GameData.gamePuzzleName + "\" puzzle in " + timeChallengeType + " #WordSearch - Can You Beat Me? @1touchstudio " ;
		MainDriver.Instance.body += MainDriver.Instance.appUrl;
		StartCoroutine(MainDriver.Instance.SaveScreenShot());

	}

	IEnumerator SaveScreenShot()
	{
		// create the texture
		yield return new WaitForEndOfFrame();
		MyImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24,true);//* 0.38f
		// put buffer into texture
		MyImage.ReadPixels(new Rect(0f, 0f , Screen.width, Screen.height),0,0);// * 0.8f
		// apply
		MyImage.Apply();
		yield return new WaitForEndOfFrame();
		byte[] bytes = MyImage.EncodeToPNG();
		pathToShareImg = Application.persistentDataPath + "/wordsearch.png";
		File.WriteAllBytes(pathToShareImg, bytes);
		yield return new WaitForEndOfFrame();

		#if UNITY_ANDROID
		StartCoroutine(MainDriver.Instance.ShareAndroidMedia());
		#elif UNITY_IPHONE
		MainDriver.Instance.ShareIosMedia();
		#endif

		//Check SHare Quest
		if(!questChecker.QUEST_FRIEND_SHARE)
		{
			PlayerPrefs.SetBool(Constants.QUEST_FRIEND_SHARE, true);
			questChecker.UpdateQuestStatus();
			MainDriver.Instance.PostAchievement(Constants.QUEST_FRIEND_SHARE_ID);
			MainDriver.Instance.AwardDiamonds(Constants.QUEST_AWARD);
		}
	}
	
	IEnumerator ShareAndroidText()
	{
		yield return new WaitForEndOfFrame();
		#if UNITY_ANDROID
				
		AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
		AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
		
		intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
		intentObject.Call<AndroidJavaObject>("setType", "text/plain");
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), MainDriver.Instance.subject);
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TITLE"), "WordSearch");
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), MainDriver.Instance.body);
		
		//		AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
		//		
		//		AndroidJavaObject fileObject = new AndroidJavaObject("java.io.File", path);// Set Image Path Here
		//		
		//		AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromFile", fileObject);
		
		//		string uriPath =  uriObject.Call<string>("getPath");
		//		bool fileExist = fileObject.Call<bool>("exists");
		//		Debug.Log("File exist : " + fileExist);
		AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
		currentActivity.Call("startActivity", intentObject);
		

		
		#endif
	}
	
	IEnumerator ShareAndroidMedia ()
	{

		yield return new WaitForEndOfFrame();
		#if UNITY_ANDROID

//		byte[] bytes = MyImage.EncodeToPNG();
//		pathToShareImg = Application.persistentDataPath + "/zigzagjump.png";
//		File.WriteAllBytes(pathToShareImg, bytes);

		AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
		AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

		intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
		intentObject.Call<AndroidJavaObject>("setType", "image/*");
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), MainDriver.Instance.subject);
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TITLE"), "ZigZagJump");
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), MainDriver.Instance.body);

		AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
		AndroidJavaClass fileClass = new AndroidJavaClass("java.io.File");

		AndroidJavaObject fileObject = new AndroidJavaObject("java.io.File", pathToShareImg);// Set Image Path Here

		AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromFile", fileObject);

		bool fileExist = fileObject.Call<bool>("exists");
		if (fileExist)
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);

		AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
		currentActivity.Call("startActivity", intentObject);

		#endif

	}
	
	void ShareIosMedia()
	{
		#if UNITY_IPHONE
		string shareMessage = MainDriver.Instance.body;
		MediaShareIos (pathToShareImg, shareMessage);
		#endif
	}
	
	void ShareIosText()
	{
		string shareMessage = MainDriver.Instance.body;
		#if UNITY_IPHONE
		TextShareIos(shareMessage);
		#endif
	}
	
	#endregion
	
	
	#region Sounds
	
	public void PlayThemeSound()
	{
		if(Instance.audioSource != null)
		{
			Instance.audioSource.PlayOneShot(Instance.packSound);
		}
	}
	
	public void PlayButtonSound()
	{
		if(Instance.audioSource != null)
		{
			Instance.audioSource.PlayOneShot(Instance.buttonSound);
		}
		
	}
	
	public void PlayWordSelectionSound()
	{
		if(Instance.audioSource != null)
		{
			Instance.audioSource.PlayOneShot(Instance.wordSelectionSound);
		}
	}
	
	public void PlayCorrectWordSound()
	{
		if(Instance.audioSource != null)
		{
			Instance.audioSource.PlayOneShot(Instance.correctSound);
		}
	}
	
	public void PlayLevelCompletion()
	{
		if(Instance.audioSource != null)
		{
			Instance.audioSource.PlayOneShot(Instance.levelWonSound);
		}
	}
	#endregion
}
