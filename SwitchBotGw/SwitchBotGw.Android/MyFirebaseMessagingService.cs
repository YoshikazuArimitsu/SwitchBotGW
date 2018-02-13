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
using SwitchBotGw.Services;

namespace SwitchBotGw.Droid {
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService {
        const string TAG = "MyFirebaseMsgService";

        public override void OnMessageReceived(RemoteMessage message) {
            var device = message.Data["device"];
            var command = message.Data["command"];
            var switchBot = App.DIContainer.GetInstance<ISwitchBotService>();
            var result = command == "TurnOn" ? switchBot.TurnOnAsync(device).Result : switchBot.TurnOffAsync(device).Result;

            var n = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.abc_ic_star_black_16dp)
            .SetContentTitle("Hello notification")
            .SetVisibility(NotificationVisibility.Public)
            .Build();

            var nm = (NotificationManager)this.GetSystemService(Context.NotificationService);
            nm.Notify(0, n);
            /*
            if (message.Data != null) {

                var intent = new Intent(this, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.ClearTop);
                var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

                var notificationBuilder = new Notification.Builder(this)
                    .SetContentTitle("FCM Message")
                    .SetContentText("Hello")
                    .SetAutoCancel(true)
                    .SetContentIntent(pendingIntent);

                var notificationManager = NotificationManager.FromContext(this);
                notificationManager.Notify(0, notificationBuilder.Build());

                // データ部をJSON化。
                // JavaSetだとシリアライズできないので一旦Dictionaryに落としてからシリアライズ
                //PushNotification.FirePushReceived(
                //    JsonConvert.SerializeObject(
                //        new Dictionary<string, string>(message.Data)
                //        ));
            }
            */
        }
    }
}