using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using System.IO;

public class Tapdaq : MonoBehaviour {

	// Interop Delegate
	public delegate void nativeAdDelegate(string _adObject);
	public delegate void interstitialAdDelegate(string _adObject);

	public delegate void Interop_InterstitialDelegateCallBack(string _adObject);
	public delegate void Interop_NativeDelegateCallBack(string _adObject, string _adObject2, string _adObject3);

	[DllImport ("__Internal")]
	private static extern void _GenerateCallBacks(Interop_NativeDelegateCallBack _callback, Interop_InterstitialDelegateCallBack _callback2);

#if UNITY_IPHONE

	//================================= Interstitials ==================================================
	[DllImport ("__Internal")]
	private static extern void _ConfigureTapdaq(string appIdChar, string clientKeyChar, int frequencyCap, int frequencyCapDurationInDays, string enabledAdTypesChar, bool testMode);

	[DllImport ("__Internal")]
	private static extern void _ShowInterstitial(int _pointer);

	//================================== Natives =================================================
	[DllImport ("__Internal")]
	private static extern void _FetchNative(nativeAdDelegate _delegate, int _adType, int _adSize, int _orientation); 

	[DllImport ("__Internal")]
	private static extern void _SendNativeClick(int _pointer);
	
	[DllImport ("__Internal")]
	private static extern void _SendNativeImpression(int _pointer);

#endif

	public enum TDAdType
	{
		interstitial,
		native,
	};
	
	public enum TDOrientation
	{
		portrait,
		landscape,
		universal
	};

	public enum TDNativeAdUnit
	{
		square,
		newsfeed,
		fullscreen,
		strip
	};
	
	public enum TDNativeAdSize
	{
		small,
		medium,
		large
	};

	private enum TDLogSeverity
	{
		debug,
		warning,
		error
	};

	private const string kTDMessageUnsupportedPlatform = "We support iOS and Android platforms only.";

#region Class Variables

	public static Tapdaq TDinstance;
	private TapdaqSettings settings;

	private string ios_applicationID = ""; 
	private string ios_clientKey = "";

	private string android_applicationID = ""; 
	private string android_clientKey = "";

	private int frequencyCap = 10; 
	private int frequencyCapDurationInDays = 2; 

	private bool testModeEnabled = true;
	private bool showLogMessages = false;

	//-----------Ad Types//
	private bool interstitials = false;

	private bool nativeSquareLarge = false;
	private bool nativeSquareMedium = false; 
	private bool nativeSquareSmall = false;
	private bool nativeNewsfeedPortraitLarge = false;
	private bool nativeNewsfeedPortraitMedium = false;
	private bool nativeNewsfeedPortraitSmall = false;
	private bool nativeNewsfeedLandscapeLarge = false;
	private bool nativeNewsfeedLandscapeMedium = false;
	private bool nativeNewsfeedLandscapeSmall = false;
	private bool nativeFullscreenPortraitLarge = false;
	private bool nativeFullscreenPortraitMedium = false; 
	private bool nativeFullscreenPortraitSmall = false;
	private bool nativeFullscreenLandscapeLarge = false;
	private bool nativeFullscreenLandscapeMedium = false;
	private bool nativeFullscreenLandscapeSmall = false;
	private bool nativeStripPortraitLarge = false;
	private bool nativeStripPortraitMedium = false;
	private bool nativeStripPortraitSmall = false;
	private bool nativeStripLandscapeLarge = false;
	private bool nativeStripLandscapeMedium = false;
	private bool nativeStripLandscapeSmall = false;

	private List<string>enabledAdTypes = new List<string>();

	private static TDNativeAd thisNativeAd;
	private static TapdaqNativeAd externalNative = new TapdaqNativeAd();

	///   delegates
	public delegate void willDisplayInterstitialDelegate();
	public delegate void didDisplayInterstitialDelegate();
	public delegate void didCloseInterstitialDelegate();
	public delegate void didClickInterstitialDelegate();
	public delegate void didFailToLoadInterstitialDelegate();
	public delegate void hasNoInterstitialsAvailableDelegate();
	public delegate void hasInterstitialsAvailableForOrientationDelegate(string orientation);

