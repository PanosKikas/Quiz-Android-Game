using System;
using System.Text;
using UnityEngine;
using ChartboostSDK;
using System.Collections.Generic;

public class ChartboostExample: MonoBehaviour
{

	public Texture2D logo;

	public Vector2 scrollPosition = Vector2.zero;
	private List<string> delegateHistory;

	private bool hasInterstitial = false;
	private bool hasRewardedVideo = false;
	private int frameCount = 0;

	private bool ageGate = false;
	private bool autocache = true;
	private bool activeAgeGate = false;
	private bool showInterstitial = true;
	private bool showRewardedVideo = true;
	private int BANNER_HEIGHT = 110;
	private int REQUIRED_HEIGHT = 650;
	private int ELEMENT_WIDTH = 190;
	private Rect scrollRect;
	private Rect scrollArea;
	private Vector3 guiScale;
	private float scale;

#if UNITY_IPHONE
	private CBStatusBarBehavior statusBar = CBStatusBarBehavior.Ignore;
#endif

	void OnEnable() {
		SetupDelegates();
	}

	void Start() {
		delegateHistory = new List<string>();
		#if UNITY_IPHONE
		Chartboost.setShouldPauseClickForConfirmation(ageGate);
		#endif
		Chartboost.setAutoCacheAds(autocache);
		/* 
		//Uncomment to set your mediation partner (if any) such as AdMob, MoPub, Supersonic etc.
		Chartboost.setMediation (CBMediation.AdMob, "1.0");
		*/
    
    // Use this to restrict Chartboost's ability to collect personal data from the device. 
    // That is, whether or not IDFA and ip address should be collected by the SDK and the server. 
    // Use this to communicate an EU Data Subject's preference regarding data collection.
    	//Chartboost.setPIDataUseConsent(CBPIDataUseConsent.YesBehavioral);
		AddLog("Is Initialized: " + Chartboost.isInitialized());
		/*
		// Create the Chartboost gameobject with the editor AppId and AppSignature
		// Remove the Chartboost gameobject from the sample first
		Chartboost.Create();
		*/

		/*
		// Sample to create Chartboost gameobject from code overriding editor AppId and AppSignature
		// Remove the Chartboost gameobject from the sample first
		#if UNITY_IPHONE
		Chartboost.CreateWithAppId("4f21c409cd1cb2fb7000001b", "92e2de2fd7070327bdeb54c15a5295309c6fcd2d");
		#elif UNITY_ANDROID
		Chartboost.CreateWithAppId("4f7b433509b6025804000002", "dd2d41b69ac01b80f443f5b6cf06096d457f82bd");
		#endif
		*/
	}

	void SetupDelegates()
	{
		// Listen to all impression-related events
		Chartboost.didInitialize += didInitialize;
		Chartboost.didFailToLoadInterstitial += didFailToLoadInterstitial;
		Chartboost.didDismissInterstitial += didDismissInterstitial;
		Chartboost.didCloseInterstitial += didCloseInterstitial;
		Chartboost.didClickInterstitial += didClickInterstitial;
		Chartboost.didCacheInterstitial += didCacheInterstitial;
		Chartboost.shouldDisplayInterstitial += shouldDisplayInterstitial;
		Chartboost.didDisplayInterstitial += didDisplayInterstitial;
		Chartboost.didFailToRecordClick += didFailToRecordClick;
		Chartboost.didFailToLoadRewardedVideo += didFailToLoadRewardedVideo;
		Chartboost.didDismissRewardedVideo += didDismissRewardedVideo;
		Chartboost.didCloseRewardedVideo += didCloseRewardedVideo;
		Chartboost.didClickRewardedVideo += didClickRewardedVideo;
		Chartboost.didCacheRewardedVideo += didCacheRewardedVideo;
		Chartboost.shouldDisplayRewardedVideo += shouldDisplayRewardedVideo;
		Chartboost.didCompleteRewardedVideo += didCompleteRewardedVideo;
		Chartboost.didDisplayRewardedVideo += didDisplayRewardedVideo;
		Chartboost.didPauseClickForConfirmation += didPauseClickForConfirmation;
		Chartboost.willDisplayVideo += willDisplayVideo;
		#if UNITY_IPHONE
		Chartboost.didCompleteAppStoreSheetFlow += didCompleteAppStoreSheetFlow;
		#endif
	}

	private Vector2 beginFinger;        // finger
	private float deltaFingerY;         // finger
	private Vector2 beginPanel;         // scrollpanel
	private Vector2 latestPanel;        // scrollpanel

 	void Update() {
		UpdateScrolling();

		frameCount++;
		if( frameCount > 30 )
		{
			// update these periodically and not every frame
			hasInterstitial = Chartboost.hasInterstitial(CBLocation.Default);
			hasRewardedVideo = Chartboost.hasRewardedVideo(CBLocation.Default);

			frameCount = 0;
		}
	}

