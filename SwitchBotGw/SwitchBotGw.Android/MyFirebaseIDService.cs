using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Iid;
using Firebase.Messaging;

namespace SwitchBotGw.Droid {
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseIDService : FirebaseInstanceIdService {
        const string TAG = "MyFirebaseIIDService";

        public override void OnTokenRefresh() {
            var token_ = FirebaseInstanceId.Instance.Token;
            Log.Debug(TAG, "FCM token refresh: " + token_);

            // サンプル実装なのでFCMのトークン更新時はログを出すだけ。
            // 本番ではこのタイミングでWebAPIClient.ApiRegistrationPostAsyncを叩いて
            // RegistrationIdを払い出しなおす必要がある。
            // 登録時のTagsはどこかに覚えておき、払い出しなおしのタイミングでまた設定する。

            // トピック購読
            FirebaseMessaging.Instance.SubscribeToTopic("topic");
        }
    }
}