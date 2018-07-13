using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PlayerPrefs = PreviewLabs.PlayerPrefs;
using Facebook.Unity;

public class SettingsPanel : MonoBehaviour {

	public RectTransform settingPanel;
	public Text notificationText, soundText;

	Color onColor = new Color(100.0f/255, 201.0f/255, 74.0f/255);
	Color offColor = new Color(248.0f/255, 185.0f/255, 89.0f/255);

	void OnEnable()
	{
		if(GameData.isNotifcationON)
		{
			notificationText.text = "ON";
			notificationText.color = onColor;
		}
		else
		{
			notificationText.text = "OFF";
			notificationText.color = offColor;
		}
		
		if(GameData.isMusicON)
		{
			soundText.text = "ON";
			soundText.color = onColor;
		}
		else
		{
			soundText.text = "OFF";
			soundText.color = offColor;
		}
	}

	// Use this for initialization
	void Start () 
	{
		if(GameData.isNotifcationON)
		{
			notificationText.text = "ON";
			notificationText.color = onColor;
		}
		else
		{
			notificationText.text = "OFF";
			notificationText.color = offColor;
		}

		if(GameData.isMusicON)
		{
			soundText.text = "ON";
			soundText.color = onColor;
		}
		else
		{
			soundText.text = "OFF";
			soundText.color = offColor;
		}
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape)) 
		{
			BackFromSetting();
		}
	}
	
	public void BackFromSetting()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		settingPanel.gameObject.SetActive(false);
		if(Application.loadedLevel == 1)
		{
			LobbyHandler.currentPanel.gameObject.SetActive(true);
		}

	}

	public void NotificationOnOff()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		if(GameData.isNotifcationON)
		{
			GameData.isNotifcationON = false;
			notificationText.text = "OFF";
			notificationText.color = offColor;
		}
		else
		{
			GameData.isNotifcationON = true;
			notificationText.text = "ON";
			notificationText.color = onColor;
			
			#if UNITY_ANDROID
			EtceteraAndroid.cancelNotification( Constants.H4 );
			EtceteraAndroid.cancelNotification( Constants.H8 );
			EtceteraAndroid.cancelNotification( Constants.D1 );
			EtceteraAndroid.cancelNotification( Constants.D3 );
			EtceteraAndroid.cancelNotification( Constants.D7 );
			EtceteraAndroid.cancelNotification( Constants.D14 );
			EtceteraAndroid.cancelNotification( Constants.D30 );
			EtceteraAndroid.cancelAllNotifications();
			#elif UNITY_IOS
			NotificationServices.ClearLocalNotifications();
			NotificationServices.CancelAllLocalNotifications ();
			#endif
			
		}
		PlayerPrefs.SetBool(Constants.KEY_NOTIFICATIONS, GameData.isNotifcationON);
		PlayerPrefs.Flush();
	}

	public void SoundOnOff()
	{
		if(GameData.isMusicON)
		{
			GameData.isMusicON = false;
			soundText.text = "OFF";
			soundText.color = offColor;
		}
		else
		{
			GameData.isMusicON = true;
			soundText.text = "ON";
			soundText.color = onColor;
			if(GameData.isMusicON)
			{
				MainDriver.Instance.PlayButtonSound();
			}
		}
		PlayerPrefs.SetBool(Constants.KEY_SOUND, GameData.isMusicON);
		PlayerPrefs.Flush();
	}

	public void EmailSupport()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}

		string email =  "contact@1touchstudios.com"; //""MY EMAIL ADDRESS";
		
		string subject = MyEscapeURL("Word Search");
		
		string body = MyEscapeURL("Hi,\r\nHere is my problem description:");
		
		#if UNITY_ANDROID
		Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
		
		#else
		Application.OpenURL ("mailto:" + email + "?subject=" + subject + "&body=" + body);
		#endif
	}

	string MyEscapeURL (string url)
	{
		return WWW.EscapeURL(url).Replace("+","%20");	
	}

	public void FBLogOut()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		FB.LogOut();
	}

	public void TutorialPlay()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		GameData.gameMode = EGameMode.TutorialMode;
		GameData.puzzleFileName = "Tutorial";
		Application.LoadLevelAsync("GamePlay");
	}
}
