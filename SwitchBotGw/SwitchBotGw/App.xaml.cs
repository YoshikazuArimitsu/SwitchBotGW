using SwitchBotGw.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace SwitchBotGw
{
	public partial class App : Application
	{
        public static SimpleInjector.Container DIContainer;

		public App ()
		{
			InitializeComponent();

            //-- SimpleInjectorのセットアップ
            DIContainer = new SimpleInjector.Container();
            DIContainer.Register<ISwitchBotService>(() => {
                return new SwitchBotService();
            });
            DIContainer.Verify();

			MainPage = new SwitchBotGw.MainPage();
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
