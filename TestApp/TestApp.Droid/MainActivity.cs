using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Gms.Ads;
using YutShot.Droid;


namespace YutShot
{
	public class Native : YutShot.INative
	{
		MainActivity activity;
        public GameHelper gameHelper;
		public Native(MainActivity _activity)
		{
			activity = _activity;
            gameHelper = new GameHelper(activity);

            
            OnCreate();
		}
        public void unlockAchievement(MainGame.Achievement achieve)
        {

            if (gameHelper.SignedOut) return;
            string code = null;
            switch (achieve)
            {
                case MainGame.Achievement.BabyStep:
                    code = "CgkIndPlo6MHEAIQAw";
                    break;
                case MainGame.Achievement._50Points:
                    code = "CgkIndPlo6MHEAIQBA";
                    break;
                case MainGame.Achievement._100Points:
                    code = "CgkIndPlo6MHEAIQBQ";
                    break;
                case MainGame.Achievement._70Points1Life:
                    code = "CgkIndPlo6MHEAIQBg";
                    break;
                case MainGame.Achievement.OutOfTheEarth:
                    code = "CgkIndPlo6MHEAIQBw";
                    break;
                case MainGame.Achievement.Interstellar:
                    code = "CgkIndPlo6MHEAIQCA";
                    break;
                case MainGame.Achievement.Doge:
                    code = "CgkIndPlo6MHEAIQCQ";
                    break;
            }
            if (code != null) gameHelper.UnlockAchievement(code);
        }
        public void submitHighScore(int _score)
        {
			if (gameHelper.SignedOut) { gameHelper.SignIn(); if (!gameHelper.SignedOut) gameHelper.SubmitScore("CgkIndPlo6MHEAIQCw", _score); }
            else
            {
				gameHelper.SubmitScore("CgkIndPlo6MHEAIQCw", _score);
            }
        }
        public void showLeaderboard()
        {
            if (gameHelper.SignedOut) { gameHelper.SignIn(); if (!gameHelper.SignedOut) gameHelper.ShowAllLeaderBoardsIntent(); }
            else
            {
                gameHelper.ShowAllLeaderBoardsIntent();
            }
        }

        public void setLife(int _life)
		{
			ISharedPreferences prefs = activity.GetPreferences(FileCreationMode.Private);
			ISharedPreferencesEditor editor = prefs.Edit();
			editor.PutInt("life", _life);
			editor.Commit();
		}
		public int getLife()
		{
			ISharedPreferences prefs = activity.GetPreferences(FileCreationMode.Private);
			return prefs.GetInt("life", 0);
		}
		public void setHighScore(int score)
		{
			ISharedPreferences prefs = activity.GetPreferences(FileCreationMode.Private);
			ISharedPreferencesEditor editor = prefs.Edit();
			editor.PutInt("highscore", score);
			editor.Commit();
		}
		public int getHighScore()
		{
			ISharedPreferences prefs = activity.GetPreferences(FileCreationMode.Private);
			return prefs.GetInt("highscore", 0);
		}
		//Ad

		private AdView bannerAd;
		public AdView BannerAd
		{
			get { return bannerAd; }
		}
		private static InterstitialAd interstitial;
		private const string bannerID = "ca-app-pub-8187309959937315/7224280382";
		private const string interstitialID = "ca-app-pub-8187309959937315/1177746785";
		// private const string testDeviceID = "D10295D209DE2B07F58F84034E12F06C";
		private const string testDeviceID = "";
		private LinearLayout ll;
		private FrameLayout fl;
		public void OnCreate()
		{
			interstitial = AdWrapper.ConstructFullPageAd(activity, interstitialID);
            fl = new FrameLayout(activity);
            gameHelper.ViewForPopups = (View)activity.g.Services.GetService(typeof(View));
			fl.AddView((View)activity.g.Services.GetService(typeof(View)));
            
            
            
            gameHelper.Initialize();


            showBannerAds();
            //SetContentView((View)g.Services.GetService(typeof(View)));
            activity.SetContentView(fl);
        }
		public void showBannerAds()
		{
			if (ll == null)
			{
				hideBannerAds();
				ll = new LinearLayout(activity);
				ll.Orientation = Orientation.Horizontal;
				ll.SetGravity(GravityFlags.Center | GravityFlags.Top);
				bannerAd = new AdView(activity);
				bannerAd.AdSize = AdSize.Banner;
				bannerAd.AdUnitId = bannerID;
				ll.AddView(bannerAd);
				fl.AddView(ll);
			}
			bannerAd.CustomBuild(testDeviceID);
		}
		public void hideBannerAds()
		{
			if (ll != null)
			{
				fl.RemoveView(ll);
				ll.RemoveView(bannerAd);
				bannerAd.Dispose();
				bannerAd = null;
				ll.Dispose();
				ll = null;
			}
		}
		public void showInterstitialAds()
		{
			activity.RunOnUiThread(() =>
			{
				if (interstitial.AdListener != null)
					interstitial.AdListener.Dispose();
				interstitial.AdListener = null;
				var intlistener = new YutShot.adlistener();
				intlistener.AdLoaded += async () => {
					do
					{
						await System.Threading.Tasks.Task.Delay(100);
					} while (!interstitial.IsLoaded || activity.g.appState == MainGame.AppState.InGame);
					interstitial.Show(); 
				};
				interstitial.AdListener = intlistener;
				interstitial.CustomBuild(testDeviceID);
			});
		}

