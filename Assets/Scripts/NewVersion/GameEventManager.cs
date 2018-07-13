public static class GameEventManager 
{
	
	public delegate void GameEvent();
	
	public static event GameEvent ThemeSetected, PuzzleSelected, FacebookLoggedIn, StartVideoTimer, VideoTimerFinished;
	
	public static void TriggerThemeSelection()
	{
		if(ThemeSetected != null){
			ThemeSetected();
		}
	}
	
	public static void TriggerPuzzleSelection()
	{
		if(PuzzleSelected != null)
		{
			PuzzleSelected();
		}
	}

	public static void TriggerFBLogin()
	{
		if(FacebookLoggedIn != null)
		{
			FacebookLoggedIn();
		}
	}

	public static void TriggerStartVideoTimer()
	{
		if(StartVideoTimer != null)
		{
			StartVideoTimer();
		}
	}

	public static void TriggerVideoTimerFinished()
	{
		if(VideoTimerFinished != null)
		{
			VideoTimerFinished();
		}
	}


}
