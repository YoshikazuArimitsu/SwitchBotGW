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
        }

        public MainPageViewModel() : this(
            App.DIContainer.GetInstance<ISwitchBotService>()
            ) { }
        #endregion

        #region Command Impl
        private async Task TurnOnAsync() {
            await SwitchBot.Test("cf46d116b6a1", SwitchBotService.TurnOnCommand);
            Debug.WriteLine("TurnOn completed.");
        }

        private async Task TurnOffAsync() {
            await SwitchBot.Test("cf46d116b6a1", SwitchBotService.TurnOffCommand);
            Debug.WriteLine("TurnOff completed.");
        }
        #endregion
    }
}
