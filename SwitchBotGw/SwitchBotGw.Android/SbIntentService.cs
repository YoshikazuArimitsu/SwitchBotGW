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
using SwitchBotGw.Services;

namespace SwitchBotGw.Droid {
    [Service]
    public class SbIntentService : IntentService {
        const string TAG = "SbIntentService";

        public SbIntentService() : base("SbIntentService") {

        }

        protected override void OnHandleIntent(Intent intent) {
            Log.Info(TAG, "OnHandleIntent!!");
            var sb = App.DIContainer.GetInstance<ISwitchBotService>();
            sb.DoDebugAsync().Wait();
        }
    }
}