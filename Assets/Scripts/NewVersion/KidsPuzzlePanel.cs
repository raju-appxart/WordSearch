using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KidsPuzzlePanel : MonoBehaviour {
	
	public RectTransform kidsPanel;

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape)) 
		{
			BackToMainMenu();
		}
	}


	public void BackToMainMenu()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		//Back from Kids Mode
		GameData.gameMode = EGameMode.FullMode;
		kidsPanel.gameObject.SetActive(false);
		LobbyHandler.currentPanel.gameObject.SetActive(true);
	}


}
