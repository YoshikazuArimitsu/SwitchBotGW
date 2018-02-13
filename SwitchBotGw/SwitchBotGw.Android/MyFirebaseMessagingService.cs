using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Messaging;

namespace SwitchBotGw.Droid {
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService {
        const string TAG = "MyFirebaseMsgService";

        public override void OnMessageReceived(RemoteMessage message) {
            Log.Debug(TAG, "From: " + message.From);
            if (message.Data != null) {
                // データ部をJSON化。
                // JavaSetだとシリアライズできないので一旦Dictionaryに落としてからシリアライズ
                //PushNotification.FirePushReceived(
                //    JsonConvert.SerializeObject(
                //        new Dictionary<string, string>(message.Data)
                //        ));
            }
        }
    }
}