	void UpdateScrolling()
	{
	    if ( Input.touchCount != 1 ) return;

		Touch touch = Input.touches[0];
		if ( touch.phase == TouchPhase.Began )
		{
			beginFinger = touch.position;
			beginPanel = scrollPosition;
		}

		if ( touch.phase == TouchPhase.Moved )
		{
			Vector2 newFingerScreenPos = touch.position;
			deltaFingerY = newFingerScreenPos.y - beginFinger.y;

			float newY = beginPanel.y + (deltaFingerY / scale);

			latestPanel = beginPanel;
			latestPanel.y = newY;

			scrollPosition = latestPanel;
		}
	}

	void AddLog(string text)
	{
		Debug.Log(text);
		delegateHistory.Insert(0, text + "\n");
		int count = delegateHistory.Count;
		if( count > 20 )
		{
			delegateHistory.RemoveRange(20, count-20);
		}
	}

	void OnGUI() {
/*
#if UNITY_ANDROID
		// Disable user input for GUI when impressions are visible
		// This is only necessary on Android if we have disabled impression activities
		//   by having called CBBinding.init(ID, SIG, false), as that allows touch
		//   events to leak through Chartboost impressions
		GUI.enabled = !Chartboost.isImpressionVisible();
#endif
*/
	    //get the screen's width
	    float sWidth = Screen.width;
	    float sHeight = Screen.height;
	    //calculate the rescale ratio
	    float guiRatioX = sWidth/240.0f;
	    float guiRatioY = sHeight/210.0f;

	    float myScale = Mathf.Min(6.0f, Mathf.Min(guiRatioX, guiRatioY));
	    if(scale != myScale) {
	    	scale = myScale;
			guiScale = new Vector3(scale,scale,1);
	    }
		GUI.matrix = Matrix4x4.Scale(guiScale);

    	ELEMENT_WIDTH = (int)(sWidth/scale)-30;
    	float height = REQUIRED_HEIGHT;
    	
		scrollRect = new Rect(0, BANNER_HEIGHT, ELEMENT_WIDTH+30, sHeight/scale-BANNER_HEIGHT);
		scrollArea = new Rect(-10, BANNER_HEIGHT, ELEMENT_WIDTH, height);


		LayoutHeader();
		if( activeAgeGate )
		{
			GUI.ModalWindow(1, new Rect(0, 0, Screen.width, Screen.height), LayoutAgeGate, "Age Gate");
			return;
		}
		scrollPosition = GUI.BeginScrollView(scrollRect, scrollPosition, scrollArea);

		LayoutButtons();
		LayoutToggles();
		GUI.EndScrollView();
	}

	void LayoutHeader()
	{
		// A view with some debug information
		GUILayout.Label(logo, GUILayout.Height(30), GUILayout.Width(ELEMENT_WIDTH+20));
		String text = "";
		foreach( String entry in delegateHistory)
		{
			text += entry;
		}
		GUILayout.TextArea( text, GUILayout.Height(70), GUILayout.Width(ELEMENT_WIDTH+20));
	}

	void LayoutToggles()
	{
		GUILayout.Space(5);
		GUILayout.Label("Options:");
		showInterstitial = GUILayout.Toggle(showInterstitial, "Should Display Interstitial");
		showRewardedVideo = GUILayout.Toggle(showRewardedVideo, "Should Display Rewarded Video");
		if( GUILayout.Toggle(ageGate, "Should Pause for AgeGate") != ageGate )
		{
			ageGate = !ageGate; // toggle
			Chartboost.setShouldPauseClickForConfirmation(ageGate);
		}
		if( GUILayout.Toggle(autocache, "Auto cache ads") != autocache )
		{
			autocache = !autocache; // toggle
			Chartboost.setAutoCacheAds(autocache);
		}

		#if UNITY_IPHONE
		GUILayout.Label("Status Bar Behavior:");
		int slider = Mathf.RoundToInt(GUILayout.HorizontalSlider((int)statusBar, 0, 2, GUILayout.Width(ELEMENT_WIDTH/2)));
		if( slider != (int)statusBar )
		{
			statusBar = (CBStatusBarBehavior)slider;
			Chartboost.setStatusBarBehavior(statusBar);
			switch(statusBar)
			{
				case CBStatusBarBehavior.Ignore:
					AddLog("set to Ignore");
					break;
				case CBStatusBarBehavior.RespectButtons:
					AddLog("set to RespectButtons");
					break;
				case CBStatusBarBehavior.Respect:
					AddLog("set to Respect");
					break;
			}
		}
		#endif
	}

