using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.MobileAds;
using AVFoundation;
using Foundation;
using CoreGraphics;
using UIKit;

namespace YutShot.iOS
{
	public class Native : YutShot.INative
	{
		Application app;
        GameHelper gameHelper;
		public Native(Application _app)
		{
			
			app = _app;

			if (app.supportGC) gameHelper = new GameHelper(app);
		}

        public void unlockAchievement(MainGame.Achievement achieve)
        {
            if (app.supportGC) gameHelper.unlockAchievement(achieve);
        }
        public void submitHighScore(int _score)
        {
            if (app.supportGC) gameHelper.submitScore(_score);
        }
        public void showLeaderboard()
        {
            if (app.supportGC) gameHelper.showLeaderboard();
        }
        public void setLife(int _life)
		{
			NSUserDefaults.StandardUserDefaults.SetInt(_life, "life");
			NSUserDefaults.StandardUserDefaults.Synchronize();
		}
		public int getLife()
		{
			return (int)NSUserDefaults.StandardUserDefaults.IntForKey("life");
		}
		public void FinishedLaunching(UIApplication app)
		{
			navController = UIApplication.SharedApplication.Windows[0].RootViewController;
			//window = new UIWindow(new CGRect((UIScreen.MainScreen.Bounds.Width - AdSizeCons.Banner.Size.Width) / 2, 0, AdSizeCons.Banner.Size.Width, AdSizeCons.Banner.Size.Height));//UIScreen.MainScreen.Bounds);
			window = new UIWindow(UIScreen.MainScreen.Bounds);
			window.RootViewController = navController;
			// window.MakeKeyAndVisible();
			showBannerAds();

		}
		public void setHighScore(int score)
		{
			NSUserDefaults.StandardUserDefaults.SetInt(score, "hs");
			NSUserDefaults.StandardUserDefaults.Synchronize();
		}
		public int getHighScore()
		{
			return (int)NSUserDefaults.StandardUserDefaults.IntForKey("hs");
		}
		//Ad

		UIWindow window;
		UIViewController navController;

		// BannerView adViewTableView;
		BannerView adViewWindow;
		Interstitial adInterstitial;

		// bool adOnTable = false;
		bool adOnWindow = false;
		bool interstitialRequested = false;

		const string bannerId = "ca-app-pub-8187309959937315/4131213185";
		const string intersitialId = "ca-app-pub-8187309959937315/5607946384";

		public void showBannerAds()
		{
			if (adViewWindow == null)
			{

				// Setup your GADBannerView, review AdSizeCons class for more Ad sizes. 
				adViewWindow = new BannerView(size: AdSizeCons.Banner,
					origin: new CGPoint((window.Bounds.Size.Width - AdSizeCons.Banner.Size.Width) / 2, 0)) //origin: new CGPoint(window.Bounds.Size.Width / 2, window.Bounds.Size.Height - AdSizeCons.Banner.Size.Height))
				{
					AdUnitID = bannerId,
					RootViewController = navController
				};

				// Wire AdReceived event to know when the Ad is ready to be displayed
				adViewWindow.AdReceived += (object sender, EventArgs e) =>
				{
					if (!adOnWindow)
					{
						//navController.View.Subviews.First().Frame = new CGRect(0, 0, 320, UIScreen.MainScreen.Bounds.Height - 50);
						navController.View.AddSubview(adViewWindow);
						adOnWindow = true;
					}
				};
			}
			adViewWindow.LoadRequest(Request.GetDefaultRequest());
		}

		public void hideBannerAds()
		{
			if (adViewWindow != null)
			{
				if (adOnWindow)
				{
					navController.View.Subviews.First().Frame = new CGRect(0, 0, 320, UIScreen.MainScreen.Bounds.Height);
					adViewWindow.RemoveFromSuperview();
				}
				adOnWindow = false;

				// You need to explicitly Dispose BannerView when you dont need it anymore
				// to avoid crashes if pending request are in progress
				adViewWindow.Dispose();
				adViewWindow = null;
			}
		}

		public void showInterstitialAds()
		{
			if (interstitialRequested)
				return;

			if (adInterstitial == null)
			{
				adInterstitial = new Interstitial(intersitialId);

				adInterstitial.ScreenDismissed += (sender, e) =>
				{
					interstitialRequested = false;

					// You need to explicitly Dispose Interstitial when you dont need it anymore
					// to avoid crashes if pending request are in progress
					adInterstitial.Dispose();
					adInterstitial = null;
				};
			}

			interstitialRequested = true;
			adInterstitial.LoadRequest(Request.GetDefaultRequest());

			ShowInterstitial();
		}

		async void ShowInterstitial()
		{
			// We need to wait until the Intersitial is ready to show
			do
			{
				await Task.Delay(100);
			} while (!adInterstitial.IsReady || app.game.appState == MainGame.AppState.InGame);

			// Once is ready, show it
			app.InvokeOnMainThread(() => adInterstitial.PresentFromRootViewController(navController));
		}
	}
    [Register("AppDelegate")]
    public class Application : UIApplicationDelegate
    {
        public MainGame game;
		public bool supportGC = UIDevice.CurrentDevice.CheckSystemVersion(7, 0);
        internal void RunGame()
        {
            game = new MainGame();
			game.NativeFunctions = new Native(this);
            game.Run();
        }
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            UIApplication.CheckForIllegalCrossThreadCalls = false;
            
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
            

        }
        public override void FinishedLaunching(UIApplication app)
        {
            AVAudioSession.SharedInstance().SetCategory(AVAudioSessionCategory.Ambient);
            RunGame();
			((Native)(game.NativeFunctions)).FinishedLaunching(app);
           
        }
    }
}
