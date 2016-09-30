using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace YutShot
{
    public class MainGame : Game
    {
        //TODO: food-based -> fat guy, high score
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RenderTarget2D renderBuffer;
        Vector2 renderScale;
        
        public AppState appState = AppState.Menu;
        public GameState gameState = GameState.Starting;
        public byte currentLevel = 0;
        public byte currentRound = 0;
        Bullet bullet;
        Jack jack = new Jack();
        List<Projectile> Projectiles = new List<Projectile>();
        //List<string> debugList = new List<string>();
        public List<TimerAction> Timers = new List<TimerAction>();
		List<GUIElement> GUIElements = new List<GUIElement>();
        Score score = new Score();
        Score highScore = new Score();
        Score livesUsed = new Score();
        public List<DebrisEntity> Debris = new List<DebrisEntity>();
        Backdrop backdrop;
        //public Func<int> getHighScore;
        //public Action<int> setHighScore;
        //public Action showBannerAd, hideBannerAd, showInterstitialAd;
        public bool receivingInput = true;
        public bool paused = false;
        int consecutivePlays = 1;
        bool creditinterstitial = false;
        static public Random rand = new Random(DateTime.Now.Millisecond);
		public INative NativeFunctions;
		public Life life;
        public bool losing = false;
        public enum Achievement
        {
            BabyStep,
            _50Points,
            _100Points,
            _70Points1Life,
            OutOfTheEarth,
            Interstellar,
            Doge
        }
        public enum AppState
        {
            InGame,
            Menu,
            Credits,
            Results
        }
        public enum GameState
        {
            Starting,
            WaitingHit,
            AfterHit,
            Ending
        }
        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = true;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;
            Content.RootDirectory = "Content";
            livesUsed.scale = 3.5f;
            livesUsed.position = new Vector2(506, 1060);
            /*getHighScore = ghs;
            setHighScore = shs;
            showBannerAd = sba;
            hideBannerAd = hba;
            showInterstitialAd = sia;*/
        }

        protected override void Initialize()
        {
            
            base.Initialize();
            life = new Life(NativeFunctions.getLife(),this);
            highScore.score = NativeFunctions.getHighScore();
            makePauseButton();
            
           // highScore.score = 102;
        }

        private void makePauseButton()
        {
            GUIElements.Add(new Button("",
                new Vector2(22.5f, 1257.5f),
                4,
                Color.White,
                    Pause
                ,
                0,
                3));
        }
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Resources.LoadContent(Content, GraphicsDevice);
            renderScale = new Vector2((float)GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / (float)GameVars.RENDERWIDTH, (float)GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / (float)GameVars.RENDERHEIGHT);
            renderBuffer = new RenderTarget2D(GraphicsDevice, GameVars.RENDERWIDTH,
                GameVars.RENDERHEIGHT, false, SurfaceFormat.Color, DepthFormat.None);
            Jack.LoadContent(GraphicsDevice);
           backdrop  = new Backdrop();
            constructMainMenu();
            
        }
        protected void processTouch()
        {
            if (receivingInput)
            {
                TouchCollection touchCollection;
                touchCollection = TouchPanel.GetState();
                if (touchCollection.Count > 0) //Received Touch
                {
                    TouchLocation touchLocation = touchCollection[0];
                    if (touchLocation.State == TouchLocationState.Pressed)
                    {
                        Vector2 coords = touchLocation.Position / renderScale;
                        foreach (GUIElement b in GUIElements)
                        {

                            if (b.GetType().Equals(typeof(Button))) 
                            {
                                if (paused && ((Button)b).Caption != "paused") continue;
                                if (((Button)b).isHit(coords, this))
                                {
                                    return;
                                }
                            }
                        }
                        if (paused) return;
                        switch (appState)
                        {
                            case AppState.InGame:
                                switch (gameState)
                                {
                                    case GameState.WaitingHit:
                                        //debugList.Clear();
                                        Resources.shotSound.Play(0.6f,0,0);
                                        bullet = new Bullet(coords);
                                        int hitCount = 0;

                                        for (int i = Projectiles.Count - 1; i >= 0; i--)
                                        {
                                            if (bullet.checkHit(Projectiles[i])) { Projectiles[i].ShatterAndDestroy(this); hitCount++; }
                                        }
                                        //Check how many is hit
                                        score.add(hitCount);
                                        if (hitCount == Projectiles.Count)
                                        { Timers.Add(new TimerAction(GameVars.roundInterval, delegate { gameState = GameState.Starting; }));
                                            Resources.explodeSound.Play(1, (float)(GameVars.random.NextDouble() - 0.5f), 0);
                                            Resources.dingSound.Play();
                                        }
                                        else
                                        {
                                            Resources.explodeSound.Play(1, (float)(GameVars.random.NextDouble() - 0.5f), 0);
                                            losing = true;
                                        }
                                        checkAchievement();
                                        gameState = GameState.AfterHit;
                                        break;
                                    case GameState.AfterHit:
                                        if (losing)
                                        {
                                            foreach (Projectile p in Projectiles)
                                            {
                                                p.ShatterAndDestroy(this);
                                            }
                                            gameState = GameState.Ending;
                                            
                                            endGame();
                                            losing = false;
                                        }
                                        break;
                                }
                                break;


                        }

                    }
                }
            }
        }
        protected void checkAchievement()
        {
            if (currentLevel == 1) { NativeFunctions.unlockAchievement(Achievement.BabyStep); }
            else if (score.score >= 50) { NativeFunctions.unlockAchievement(Achievement._50Points); }
            else if (score.score >= 100) { NativeFunctions.unlockAchievement(Achievement._100Points); }
            else if (score.score >= 70 && livesUsed.score == 0) { NativeFunctions.unlockAchievement(Achievement._70Points1Life); }
            else if (currentLevel == 2) { NativeFunctions.unlockAchievement(Achievement.OutOfTheEarth); }
            else if (currentLevel == 7) { NativeFunctions.unlockAchievement(Achievement.Interstellar); }
            else if (currentLevel == 9) { NativeFunctions.unlockAchievement(Achievement.Doge); }

        }
        protected void endGame()
        {
            bullet = null;
            appState = AppState.Results;
            if (score.score > highScore.score)
            {
                highScore.score = score.score;
                NativeFunctions.setHighScore(score.score);
                NativeFunctions.submitHighScore(score.score);
                constructResults(true);
            }
            else
            {
                constructResults(false);
            }
            consecutivePlays++;
            
        }
        protected void constructCredits()
        {
            if (!creditinterstitial) { NativeFunctions.showInterstitialAds(); creditinterstitial = true; }
            
            GUIElements.Add(new GUISprite(new Vector2(GameVars.RENDERWIDTH / 2, GameVars.RENDERHEIGHT / 2), 1, Color.White, GameVars.fullGUIRatio / 4, 4));
            GUIElements.Add(new Button("",
                new Vector2(609,1150),
                2,
                Color.White,
                delegate
                {
                    appState = AppState.Menu;
                    gameState = GameState.AfterHit;
                    foreach (GUIElement ge in GUIElements)
                    {
                        ge.fadeOut(0.6f);
                    }
                    makePauseButton();
                    constructMainMenu();
                },
                4,
                10
            ));
        }
		protected void constructMainMenu()
		{
            //big play button
            GUIElements.Add(new GUISprite(new Vector2(GameVars.RENDERWIDTH / 2, GameVars.RENDERHEIGHT / 2), 0, Color.White, GameVars.fullGUIRatio, 4));
			GUIElements.Add(new Button("",
				new Vector2(GameVars.RENDERWIDTH / 2, 580),
				0,
				Color.White,
				delegate
				{
                    
                    appState = AppState.InGame;
					gameState = GameState.AfterHit;
                    score.position = new Vector2(GameVars.RENDERWIDTH / 2, 200);
                    score.scale = 4.9f;
                    score.score = 0;
                    Timers.Add(new TimerAction(0.7f,delegate { gameState = GameState.Starting; }));
                    foreach (GUIElement ge in GUIElements)
                    {
                        ge.fadeOut(0.6f);
                    }
                    makePauseButton();
                    jack.Start();

                },
                4,
                15.5f
            ));
            GUIElements.Add(new Button("",
                new Vector2(GameVars.RENDERWIDTH * 0.35f, 795),
                1,
                Color.White,
                delegate
                {
                    appState = AppState.Credits;
                    foreach (GUIElement ge in GUIElements)
                    {
                        ge.fadeOut(0.6f);
                    }
                    makePauseButton();
                    constructCredits();
                },
                4,
                9
            ));
            GUIElements.Add(new Button("",
                new Vector2(GameVars.RENDERWIDTH * 0.65f, 795),
                5,
                Color.White,
                delegate
                {
                    NativeFunctions.submitHighScore(highScore.score);
                    NativeFunctions.showLeaderboard();
                    //NativeFunctions.unlockAchievement(Achievement.BabyStep);
                },
                4,
                9,true
            ));
            highScore.position = new Vector2(GameVars.RENDERWIDTH / 2, 1038);
            highScore.scale = 4.9f;
            currentLevel = 0;
            currentRound = 0;
            backdrop.restart();
            jackwander();
        }
        private void jackwander()
        {
            if (appState != AppState.InGame)
            jack.WalkTowards(GameVars.RENDERWIDTH - 100, () => { jack.WalkTowards(0, jackwander); });
        }
        protected void Pause()
        {
            GUIElements.Add(new Button("paused",
                new Vector2(22.5f, 1257.5f),
                0,
                Color.White,
                    Resume
                ,
                0,
                3));
            GUIElements.Add(new GUISprite(Vector2.Zero, GameVars.GUITextureNumber, Color.White * 0.45f, 1280, 4));
            
            paused = true;
        }
        protected void Resume()
        {
            GUIElements[GUIElements.Count - 1].fadeOut(0.5f);

            makePauseButton();
            paused = false;
        }
        protected void constructResults(bool hs)
        {
            Resources.loseSound.Play();
            GUIElements.Add(new GUISprite(new Vector2(GameVars.RENDERWIDTH / 2, GameVars.RENDERHEIGHT / 2), (byte)(hs? 3: 2), Color.White, GameVars.fullGUIRatio, 4));
            GUIElements.Add(new GUISprite(new Vector2(GameVars.RENDERWIDTH / 2, 800), (byte)(100 + currentLevel), Color.White, 4.8f, 4));
            GUIElements.Add(new GUISprite(new Vector2(GameVars.RENDERWIDTH / 2, GameVars.RENDERHEIGHT / 2), 4, Color.White, GameVars.fullGUIRatio, 4));
            GUIElements.Add(new Button("",
                new Vector2(life.lives > 0 ? 193 : 279,920),
                0,
                Color.White,
                delegate
                {
                    appState = AppState.InGame;
                    gameState = GameState.AfterHit;
                    backdrop.restart();
                    score.position = new Vector2(GameVars.RENDERWIDTH / 2, 200);
                    score.scale = 4.9f;
                    score.score = 0;
                    Timers.Add(new TimerAction(1.5f, delegate { gameState = GameState.Starting; }));
                    foreach(GUIElement ge in GUIElements)
                    {
                        ge.fadeOut(0.6f);

                    }
                    makePauseButton();
                    jack.Start();
                    livesUsed.score = 0;
                    currentLevel = 0;
                    currentRound = 0;
                },
                4,
                9.1f
            ));
            GUIElements.Add(new Button("",
                new Vector2(life.lives > 0 ? 531 : 441, 920),
                2,
                Color.White,
                delegate
                {
                    appState = AppState.Menu;
                    gameState = GameState.AfterHit;
                    foreach (GUIElement ge in GUIElements)
                    {
                        ge.fadeOut(0.6f);
                    }
                    makePauseButton();
                    jack.Start();
                    constructMainMenu();
                    livesUsed.score = 0;
                    currentLevel = 0;
                    currentRound = 0;
                },
                4,
                9.1f
            ));

            //TODO: implement lives - continue
            if (life.lives > 0)
            GUIElements.Add(new Button("",
				new Vector2(362, 920),
				3,
				Color.White,
				delegate
				{
					appState = AppState.InGame;
					gameState = GameState.AfterHit;
					score.position = new Vector2(GameVars.RENDERWIDTH / 2, 200);
					score.scale = 4.9f;
					Timers.Add(new TimerAction(1.5f, delegate { gameState = GameState.Starting; }));
					foreach (GUIElement ge in GUIElements)
					{
						ge.fadeOut(0.6f);

					}
                    makePauseButton();
                    jack.Start();
					life.lives--;
                    livesUsed.add(1);
				},
				4,
				9.1f
			));

            if (hs)
            {
                highScore.position = new Vector2(GameVars.RENDERWIDTH / 2, 609);
                highScore.scale = 5.13f;
                score.scale = 0;
            }
            else
            {
                score.position = new Vector2(514, 609);
                score.scale = 5.13f;
                highScore.position = new Vector2(229, 609);
                highScore.scale = 5.13f;
            }
            
            
            
            jack.Result();
            if (consecutivePlays % 15 == 0)
            {
                NativeFunctions.showInterstitialAds();
            }
        }
        protected void moveJackToProj()
        {
            if (gameState == GameState.Ending) return;
            Projectile temp = null;
            float temppos = 0;
            foreach(Projectile p in Projectiles)
            {
                if (p.velocity.Y > 0)
                {
                    if (p.position.Y > temppos)
                    {
                        temp = p;
                        temppos = p.position.Y;
                    }
                }
            }
            if (temp != null)
            {
                jack.WalkTowards(temp.position.X);
                if (temppos > GameVars.RENDERHEIGHT - 100)
                {
                    gameState = GameState.Ending;
                    Resources.eatSound.Play();
                    jack.animate(Jack.Animation.Eat, delegate { endGame(); });
                    temp.ShatterAndDestroy(this);
                }
            }
            else
            {
                jack.animate(Jack.Animation.Stop);
            }
        }
        protected override void Update(GameTime gameTime)
        {

            
            
            foreach (TimerAction ta in Timers) { ta.Update(gameTime); }
            foreach (TimerAction ta in Timers) { if (ta.disposable) Timers.Remove(ta); break; }
            //Update all buttons
			for (int i = GUIElements.Count - 1; i >= 0; i--)
            {
				GUIElements[i].Update(gameTime);
				if (GUIElements[i].disposable) {
					GUIElements.RemoveAt(i);
                }
            }
            processTouch();
            //Debug Lists
/*
            debugList.Clear();
            debugList.Add("Debris Count: " + Debris.Count);
            debugList.Add("Projectiles Count: " + Projectiles.Count);
            debugList.Add("Current Level: " + currentLevel);
            debugList.Add("Current Round: " + currentRound);
            debugList.Add("Score: " + score.score);
            debugList.Add("Jack Frame: " + jack.currentFrame);
            debugList.Add("AppState: " + appState.ToString());
*/
           
            //Update entities
            for (int i = Debris.Count - 1; i >= 0; i--)
            {
                Debris[i].Update(gameTime);
                if (Debris[i].disposable) { Debris.RemoveAt(i); }
            }
            if (paused) return;
            backdrop.Update(gameTime);
            for (int i = Projectiles.Count - 1; i >= 0; i--)
            {
                if (Projectiles[i].disposable) { Projectiles.RemoveAt(i); }
            }
            foreach (Projectile pj in Projectiles) pj.Update(gameTime);
            if (bullet != null) bullet.Update(gameTime);
            
            jack.Update(gameTime);
            life.Update(gameTime);
            switch (appState)
            {
                case AppState.Menu:
                    
                    break;
                case AppState.InGame:
                    moveJackToProj();

                    switch (gameState)
                    {
                        case GameState.Starting:
                            //Resetting Scene
                            Projectiles.Clear();
                            losing = false;
                            bullet = null;
                            for (byte i = 0; i < GameVars.maxLevel; i++)
                            {
                                if (GameVars.roundsAtLevel[i] == currentRound)
                                {
                                    currentLevel = i;
                                    backdrop.startSlideTo(currentLevel,0.5f,delegate { });
                                }
                            }

                            
                            Vector2 targetCoords = new Vector2(rand.Next((int)(GameVars.RENDERWIDTH * .1), (int)(GameVars.RENDERWIDTH * .9)), rand.Next((int)(GameVars.RENDERHEIGHT * .2), (int)(GameVars.RENDERHEIGHT * .7)));
                            float arrivalTime = GameVars.projectilesInterval * (GameVars.projectilesNumber[currentLevel] - 1) + (float)(rand.NextDouble() + 1) + GameVars.arrivalTimeLag;
							int t = 0;
                            //play sound
                            float temptime = arrivalTime * 0.3f / GameVars.projectilesNumber[currentLevel];
                            Resources.throwSound.Play(1, (float)(GameVars.random.NextDouble() - 0.5f), 0);
                            for (int i = 1; i < GameVars.projectilesNumber[currentLevel];i++)
                            {
                                Timers.Add(new TimerAction(temptime * i, delegate { Resources.throwSound.Play(1, (float)(GameVars.random.NextDouble() - 0.5f), 0); }));
                            }

                            if (rand.Next(18) == 5) //Heart Projectile
							{
								t++;
								Projectiles.Add(newProjectileInstance(255, 0, targetCoords, arrivalTime));
								Projectiles[0].SetInitialPosition();
								Projectiles[0].Initialize();
							}
							for (; t < GameVars.projectilesNumber[currentLevel]; t++)
                            {
								
                                Projectiles.Add(newProjectileInstance((byte)rand.Next(GameVars.projectilesTypeNumbers[currentLevel]),t * GameVars.projectilesInterval,targetCoords, arrivalTime));
                                Projectiles[t].SetInitialPosition();
                                Projectiles[t].Initialize();
                                
                            }
                            
                            currentRound++;
                            gameState = GameState.WaitingHit;
                            break;
                        case GameState.WaitingHit:
                            
                            
                            
                            break;
                        case GameState.AfterHit:

                            break;
                    }


                    //Update All Entities
                    

                    break;
                case AppState.Results:
                    
                    break;
                case AppState.Credits:
                    break;
            }
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderBuffer);
            GraphicsDevice.Clear(Color.Transparent);
            //Draw Buffer1 (Pixelated)
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            backdrop.Draw(spriteBatch);
            jack.Draw(spriteBatch);
            foreach (DebrisEntity d in Debris)
                d.Draw(spriteBatch);
            foreach (Projectile pj in Projectiles)
                pj.Draw(spriteBatch);
			foreach (GUIElement b in GUIElements)
                b.Draw(spriteBatch);
            if (bullet != null)
            {
                bullet.Draw(spriteBatch);
            }
            life.Draw(spriteBatch);
            switch (appState)
            {
                case AppState.InGame:
                    score.Draw(spriteBatch);
                    break;
                case AppState.Credits:

                    break;
                case AppState.Menu:
                    highScore.Draw(spriteBatch);
                    break;
                case AppState.Results:
                    score.Draw(spriteBatch);
                    highScore.Draw(spriteBatch);
                    livesUsed.Draw(spriteBatch);
                    break;
            }


            //Draw Debug Info