	void LayoutButtons()
	{
		// The view with buttons to trigger the main Chartboost API calls
		GUILayout.Space(5);
		GUILayout.Label("Has Interstitial: " + hasInterstitial);

		if (GUILayout.Button("Cache Interstitial", GUILayout.Width(ELEMENT_WIDTH))) {
			Chartboost.cacheInterstitial(CBLocation.Default);
		}
		
		if (GUILayout.Button("Show Interstitial", GUILayout.Width(ELEMENT_WIDTH))) {
			Chartboost.showInterstitial(CBLocation.Default);
		}
		
		GUILayout.Space(5);
		GUILayout.Label("Has Rewarded Video: " + hasRewardedVideo);
		if (GUILayout.Button("Cache Rewarded Video", GUILayout.Width(ELEMENT_WIDTH))) {
			Chartboost.cacheRewardedVideo(CBLocation.Default);
		}
		
		if (GUILayout.Button("Show Rewarded Video", GUILayout.Width(ELEMENT_WIDTH))) {
			Chartboost.showRewardedVideo(CBLocation.Default);
		}


		GUILayout.Space(5);
		GUILayout.Label("Privacy Management:");
		if (GUILayout.Button("Add CCPA Opt Out", GUILayout.Width(ELEMENT_WIDTH))) {
			Chartboost.addDataUseConsent(CBCCPADataUseConsent.OptOutSale);
		}
		if (GUILayout.Button("Add CCPA Opt In", GUILayout.Width(ELEMENT_WIDTH))) {
			Chartboost.addDataUseConsent(CBCCPADataUseConsent.OptInSale);
		}
		if (GUILayout.Button("Get CCPA", GUILayout.Width(ELEMENT_WIDTH))) {
			CBDataUseConsent dataUse = Chartboost.getDataUseConsent(CBPrivacyStandard.CCPA);
			if (dataUse != null)
				logDataUse(dataUse);
			else
				AddLog("CCPA is not set.");
		}
		if (GUILayout.Button("Clear CCPA", GUILayout.Width(ELEMENT_WIDTH))) {
			Chartboost.clearDataUseConsent(CBPrivacyStandard.CCPA);
		}
		if (GUILayout.Button("Add GDPR Behavioral", GUILayout.Width(ELEMENT_WIDTH))) {
			Chartboost.addDataUseConsent(CBGDPRDataUseConsent.Behavioral);
		}
		if (GUILayout.Button("Add GDPR No Behavioral", GUILayout.Width(ELEMENT_WIDTH))) {
			Chartboost.addDataUseConsent(CBGDPRDataUseConsent.NoBehavioral);
		}
		if (GUILayout.Button("Get GDPR", GUILayout.Width(ELEMENT_WIDTH))) {
			CBDataUseConsent dataUse = Chartboost.getDataUseConsent(CBPrivacyStandard.GDPR);
			if (dataUse != null)
				logDataUse(dataUse);
			else
				AddLog("GDPR is not set.");
		}
		if (GUILayout.Button("Clear GDPR", GUILayout.Width(ELEMENT_WIDTH))) {
			Chartboost.clearDataUseConsent(CBPrivacyStandard.GDPR);
		}
		if (GUILayout.Button("Add Foo Bar", GUILayout.Width(ELEMENT_WIDTH))) {
			Chartboost.addDataUseConsent(new CBCustomDataUseConsent("Foo","Bar"));
		}
		if (GUILayout.Button("Add Foo Biz", GUILayout.Width(ELEMENT_WIDTH))) {
			Chartboost.addDataUseConsent(new CBCustomDataUseConsent("Foo","Biz"));
		}
		if (GUILayout.Button("Get Foo", GUILayout.Width(ELEMENT_WIDTH))) {
			CBDataUseConsent dataUse = Chartboost.getDataUseConsent(CBPrivacyStandard.CustomPrivacyStandard("Foo"));
			if (dataUse != null)
				logDataUse(dataUse);
			else
				AddLog("Foo is not set.");
		}
		if (GUILayout.Button("Clear Foo", GUILayout.Width(ELEMENT_WIDTH))) {
			Chartboost.clearDataUseConsent(CBPrivacyStandard.CustomPrivacyStandard("Foo"));
		}
	}


	void LayoutAgeGate(int windowID)
	{
		GUILayout.Space(BANNER_HEIGHT);
		GUILayout.Label("Want to pass the age gate?");
		GUILayout.BeginHorizontal(GUILayout.Width(ELEMENT_WIDTH));
		if( GUILayout.Button("YES") )
		{
			Chartboost.didPassAgeGate(true);
			activeAgeGate = false;
		}
		if( GUILayout.Button("NO") )
		{
			Chartboost.didPassAgeGate(false);
			activeAgeGate = false;
		}

		GUILayout.EndHorizontal();
	}
	
