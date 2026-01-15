//using AppsFlyerSDK;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class AppsflyerController : Singleton<AppsflyerController>
//{
//    public void Init()
//    {
//        AppsFlyerAdRevenue.start();
//    }

//    public void LogEvent(string eventName)
//    {
//        Dictionary<string, string>
//            eventValues = new Dictionary<string, string>();
//        eventValues.Add(AFInAppEvents.CURRENCY, "USD");
//        AppsFlyer.sendEvent(eventName, eventValues);
//    }

//    public void LogCustomEvent(string eventName, string paramName, string value)
//    {
//        Dictionary<string, string>
//            eventValues = new Dictionary<string, string>();
//        eventValues.Add(paramName, value);
//        eventValues.Add(AFInAppEvents.CURRENCY, "USD");
//        AppsFlyer.sendEvent(eventName, eventValues);
//    }

//    public void LogCustomEvent(string eventName, Dictionary<string, string> eventValues)
//    {
//        AppsFlyer.sendEvent(eventName, eventValues);
//    }
//}
