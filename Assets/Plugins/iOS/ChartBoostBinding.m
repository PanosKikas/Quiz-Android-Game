/*
 * ChartboostBinding.m
 * Chartboost
 *
 * Copyright 2011 Chartboost. All rights reserved.
 */

#import "ChartBoostManager.h"
#import  "Chartboost.h"
#import  "CHBDataUseConsent.h"
#import  "CBAnalytics.h"


// Converts C style string to NSString
#define GetStringParam(_x_) (_x_ != NULL) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

// Converts C style string to NSString as long as it isnt empty
#define GetStringParamOrNil(_x_) (_x_ != NULL && strlen(_x_)) ? [NSString stringWithUTF8String:_x_] : nil

static char* MakeStringCopy(const char* string) {
    if (string == NULL) return NULL;
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

void _chartBoostInit(const char *appId, const char *appSignature, const char *unityVersion)
{
    [[ChartBoostManager sharedManager] startChartBoostWithAppId: GetStringParam(appId)
        appSignature: GetStringParam(appSignature)
        unityVersion: GetStringParam(unityVersion)];
}

BOOL _chartBoostIsAnyViewVisible()
{
    return [Chartboost isAnyViewVisible];
}

void _chartBoostCacheInterstitial(const char *location)
{
    [Chartboost cacheInterstitial: GetStringParamOrNil(location)];
}

BOOL _chartBoostHasInterstitial(const char *location)
{
	return [Chartboost hasInterstitial: GetStringParamOrNil(location)];
}

void _chartBoostShowInterstitial(const char *location)
{
    [Chartboost showInterstitial: GetStringParamOrNil(location)];
}


void _chartBoostCacheRewardedVideo(const char *location)
{
    [Chartboost cacheRewardedVideo: GetStringParamOrNil(location)];
}


BOOL _chartBoostHasRewardedVideo(const char *location)
{
	return [Chartboost hasRewardedVideo: GetStringParamOrNil(location)];
}


void _chartBoostShowRewardedVideo(const char *location)
{
    [Chartboost showRewardedVideo: GetStringParamOrNil(location)];
}

void _chartBoostSetCustomId(const char *ID)
{
    [Chartboost setCustomId: GetStringParamOrNil(ID)];
}


void _chartBoostDidPassAgeGate(BOOL pass)
{
    [Chartboost didPassAgeGate: pass];
}

char* _chartBoostGetCustomId()
{
    return MakeStringCopy([Chartboost getCustomId].UTF8String);
}


void _chartBoostHandleOpenURL(const char *url, const char *sourceApp)
{
    NSString *urlString = GetStringParamOrNil(url);
    if (!urlString)
        return;
    [Chartboost handleOpenURL: [NSURL URLWithString: urlString] sourceApplication: GetStringParamOrNil(sourceApp)];
}

void _chartBoostSetShouldPauseClickForConfirmation(BOOL pause)
{
    [Chartboost setShouldPauseClickForConfirmation:pause];
}

void _chartBoostSetShouldRequestInterstitialsInFirstSession(BOOL request)
{
    [Chartboost setShouldRequestInterstitialsInFirstSession:request];
}

// Functions called by the delegates
void _chartBoostShouldDisplayInterstitialCallbackResult(BOOL result)
{
    [ChartBoostManager sharedManager].unityResponseShouldDisplayInterstitial = result;
    if(!result)
    {
        [ChartBoostManager sharedManager].hasCheckedWithUnityToDisplayInterstitial = NO;
    }
}

void _chartBoostShouldDisplayRewardedVideoCallbackResult(BOOL result)
{
    [ChartBoostManager sharedManager].unityResponseShouldDisplayRewardedVideo = result;
    if(!result)
    {
        [ChartBoostManager sharedManager].hasCheckedWithUnityToDisplayRewardedVideo = NO;
    }
}

BOOL _chartBoostGetAutoCacheAds()
{
    return [Chartboost getAutoCacheAds];
}

void _chartBoostSetAutoCacheAds(BOOL autoCacheAds)
{
    [Chartboost setAutoCacheAds:autoCacheAds];
}

void _chartBoostSetStatusBarBehavior(int statusBarBehavior)
{
	[Chartboost setStatusBarBehavior:(CBStatusBarBehavior)statusBarBehavior];
}

void _chartBoostSetShouldPrefetchVideoContent(BOOL shouldPrefetch)
{
    [Chartboost setShouldPrefetchVideoContent:shouldPrefetch];
}

void _chartBoostTrackInAppPurchaseEvent(const char * receipt, const char * productTitle, const char * productDescription, const char * productPrice, const char * productCurrency, const char * productIdentifier)
{
    // The API was vague previously about what the receipt string meant.
    // We now expect a base64 encoded string.
    [CBAnalytics trackInAppPurchaseEventWithString:GetStringParamOrNil(receipt) productTitle:GetStringParamOrNil(productTitle) productDescription:GetStringParamOrNil(productDescription) productPrice:[NSDecimalNumber decimalNumberWithString:GetStringParamOrNil(productPrice)] productCurrency:GetStringParamOrNil(productCurrency) productIdentifier:GetStringParamOrNil(productIdentifier)];
}

void _chartBoostSetGameObjectName(const char *name)
{
    [ChartBoostManager sharedManager].gameObjectName = GetStringParam(name);
}

void _chartBoostTrackLevelInfo(const char * eventLabel, int levelType, int mainLevel, int subLevel, const char * description)
{
    [CBAnalytics trackLevelInfo:GetStringParamOrNil(eventLabel) eventField:(CBLevelType)levelType mainLevel:mainLevel subLevel:subLevel description:GetStringParamOrNil(description)];
}

void _chartBoostSetMediation(int mediator, const char *version)
{
    [Chartboost setMediation:(CBMediation)mediator withVersion:GetStringParamOrNil(version)];
}
void _chartBoostAddDataUseConsent(const char *privacyStandard,const char *consentValue)
{
    NSString *privacyStandardStr = GetStringParamOrNil(privacyStandard);
    NSString *consentValueStr = GetStringParamOrNil(consentValue);
    if (!privacyStandardStr || !consentValue)
        return;
    CHBDataUseConsent * consent;
    if ([privacyStandardStr isEqualToString:CHBPrivacyStandardCCPA])
    {
        consent = [CHBCCPADataUseConsent ccpaConsent:(CHBCCPAConsent) [consentValueStr intValue]];
    }
    else  if ([privacyStandardStr isEqualToString:CHBPrivacyStandardGDPR])
    {
        consent = [CHBGDPRDataUseConsent gdprConsent:(CHBGDPRConsent) [consentValueStr intValue]];
    }
    else
    {
        consent = [CHBCustomDataUseConsent customConsentWithPrivacyStandard:privacyStandardStr consent:consentValueStr];
    }
    if (consent)
    {
        [Chartboost addDataUseConsent:consent];
    }
}
char * _chartBoostGetConsentForPrivacyStandard(const char *privacyStandard)
{
    CHBPrivacyStandard standard = GetStringParamOrNil(privacyStandard);
    if (!standard)
        return nil;
    CHBDataUseConsent * consent = [Chartboost dataUseConsentForPrivacyStandard:standard];
    NSString *consentValue = nil;
    if ([consent isKindOfClass:[CHBCustomDataUseConsent class]])
    {
        consentValue = ((CHBCustomDataUseConsent*)consent).consent;
    }
    else if ([consent isKindOfClass:[CHBGDPRDataUseConsent class]])
    {
         consentValue = [NSString stringWithFormat:@"%ld",(long)((CHBGDPRDataUseConsent*)consent).consent];
    }
    else if ([consent isKindOfClass:[CHBCCPADataUseConsent class]])
    {
        consentValue = [NSString stringWithFormat:@"%ld",(long)((CHBCCPADataUseConsent*)consent).consent];
    }
    if (consentValue)
        return MakeStringCopy(consentValue.UTF8String);
    
    return nil;
    
}

void _chartBoostClearDataUseConsent(const char *privacyStandard)
{
    CHBPrivacyStandard standard = GetStringParamOrNil(privacyStandard);
    if (!standard)
        return;
    [Chartboost clearDataUseConsentForPrivacyStandard:standard];
}
void _chartBoostRestrictDataCollection(BOOL shouldRestrict)
{
    [Chartboost restrictDataCollection:shouldRestrict];
}

void _chartBoostsetPIDataUseConsent(CBPIDataUseConsent consent)
{
    [Chartboost setPIDataUseConsent:consent];
}

void _chartBoostSetMuted(BOOL muted)
{
    [Chartboost setMuted:muted];
}