	void OnDisable() {
		// Remove event handlers
		Chartboost.didInitialize -= didInitialize;
		Chartboost.didFailToLoadInterstitial -= didFailToLoadInterstitial;
		Chartboost.didDismissInterstitial -= didDismissInterstitial;
		Chartboost.didCloseInterstitial -= didCloseInterstitial;
		Chartboost.didClickInterstitial -= didClickInterstitial;
		Chartboost.didCacheInterstitial -= didCacheInterstitial;
		Chartboost.shouldDisplayInterstitial -= shouldDisplayInterstitial;
		Chartboost.didDisplayInterstitial -= didDisplayInterstitial;
		Chartboost.didFailToRecordClick -= didFailToRecordClick;
		Chartboost.didFailToLoadRewardedVideo -= didFailToLoadRewardedVideo;
		Chartboost.didDismissRewardedVideo -= didDismissRewardedVideo;
		Chartboost.didCloseRewardedVideo -= didCloseRewardedVideo;
		Chartboost.didClickRewardedVideo -= didClickRewardedVideo;
		Chartboost.didCacheRewardedVideo -= didCacheRewardedVideo;
		Chartboost.shouldDisplayRewardedVideo -= shouldDisplayRewardedVideo;
		Chartboost.didCompleteRewardedVideo -= didCompleteRewardedVideo;
		Chartboost.didDisplayRewardedVideo -= didDisplayRewardedVideo;
		Chartboost.didPauseClickForConfirmation -= didPauseClickForConfirmation;
		Chartboost.willDisplayVideo -= willDisplayVideo;
		#if UNITY_IPHONE
		Chartboost.didCompleteAppStoreSheetFlow -= didCompleteAppStoreSheetFlow;
		#endif
	}
	
	void logDataUse(CBDataUseConsent consent)
	{
		AddLog(string.Format("DataUse:{0}",consent.ToString()));
	}
	void didInitialize(bool status) {
		AddLog(string.Format("didInitialize: {0}", status));
	}

	void didFailToLoadInterstitial(CBLocation location, CBImpressionError error) {
		AddLog(string.Format("didFailToLoadInterstitial: {0} at location {1}", error, location));
	}
	
	void didDismissInterstitial(CBLocation location) {
		AddLog("didDismissInterstitial: " + location);
	}
	
	void didCloseInterstitial(CBLocation location) {
		AddLog("didCloseInterstitial: " + location);
	}
	
	void didClickInterstitial(CBLocation location) {
		AddLog("didClickInterstitial: " + location);
	}
	
	void didCacheInterstitial(CBLocation location) {
		AddLog("didCacheInterstitial: " + location);
	}
	
	bool shouldDisplayInterstitial(CBLocation location) {
		// return true if you want to allow the interstitial to be displayed
		AddLog("shouldDisplayInterstitial @" + location + " : " + showInterstitial);
		return showInterstitial;
	}
	
	void didDisplayInterstitial(CBLocation location){
		AddLog("didDisplayInterstitial: " + location);
	}

	void didFailToRecordClick(CBLocation location, CBClickError error) {
		AddLog(string.Format("didFailToRecordClick: {0} at location: {1}", error, location));
	}
	
	void didFailToLoadRewardedVideo(CBLocation location, CBImpressionError error) {
		AddLog(string.Format("didFailToLoadRewardedVideo: {0} at location {1}", error, location));
	}
	
	void didDismissRewardedVideo(CBLocation location) {
		AddLog("didDismissRewardedVideo: " + location);
	}
	
	void didCloseRewardedVideo(CBLocation location) {
		AddLog("didCloseRewardedVideo: " + location);
	}
	
	void didClickRewardedVideo(CBLocation location) {
		AddLog("didClickRewardedVideo: " + location);
	}
	
	void didCacheRewardedVideo(CBLocation location) {
		AddLog("didCacheRewardedVideo: " + location);
	}
	
	bool shouldDisplayRewardedVideo(CBLocation location) {
		AddLog("shouldDisplayRewardedVideo @" + location + " : " + showRewardedVideo);
		return showRewardedVideo;
	}
	
	void didCompleteRewardedVideo(CBLocation location, int reward) {
		AddLog(string.Format("didCompleteRewardedVideo: reward {0} at location {1}", reward, location));
	}

	void didDisplayRewardedVideo(CBLocation location){
		AddLog("didDisplayRewardedVideo: " + location);
	}

	void didPauseClickForConfirmation() {
		#if UNITY_IPHONE
		AddLog("didPauseClickForConfirmation called");
		activeAgeGate = true;
		#endif
	}

	void willDisplayVideo(CBLocation location) {
		AddLog("willDisplayVideo: " + location);
	}

#if UNITY_IPHONE
	void didCompleteAppStoreSheetFlow() {
		AddLog("didCompleteAppStoreSheetFlow");
	}
#endif

}


