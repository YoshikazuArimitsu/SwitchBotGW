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
using Xamarin.Forms;

namespace SwitchBotGw.Droid {
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService {
        const string TAG = "MyFirebaseMsgService";

        public override void OnMessageReceived(RemoteMessage message) {
#if false
            Intent intent = new Intent(Forms.Context, typeof(SbIntentService));
            Forms.Context.StartService(intent);
            //Intent intent = new Intent(this, typeof(SbIntentService));
            //StartService(intent);

#else
            var device = message.Data["device"];
            var command = message.Data["command"];
            var switchBot = App.DIContainer.GetInstance<ISwitchBotService>();

            var result = command == "TurnOn" ? switchBot.TurnOnAsync(device).Result : switchBot.TurnOffAsync(device).Result;
            var resultStr = result ? "Success" : "Failed";


            var n = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.icon)
            .SetContentTitle($"Device:{device} Command:{command} {resultStr}.")
            .SetVisibility(NotificationVisibility.Public)
            .Build();

            var nm = (NotificationManager)this.GetSystemService(Context.NotificationService);
            nm.Notify(0, n);
#endif
        }
    }
}