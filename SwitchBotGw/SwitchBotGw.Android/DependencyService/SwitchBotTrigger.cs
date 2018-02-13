using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Iid;
using SwitchBotGw.DependencyServices;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(SwitchBotGw.Droid.DependencyServices.SwitchBotTrigger))]
namespace SwitchBotGw.Droid.DependencyServices
{
    class SwitchBotTrigger : ISwitchBotTrigger {
        public void Start() {
            Intent intent = new Intent(Forms.Context, typeof(SbIntentService));
            Forms.Context.StartService(intent);
        }
    }
}