        //Leaderboards


	}
    [Activity(Label = "1Shot"
         , MainLauncher = true
         , Icon = "@drawable/icon"
         , AlwaysRetainTaskState = true
         , LaunchMode = LaunchMode.SingleInstance
         , ScreenOrientation = ScreenOrientation.Portrait
         , ConfigurationChanges = ConfigChanges.Orientation |
         ConfigChanges.Keyboard |
         ConfigChanges.KeyboardHidden)]
    public class MainActivity : Microsoft.Xna.Framework.AndroidGameActivity
    {
		public MainGame g;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            g = new MainGame();
			g.NativeFunctions = new Native(this);
           
            g.Run();
            
        }
        protected override void OnStart()
        {
            base.OnStart();
            ((Native)g.NativeFunctions).gameHelper.Start();
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (((Native)g.NativeFunctions).gameHelper != null)
                ((Native)g.NativeFunctions).gameHelper.OnActivityResult(requestCode, resultCode, data);
            base.OnActivityResult(requestCode, resultCode, data);
        }
        protected override void OnStop()
        {
            if (((Native)g.NativeFunctions).gameHelper != null)
                ((Native)g.NativeFunctions).gameHelper.Stop();
            base.OnStop();
        }
        protected override void OnPause()
        {
            base.OnPause();
        }
        protected override void OnResume()
        {
            base.OnResume();
        }


    }
    internal class adlistener : AdListener
    {
        // Declare the delegate (if using non-generic pattern). 
        public delegate void AdLoadedEvent();
        public delegate void AdClosedEvent();
        public delegate void AdOpenedEvent();
        // Declare the event. 
        public event AdLoadedEvent AdLoaded;
        public event AdClosedEvent AdClosed;
        public event AdOpenedEvent AdOpened;
        public event AdClosedEvent AdFailed;

        public override void OnAdFailedToLoad(int p0)
        {
            if (AdFailed != null) this.AdFailed();
            base.OnAdFailedToLoad(p0);
        }

        public override void OnAdLoaded()
        {
            if (AdLoaded != null) this.AdLoaded();
            base.OnAdLoaded();
        }


        public override void OnAdClosed()
        {
            if (AdClosed != null) this.AdClosed();
            base.OnAdClosed();
        }
        public override void OnAdOpened()
        {
            if (AdOpened != null) this.AdOpened();
            base.OnAdOpened();
        }
    }


    internal static class AdWrapper
    {
        public static InterstitialAd ConstructFullPageAd(Context con, string UnitID)
        {
            var ad = new InterstitialAd(con);
            ad.AdUnitId = UnitID;
            return ad;
        }
        public static AdView ConstructStandardBanner(Context con, AdSize adsize, string UnitID)
        {
            var ad = new AdView(con);
            ad.AdSize = adsize;
            ad.AdUnitId = UnitID;
            return ad;
        }
        public static InterstitialAd CustomBuild(this InterstitialAd ad, string testDeviceID)
        {
            //var requestbuilder = new AdRequest.Builder()
             //   .AddTestDevice(testDeviceID);
            var requestbuilder = new AdRequest.Builder();
            ad.LoadAd(requestbuilder.Build());
            return ad;
        }
        public static AdView CustomBuild(this AdView ad, string testDeviceID)
        {
           // var requestbuilder = new AdRequest.Builder()
              //  .AddTestDevice(testDeviceID);
            var requestbuilder = new AdRequest.Builder();
            ad.LoadAd(requestbuilder.Build());
            return ad;
        }
    }

}