	public delegate void didFailToLoadNativeDelegate();
	public delegate void hasNoNativeAdvertAvailableDelegate();
	public delegate void hasNativeAdvertsAvailableForAdUnitDelegate(string adType, string adSize, string orientation);

	public delegate void didFailToConnectToServerDelegate(string message);
	
	public static event willDisplayInterstitialDelegate willDisplayInterstitial;
	public static event didDisplayInterstitialDelegate didDisplayInterstitial;
	public static event didCloseInterstitialDelegate didCloseInterstitial;
	public static event didClickInterstitialDelegate didClickInterstitial;
	public static event didFailToLoadInterstitialDelegate didFailToLoadInterstitial;
	public static event hasNoInterstitialsAvailableDelegate hasNoInterstitialsAvailable;
	public static event hasInterstitialsAvailableForOrientationDelegate hasInterstitialsAvailableForOrientation;

	public static event didFailToLoadNativeDelegate didFailToLoadNative;
	public static event hasNoNativeAdvertAvailableDelegate hasNoNativeAdvertAvailable;
	public static event hasNativeAdvertsAvailableForAdUnitDelegate hasNativeAdvertsAvailableForAdUnit;

	public static event didFailToConnectToServerDelegate didFailToConnectToServer;

#endregion

#region Native Class
	public class TDNativeAd
	{
		public string applicationId { get; private set; }
		public string targetingId { get; private set; }
		public string subscriptionId { get; private set; }  

		public string appName {get;private set;}
		public string appDescription {get;private set;}
		public string buttonText {get;private set;}
		public string developerName {get;private set;}
		public string ageRating {get;private set;}
		public string appSize {get;private set;}
		public float averageReview {get;private set;}
		public int totalReviews {get;private set;}
		public string category {get;private set;}
		public string appVersion {get;private set;}
		public float price {get;private set;}
		public string currency {get;private set;}
		public TDNativeAdUnit adUnit {get;private set;} // Can be either `TDNativeAdUnitSquare`, `TDNativeAdUnitNewsfeed`, `TDNativeAdUnitFullscreen`, `TDNativeAdUnitStrip`
		public TDNativeAdSize adSize {get;private set;} // Can be either `TDNativeAdSizeSmall`, `TDNativeAdSizeMedium`, `TDNativeAdSizeLarge`
		public string iconUrl {get;private set;}
		public Texture2D icon {get;private set;}
		
		public string creativeIdentifier {get;private set;}
		public TDOrientation creativeOrientation {get;private set;} // Can be either `TDOrientationPortrait` or `TDOrientationLandscape
		public string creativeResolution {get;private set;} // Can be `TDResolution1x`, `TDResolution2x` or `TDResolution3x`
		
		
		public int creativeAspectRatioWidth {get;private set;}
		public int creativeAspectRatioHeight {get;private set;}
		
		public string creativeURL {get;private set;}
		public Texture2D creativeImage {get;private set;}
		
		public int pointer{get;private set;}
		
		public TDNativeAd(string objcAdString)
		{
			string[] adObject = objcAdString.Split(new[]{"<>"},System.StringSplitOptions.None);
			if(adObject.Length == 0)
			{
				logMessage(TDLogSeverity.debug, "this ad Object is empty, SDK has not initialized yet.");
			}
			else{
			applicationId = adObject[0];
			targetingId = adObject[1];
			subscriptionId = adObject[2];

			appName = adObject[3];
			appDescription = adObject[4];
			buttonText = adObject[5];
			developerName = adObject[6];
			ageRating = adObject[7];
			appSize = adObject[8];
			averageReview = float.Parse(adObject[9]);
			totalReviews = System.Int32.Parse(adObject[10]);
			category = adObject[11];
			appVersion = adObject[12];
			price = float.Parse(adObject[13]);
			currency = adObject[14];
			adUnit = (TDNativeAdUnit)System.Int32.Parse(adObject[15]); // Can be either `TDNativeAdUnitSquare`, `TDNativeAdUnitNewsfeed`, `TDNativeAdUnitFullscreen`, `TDNativeAdUnitStrip`
			adSize = (TDNativeAdSize)System.Int32.Parse(adObject[16]); // Can be either `TDNativeAdSizeSmall`, `TDNativeAdSizeMedium`, `TDNativeAdSizeLarge`
			iconUrl = adObject[17];
			icon = PathToTexture(adObject[18]);
			
			creativeIdentifier = adObject[19];
			creativeOrientation = (TDOrientation)System.Int32.Parse(adObject[20]);
			creativeResolution = adObject[21];
			
			creativeAspectRatioWidth = System.Int32.Parse(adObject[22]);
			creativeAspectRatioHeight = System.Int32.Parse(adObject[23]);
			
			creativeURL = adObject[24];
			creativeImage = PathToTexture(adObject[25]);
			pointer = System.Int32.Parse(adObject[26]);
			}
			
			
		}
		
