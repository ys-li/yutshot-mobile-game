using System;
namespace YutShot
{
	public interface INative
	{
		void showBannerAds();
		void hideBannerAds();
		void showInterstitialAds();
		void setHighScore(int _score);
		int getHighScore();
		void setLife(int _life);
		int getLife();
        void submitHighScore(int _score);
        void showLeaderboard();
        void unlockAchievement(MainGame.Achievement achieve);
	}
}

