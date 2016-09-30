using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.GamerServices;
using GameKit;
using Foundation;

namespace YutShot.iOS
{
    class GameHelper
    {
        SignedInGamer gamer;
        Application app;
        public GameHelper(Application app)
        {
            
            gamer = new SignedInGamer();
            this.app = app;
            
        }
        public void submitScore(int score)
        {
            if (gamer.IsSignedInToLive)gamer.UpdateScore("grp.1shot.normal", score);
            
        }
        public void showLeaderboard()
        {
            Guide.ShowLeaderboard();
        }
        public void unlockAchievement(MainGame.Achievement achieve)
        {
            if (!gamer.IsSignedInToLive) return;
            
            /*
            ac = new AchievementCollection();
            ac = gamer.GetAchievements();
            
            foreach (Achievement a in ac)
            {
                if(a.IsEarned)
                achievements.Add(a.Key);
            }
            */
            GKAchievement.LoadAchievements(delegate (GKAchievement[] achievements, NSError error) {
                if (error == null)
                {
                    string code = null;
                    string msg = null;
                    List<string> achievementids = new List<string>();

                    if (achievements != null)
                    {
                        foreach (GKAchievement a in achievements)
                        {
                            if (a.Completed)
                                achievementids.Add(a.Identifier);
                        }
                    }
                    switch (achieve)
                    {
                        case MainGame.Achievement.BabyStep:

                            code = "grp.1shot.babystep";
                            msg = "Baby Step";
                            break;
                        case MainGame.Achievement._50Points:
                            code = "grp.1shot.50points";
                            msg = "50 Points!";
                            break;
                        case MainGame.Achievement._100Points:
                            code = "grp.1shot.100point";
                            msg = "A hundred points.";
                            break;
                        case MainGame.Achievement._70Points1Life:
                            code = "grp.1shot.70points1life";
                            msg = "1 Life, 70 Points.";
                            break;
                        case MainGame.Achievement.OutOfTheEarth:
                            code = "grp.1shot.outoftheearth";
                            msg = "Out of the Earth";
                            break;
                        case MainGame.Achievement.Interstellar:
                            code = "grp.1shot.interstellar";
                            msg = "Interstellar";
                            break;
                        case MainGame.Achievement.Doge:
                            code = "grp.1shot.doge";
                            msg = "DogeDogeDogeDoge";
                            break;
                    }

                    if (code != null)
                    {
                        if (!achievementids.Contains(code))
                        {
                            gamer.AwardAchievement(code);
                            GKNotificationBanner.Show("Achievement Unlocked!", msg, delegate { });
                        }

                    }
                }
            });
            
        }
    }
}