		//Read I.O. path and build texture.
		private Texture2D PathToTexture(string path)
		{
			switch(creativeResolution)
			{
			case(""):
				break;
				
			}
			
			int width = Screen.width;
			int height = Screen.height;
			
			Texture2D tex  = new Texture2D(width,height,TextureFormat.RGBA32,false);
			byte[] imageBytes;
			if(path!=null)
			{	
				imageBytes = File.ReadAllBytes(path);
				tex.LoadImage(imageBytes);
			}
			
			imageBytes = null;
			File.Delete(path);
			
			return tex;
			
		}
	}


	#endregion
	#region Android Listener Class
	#if UNITY_ANDROID
	class NativeAdFetchCallback : AndroidJavaProxy
	{
		public NativeAdFetchCallback() : base("com.nerd.TapdaqUnityPlugin.TapdaqUnity$NativeAdFetchListener") { }
		void onFetchFinished(string _adObj)
		{
			BuildAndroidNativeAd(_adObj);
		}
	}
	#endif
	#endregion

	void Awake()
	{
		if (TDinstance != null) { // Ensuring no clones
			Destroy(gameObject);
			return;
		}

		//Make the plugin available across all scenes
		TDinstance = this;
		GameObject.DontDestroyOnLoad (gameObject);
		settings = GetComponent<TapdaqSettings>();
	
		showLogMessages = settings.showLogs;
		testModeEnabled = settings.testMode;

		ios_applicationID = settings.ios_applicationID;
		ios_clientKey = settings.ios_clientKey;
		android_applicationID = settings.android_applicationID;
		android_clientKey = settings.android_clientKey;

		frequencyCap = settings.frequency;
		frequencyCapDurationInDays = settings.duration;

		interstitials = settings.interstitials;

		nativeSquareLarge = settings.nativeSquareLarge;
		nativeSquareMedium = settings.nativeSquareMedium;
		nativeSquareSmall = settings.nativeSquareSmall;

		nativeNewsfeedPortraitLarge = settings.nativeNewsfeedPortraitLarge;
		nativeNewsfeedPortraitMedium = settings.nativeNewsfeedPortraitMedium;
		nativeNewsfeedPortraitSmall = settings.nativeNewsfeedPortraitSmall;

		nativeNewsfeedLandscapeLarge = settings.nativeNewsfeedLandscapeLarge;
		nativeNewsfeedLandscapeMedium = settings.nativeNewsfeedLandscapeMedium;
		nativeNewsfeedLandscapeSmall = settings.nativeNewsfeedLandscapeSmall;

		nativeFullscreenPortraitLarge = settings.nativeFullscreenPortraitLarge;
		nativeFullscreenPortraitMedium = settings.nativeFullscreenPortraitMedium; 
		nativeFullscreenPortraitSmall = settings.nativeFullscreenPortraitSmall;

		nativeFullscreenLandscapeLarge = settings.nativeFullscreenLandscapeLarge;
		nativeFullscreenLandscapeMedium = settings.nativeFullscreenLandscapeMedium;
		nativeFullscreenLandscapeSmall = settings.nativeFullscreenLandscapeSmall;

		nativeStripPortraitLarge = settings.nativeStripPortraitLarge;
		nativeStripPortraitMedium = settings.nativeStripPortraitMedium;
		nativeStripPortraitSmall = settings.nativeStripPortraitSmall;

		nativeStripLandscapeLarge = settings.nativeStripLandscapeLarge;
		nativeStripLandscapeMedium = settings.nativeStripLandscapeMedium;
		nativeStripLandscapeSmall = settings.nativeStripLandscapeSmall;


		if(showLogMessages)
		{
			logMessage(TDLogSeverity.debug, "TapdaqSDK/Test Mode Active? -- " + testModeEnabled);

#if UNITY_IPHONE
			logMessage(TDLogSeverity.debug, "TapdaqSDK/Application ID -- " + ios_applicationID);
			logMessage(TDLogSeverity.debug, "TapdaqSDK/Client Key -- " + ios_clientKey);
#elif UNITY_ANDROID
			logMessage(TDLogSeverity.debug, "TapdaqSDK/Application ID -- " + android_applicationID);
			logMessage(TDLogSeverity.debug, "TapdaqSDK/Client Key -- " + android_clientKey);
#endif

			logMessage(TDLogSeverity.debug, "TapdaqSDK/Ad Frequency -- " + frequencyCap);
			logMessage(TDLogSeverity.debug, "TapdaqSDK/Ad Duration -- " + frequencyCapDurationInDays);

			if (interstitials) logMessage(TDLogSeverity.debug, "Interstitials enabled");
				
			if (nativeSquareLarge) logMessage(TDLogSeverity.debug, "TapdaqSDK/Native Square Large enabled");
			if (nativeSquareMedium) logMessage(TDLogSeverity.debug, "TapdaqSDK/Native Square Medium enabled");
			if (nativeSquareSmall)logMessage(TDLogSeverity.debug, "TapdaqSDK/Native Square Small enabled");

			if(nativeNewsfeedPortraitLarge)logMessage(TDLogSeverity.debug, "TapdaqSDK/Native News Feed Portrait Large enabled");
			if(nativeNewsfeedPortraitMedium)logMessage(TDLogSeverity.debug, "TapdaqSDK/Native News Feed Portrait Medium enabled");
			if(nativeNewsfeedPortraitSmall)logMessage(TDLogSeverity.debug, "TapdaqSDK/Native News Feed Portrait Small enabled");

			if(nativeNewsfeedLandscapeLarge)logMessage(TDLogSeverity.debug, "TapdaqSDK/Native News Feed Landscape Large enabled");
			if(nativeNewsfeedLandscapeMedium)logMessage(TDLogSeverity.debug, "TapdaqSDK/Native News Feed Landscape Medium enabled");
			if(nativeNewsfeedLandscapeSmall)logMessage(TDLogSeverity.debug, "TapdaqSDK/Native News Feed Landscape Small enabled");

			if(nativeFullscreenPortraitLarge)logMessage(TDLogSeverity.debug, "TapdaqSDK/Native Full Screen Portrait Large enabled");
			if(nativeFullscreenPortraitMedium)logMessage(TDLogSeverity.debug, "TapdaqSDK/Native Full Screen Portrait Medium enabled");
			if(nativeFullscreenPortraitSmall)logMessage(TDLogSeverity.debug, "TapdaqSDK/Native Full Screen Portrait Small enabled");

			if(nativeFullscreenLandscapeLarge)logMessage(TDLogSeverity.debug, "TapdaqSDK/Native Full Screen Landscape Large enabled");
			if(nativeFullscreenLandscapeMedium)logMessage(TDLogSeverity.debug, "TapdaqSDK/Native Full Screen Landscape Medium enabled");
			if(nativeFullscreenLandscapeSmall)logMessage(TDLogSeverity.debug, "TapdaqSDK/Native Full Screen Landscape Small enabled");

			if(nativeStripPortraitLarge)logMessage(TDLogSeverity.debug, "TapdaqSDK/Native Strip Portrait Large enabled");
			if(nativeStripPortraitMedium)logMessage(TDLogSeverity.debug, "TapdaqSDK/Native Strip Portrait Medium enabled");
			if(nativeStripPortraitSmall)logMessage(TDLogSeverity.debug, "TapdaqSDK/Native Strip Portrait Small enabled");

			if(nativeStripLandscapeLarge)logMessage(TDLogSeverity.debug, "TapdaqSDK/Native Strip Landscape Large enabled");
			if(nativeStripLandscapeMedium)logMessage(TDLogSeverity.debug, "TapdaqSDK/Native Strip Landscape Medium enabled");
			if(nativeStripLandscapeSmall)logMessage(TDLogSeverity.debug, "TapdaqSDK/Native Strip Landscape Small enabled");

		}

//		group = ns_group;
//		interstitialCanvas = ns_interstitialCanvas;
//		interstitialPortraitImage = ns_interstitialPortraitImage;
//		interstitialLandscapeImage = ns_interstitialLandscapeImage;

		//Initialize tapdaq and commence ad caching
		string enabledAdTypes = BuildEnabledAdTypesList();

#if UNITY_IPHONE
		GenerateCallbacks();
		TDinitialize(ios_applicationID, ios_clientKey, frequencyCap, frequencyCapDurationInDays, enabledAdTypes);
#elif UNITY_ANDROID
		TDinitialize(android_applicationID, android_clientKey, frequencyCap, frequencyCapDurationInDays, enabledAdTypes);
#endif

		externalNative = new TapdaqNativeAd();
	}

