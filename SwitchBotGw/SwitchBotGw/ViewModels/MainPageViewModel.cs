using SwitchBotGw.DependencyServices;
using SwitchBotGw.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SwitchBotGw.ViewModels
{
    class MainPageViewModel : ViewModelBase
    {
        public Command TurnOnCommand { get; set; }
        public Command TurnOffCommand { get; set; }
        public Command DebugCommand { get; set; }
        private ISwitchBotService SwitchBot { get; set; }

        #region properties

        #endregion

        #region .ctor
        public MainPageViewModel(
            ISwitchBotService switchBot
            ) {
            SwitchBot = switchBot;

            TurnOnCommand = new Command(async () => await TurnOnAsync());
            TurnOffCommand = new Command(async () => await TurnOffAsync());
            DebugCommand = new Command(async () => await DebugAsync());
        }

        public MainPageViewModel() : this(
            App.DIContainer.GetInstance<ISwitchBotService>()
            ) { }
        #endregion

        #region Command Impl
        private async Task TurnOnAsync() {
            await SwitchBot.TurnOnAsync("cf46d116b6a1");
            Debug.WriteLine("TurnOn completed.");
        }

        private async Task TurnOffAsync() {
            await SwitchBot.TurnOffAsync("cf46d116b6a1");
            Debug.WriteLine("TurnOff completed.");
        }

        private Task DebugAsync() {
            var sb = DependencyService.Get<ISwitchBotTrigger>();
            sb.Start();
            //await SwitchBot.DoDebugAsync();
            Debug.WriteLine("DebugCommand completed.");
            return Task.CompletedTask;
        }
        #endregion
    }
}