/*
            for (int i = 0; i < debugList.Count; i++)
            {
                spriteBatch.DrawString(Resources.debugFont, debugList[i], new Vector2(0, i * 15), Color.White);
            }
*/
            spriteBatch.End();
            //Draw BackBuffer
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(renderBuffer,
                new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth / 2, GraphicsDevice.PresentationParameters.BackBufferHeight / 2),
                null, Color.White, 0f, new Vector2(GameVars.RENDERWIDTH / 2, GameVars.RENDERHEIGHT / 2), renderScale,
                SpriteEffects.None, 0f);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        private Projectile newProjectileInstance(byte _ID, float _lagtime, Vector2 _target, float _arrivaltime)
        {
            
            switch (_ID)
            {
                case 0:
                    return new PrDebug(_lagtime, _target, _arrivaltime);
                case 1:
                    return new Pr1(_lagtime, _target, _arrivaltime);
                case 2:
                    return new Pr2(_lagtime, _target, _arrivaltime);
                case 3:
                    return new Pr3(_lagtime, _target, _arrivaltime);
                case 4:
                    return new Pr4(_lagtime, _target, _arrivaltime);
                case 5:
                    return new Pr5(_lagtime, _target, _arrivaltime);
                case 6:
                    return new Pr6(_lagtime, _target, _arrivaltime);
                case 7:
                    return new Pr7(_lagtime, _target, _arrivaltime);
                case 8:
                    return new Pr8(_lagtime, _target, _arrivaltime);
                case 9:
                    return new Pr9(_lagtime, _target, _arrivaltime);
				case 255:
					return new PrHeart(_lagtime, _target, _arrivaltime, this);
                default:
                    return new PrDebug(_lagtime, _target, _arrivaltime);
            }
            
        }
    }
}