	[MonoPInvokeCallback(typeof(Interop_InterstitialDelegateCallBack))]
	private static void InterstitialDelegateCallBack(string _orientation)
	{
		TDinstance._hasInterstitialsAvailableForOrientation(_orientation);
	}
	
	[MonoPInvokeCallback(typeof(Interop_NativeDelegateCallBack))]
	private static void NativeDelegateCallBack(string _adObject, string _adObject2, string _adObject3)
	{
		TDinstance._hasNativeAdvertsAvailableForAdUnit(_adObject, _adObject2, _adObject3);
	}

	private void GenerateCallbacks()
	{
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor) {
			logMessage(TDLogSeverity.warning, kTDMessageUnsupportedPlatform);
		}
		
#if UNITY_IPHONE
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			_GenerateCallBacks(NativeDelegateCallBack, InterstitialDelegateCallBack);
		}
#endif
	}
	
	private void TDinitialize(string appID, string clientKey, int freq, int dur, string enabledAdTypes)
	{
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor) {
			logMessage(TDLogSeverity.warning, kTDMessageUnsupportedPlatform);
		}

		#if UNITY_IPHONE
		if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			logMessage(TDLogSeverity.debug, "TapdaqSDK/Initializing");

			_ConfigureTapdaq(appID, clientKey, freq, dur, enabledAdTypes, testModeEnabled);

		}
		#endif

		#if UNITY_ANDROID
		string _path = Application.persistentDataPath.Substring( 0, Application.persistentDataPath.Length - 5 );
		_path = _path.Substring( 0, _path.LastIndexOf( '/' ) );
		_path = Path.Combine( _path, "Documents/" );

		if(Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				AndroidJavaObject act = jc.GetStatic<AndroidJavaObject>("currentActivity");
				AndroidJavaObject appCtx = act.Call<AndroidJavaObject>("getApplicationContext");
				using (AndroidJavaClass mHumbleAssistantClass = new AndroidJavaClass("com.nerd.TapdaqUnityPlugin.TapdaqUnity"))
				{
					mHumbleAssistantClass.CallStatic("SetDataPath", appCtx,_path);
					mHumbleAssistantClass.CallStatic("InitiateTapdaq",act,appID,clientKey,enabledAdTypes, freq, dur, testModeEnabled);
				}
			}
		}
		#endif
	}


	private string BuildEnabledAdTypesList()
	{
		string adTypeList = "";

		if (interstitials) {
			if (Screen.width >= Screen.height) {
				enabledAdTypes.Add ("interstitialLandscape");
			} else {
				enabledAdTypes.Add ("interstitialPortrait");
			}
		}
			
		if (nativeSquareLarge == true) enabledAdTypes.Add("nativeSquareLarge");
		if (nativeSquareMedium == true) enabledAdTypes.Add("nativeSquareMedium");
		if (nativeSquareSmall == true) enabledAdTypes.Add("nativeSquareSmall");

		if (nativeNewsfeedPortraitLarge == true) enabledAdTypes.Add("nativeNewsfeedPortraitLarge");
		if (nativeNewsfeedPortraitMedium == true) enabledAdTypes.Add("nativeNewsfeedPortraitMedium");
		if (nativeNewsfeedPortraitSmall == true) enabledAdTypes.Add("nativeNewsfeedPortraitSmall");

		if (nativeNewsfeedLandscapeLarge == true) enabledAdTypes.Add("nativeNewsfeedLandscapeLarge");
		if (nativeNewsfeedLandscapeMedium == true) enabledAdTypes.Add("nativeNewsfeedLandscapeMedium");
		if (nativeNewsfeedLandscapeSmall == true) enabledAdTypes.Add("nativeNewsfeedLandscapeSmall");

		if (nativeFullscreenPortraitLarge == true) enabledAdTypes.Add("nativeFullscreenPortraitLarge");
		if (nativeFullscreenPortraitMedium == true) enabledAdTypes.Add("nativeFullscreenPortraitMedium");
		if (nativeFullscreenPortraitSmall == true) enabledAdTypes.Add("nativeFullscreenPortraitSmall");

		if (nativeFullscreenLandscapeLarge == true) enabledAdTypes.Add("nativeFullscreenLandscapeLarge");
		if (nativeFullscreenLandscapeMedium == true) enabledAdTypes.Add("nativeFullscreenLandscapeMedium");
		if (nativeFullscreenLandscapeSmall == true) enabledAdTypes.Add("nativeFullscreenLandscapeSmall");

		if (nativeStripPortraitLarge == true) enabledAdTypes.Add("nativeStripPortraitLarge");
		if (nativeStripPortraitMedium == true) enabledAdTypes.Add("nativeStripPortraitMedium");
		if (nativeStripPortraitSmall == true) enabledAdTypes.Add("nativeStripPortraitSmall");

		if (nativeStripLandscapeLarge == true) enabledAdTypes.Add("nativeStripLandscapeLarge");
		if (nativeStripLandscapeMedium == true) enabledAdTypes.Add("nativeStripLandscapeMedium");
		if (nativeStripLandscapeSmall == true) enabledAdTypes.Add("nativeStripLandscapeSmall");

		//Array must contain at least 1 value;
		if(enabledAdTypes.Count == 0) enabledAdTypes.Add("interstitialPortrait");

#if UNITY_IPHONE
		foreach (string adType in enabledAdTypes)
		{
			adTypeList += adType + ",";
		}
#elif UNITY_ANDROID
		foreach (string adType in enabledAdTypes)
		{
			adTypeList += adType + ",";
		}
#endif
	
		return adTypeList;
	}

	[MonoPInvokeCallback(typeof(nativeAdDelegate))]
	private static void BuildNativeAd(string _adObject)
	{
		thisNativeAd = new TDNativeAd(_adObject);
		externalNative.Build(thisNativeAd);			
	}

	public static void BuildAndroidNativeAd(string _adObject)
	{
		thisNativeAd = new TDNativeAd(_adObject);
		externalNative.Build(thisNativeAd);

	}

	public void FetchFailed(string msg)
	{
		print (msg);
		logMessage(TDLogSeverity.debug, "unable to fetch more ads");
	}

	public static void ShowInterstitial()
	{
		if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			logMessage(TDLogSeverity.warning, kTDMessageUnsupportedPlatform);
		}

		#if UNITY_IPHONE
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			//fire off event
			if(Tapdaq.TDinstance) {
				_ShowInterstitial(0);
			}
		}
		#endif
	#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				AndroidJavaObject act = jc.GetStatic<AndroidJavaObject>("currentActivity");
				AndroidJavaObject appCtx = act.Call<AndroidJavaObject>("getApplicationContext");
				using (AndroidJavaClass mHumbleAssistantClass = new AndroidJavaClass("com.nerd.TapdaqUnityPlugin.TapdaqUnity"))
				{
					mHumbleAssistantClass.CallStatic("ShowInterstitial",act);
				}
			}
		}
	#endif

	}

	public static TapdaqNativeAd GetNativeAd(TDNativeAdUnit adType, TDNativeAdSize adSize, TDOrientation orientation)
	{
#if UNITY_IPHONE
		_FetchNative(BuildNativeAd,(int)adType, (int)adSize, (int)orientation);
#endif	
#if UNITY_ANDROID
		using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaClass mHumbleAssistantClass = new AndroidJavaClass("com.nerd.TapdaqUnityPlugin.TapdaqUnity"))
			{
				mHumbleAssistantClass.CallStatic("FetchNativeAd",(int)adType, (int)adSize, (int)orientation,new NativeAdFetchCallback());
			}
		}
