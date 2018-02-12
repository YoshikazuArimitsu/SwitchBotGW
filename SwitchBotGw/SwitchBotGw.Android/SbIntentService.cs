using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using SwitchBotGw.Services;

namespace SwitchBotGw.Droid {
    [Service]
    public class SbIntentService : IntentService {
        const string TAG = "SbIntentService";

        public SbIntentService() : base("SbIntentService") {

        }

        protected override void OnHandleIntent(Intent intent) {
            Log.Info(TAG, "OnHandleIntent!!");
#if true
            // BLE操作
            var device = intent.GetStringExtra("device");
            var command = intent.GetStringExtra("command");
            var switchBot = App.DIContainer.GetInstance<ISwitchBotService>();

            var result = command == "TurnOn" ? switchBot.TurnOnAsync(device).Result : switchBot.TurnOffAsync(device).Result;
            var resultStr = result ? "Success" : "Failed";


            var n = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.icon)
            .SetContentTitle($"Device:{device} Command:{command} {resultStr}.")
            .SetVisibility(NotificationVisibility.Public)
            .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification),
            new AudioAttributes.Builder()
                    .SetUsage(AudioUsageKind.Notification)
                    .Build())
            .Build();

            var nm = (NotificationManager)this.GetSystemService(Context.NotificationService);
            nm.Notify(0, n);
#else
            // デバッグ用スキャン
            var sb = App.DIContainer.GetInstance<ISwitchBotService>();
            var r = sb.DoDebugAsync().Result;

            {
                var n = new Notification.Builder(this)
                    .SetSmallIcon(Resource.Drawable.icon)
                .SetContentTitle($"Scan() found {r} devices")
                .SetVisibility(NotificationVisibility.Public)
                .Build();

                var nm = (NotificationManager)GetSystemService(NotificationService);
                nm.Notify(0, n);
            }
#endif
        }
    }
}