#endif

		return new TapdaqNativeAd().Build(thisNativeAd);
	}

	public static void SendNativeImpression(TapdaqNativeAd ad)
	{
		if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			logMessage(TDLogSeverity.debug, "Tapdaq Dummy: Send Native Impression");
		}
		else if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			#if UNITY_IPHONE
			_SendNativeImpression(ad.pointer);
			#endif

		}
		else if(Application.platform == RuntimePlatform.Android)
		{
			#if UNITY_ANDROID
			using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				AndroidJavaObject act = jc.GetStatic<AndroidJavaObject>("currentActivity");
				AndroidJavaObject appCtx = act.Call<AndroidJavaObject>("getApplicationContext");
				using (AndroidJavaClass mHumbleAssistantClass = new AndroidJavaClass("com.nerd.TapdaqUnityPlugin.TapdaqUnity"))
				{
					mHumbleAssistantClass.CallStatic("SendNativeImpression",ad.pointer,appCtx);
				}
			}
			#endif
		}
	}

	public static void SendNativeClick(TapdaqNativeAd ad)
	{
		if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			logMessage(TDLogSeverity.debug, "Tapdaq Dummy: Send Native Click");
		}
		else if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			#if UNITY_IPHONE
			_SendNativeClick(ad.pointer);
			#endif

		}
		else if(Application.platform == RuntimePlatform.Android)
		{
			#if UNITY_ANDROID
			using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				AndroidJavaObject act = jc.GetStatic<AndroidJavaObject>("currentActivity");
				AndroidJavaObject appCtx = act.Call<AndroidJavaObject>("getApplicationContext");
				using (AndroidJavaClass mHumbleAssistantClass = new AndroidJavaClass("com.nerd.TapdaqUnityPlugin.TapdaqUnity"))
				{
					mHumbleAssistantClass.CallStatic("SendNativeClick",ad.pointer,appCtx);
				}
			}
			#endif
		}
	}

	public static void DoCoroutine(string method)
	{
		TDinstance.StopCoroutine(method);
		TDinstance.StartCoroutine(method); //this will launch the coroutine on our instance
	}

	//Delegate Events
	// Called before interstitial is shown
	public void _willDisplayInterstitial(string msg)
	{
		if(willDisplayInterstitial != null)
		{
			willDisplayInterstitial();
		}
	}

	// Called after interstitial is shown
	public void _didDisplayInterstitial(string msg)
	{
		if(didDisplayInterstitial != null)
		{
			didDisplayInterstitial();
		}
	}

	// Called when interstitial is closed
	public void _didCloseInterstitial(string msg)
	{
		if(didCloseInterstitial != null)
		{
			didCloseInterstitial();
		}
	}

	// Called when interstitial is clicked
	public void _didClickInterstitial(string msg)
	{
		//callbacks aint firing fomr android

		if(didClickInterstitial != null)
		{
			didClickInterstitial();
		}
	}

	// Called with an error occurs when requesting
	// interstitials from the Tapdaq servers
	public void _didFailToLoadInterstitial(string msg)
	{
		if(didFailToLoadInterstitial != null)
		{
			didFailToLoadInterstitial();
		}
	}

	// Called when the request for interstitials was successful,
	// but no interstitials were found
	public void _hasNoInterstitialsAvailable(string msg)
	{
		if(hasNoInterstitialsAvailable != null)
		{
			hasNoInterstitialsAvailable();
		}
	}

	// Called when the request for interstitials was successful
	// and 1 or more interstitials were found
	//NOTE - this event will be called everytime an interstitial is cached and ready to be shown. 
	//       which can happen multiple times in a few frames
	public void _hasInterstitialsAvailableForOrientation(string orientation)
	{
		if(hasInterstitialsAvailableForOrientation != null)
		{
			hasInterstitialsAvailableForOrientation(orientation);
		}
	}

	//NATIVE DELEGATES
	// Called with an error occurs when requesting
	// natives from the Tapdaq servers
	public void _didFailToLoadNativeAdverts()
	{
		if(didFailToLoadNative != null)
		{
			didFailToLoadNative();
		}
	}

	// Called when the request for natives was successful,
	// but no Natives of the requested spec were found
	public void _hasNoNativeAdvertsAvailable()
	{
		if(hasNoNativeAdvertAvailable != null)
		{
			hasNoNativeAdvertAvailable();
		}
	}
	
	// Called when the request for natives was successful
	// and 1 or more natives were found
	//NOTE - this event will be called everytime a native is cached and ready to be shown. 
	//       which can happen multiple times in a few frames
	public void _hasNativeAdvertsAvailableForAdUnit(string unit, string size, string orientation)
	{
		if(hasNativeAdvertsAvailableForAdUnit !=null)
		{
			hasNativeAdvertsAvailableForAdUnit(unit, size, orientation);
		}
	}

	public void _Android_hasNoNativeAdvertsAvailable(string msg)
	{
		if(hasNoNativeAdvertAvailable != null)
		{
			hasNoNativeAdvertAvailable();
		}
	}

	public void _Android_hasNativeAdvertsAvailableForUnit(string msg)
	{

		string[] adObject = msg.Split(new[]{"<>"},System.StringSplitOptions.None);

		string adUnit;
		string adSize;
		string adOrientation;

		switch(adObject[0])
		{
		case("_1X1"):
		{
			adUnit = "0";//"square";
			adOrientation = "2";//"universal";
		}
			break;

		case("_2X1"):
		case("_1X2"):
		{
			adUnit = "1";//"newsfeed";
			if(adObject[0] == "_2X1")
				adOrientation = "0";//"portrait";
			else
				adOrientation = "1";//"landscape";
		}
			break;

		case("_2X3"):
		case("_3X2"):
		{
			adUnit = "2";//"fullscreen";
			if(adObject[0] == "_2X3")
				adOrientation = "0";//"portrait";
			else
				adOrientation = "1";//"landscape";
		}
			break;

		case("_5X1"):
		case("_1X5"):
		{
			adUnit = "3";//"strip";
			if(adObject[0] == "_5X1")
				adOrientation = "0";//"portrait";
			else
				adOrientation = "1";//"landscape";
		}
			break;
		default:
			adUnit = "0";//"square";
			adOrientation = "0";//"portrait";
			break;
		}

		switch(adObject[1])
		{
		case("0"):
		{
			adSize = "0";//"small";
		}
			break;

		case("1"):
		{
			adSize = "1";//"medium";
		}
			break;

		case("2"):
		{
			adSize = "2";//"large";
		}
			break;

		default:
			adSize = "3";//"small";
			break;
		}


		if(hasNativeAdvertsAvailableForAdUnit !=null)
		{
			hasNativeAdvertsAvailableForAdUnit(adUnit, adSize, adOrientation);
		}
	}

	public void _FailedToConnectToServer(string msg)
	{
		if(didFailToConnectToServer != null)
		{
			didFailToConnectToServer(msg);
		}
	}

	public void _UnexpectedErrorHandler(string msg)
	{
		logMessage(TDLogSeverity.error, msg);
	}

	private static void logMessage(TDLogSeverity severity, string message)
	{
		string prefix = "Tapdaq Unity SDK: ";
		if (severity == TDLogSeverity.warning) {
			Debug.LogWarning(prefix + message);
		} else if (severity == TDLogSeverity.error) {
			Debug.LogError(prefix + message);
		} else {
			Debug.Log(prefix + message);
		}

	}

}
