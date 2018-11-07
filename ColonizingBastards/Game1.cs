using ColonizingBastards.Base.Director;
using ColonizingBastards.Base.LogicUtil;
using ColonizingBastards.Base.PlayerInteraction;
using ColonizingBastards.Base.Scene;
using ColonizingBastards.Base.ScreenUtil;
using ColonizingBastards.GameClasses;
using ColonizingBastards.GameClasses.Match;
using ColonizingBastards.GameClasses.Match.MatchObjects;
using ColonizingBastards.GameClasses.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

		private Director director;
        private Scene scene;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            scene = new Scene();

			director = new Director(graphics, Content);

            /*Logic matchLogic = new MatchLogic(director, scene, new[] { false });
            matchLogic.Initialize();
			Screen matchScreen = new MatchScreen(director, scene, matchLogic.BaseScreenSize, new []{false});

            director.AddLogic(matchLogic);
            director.AddScreen(matchScreen);*/

            Logic menuLogic = new MenuLogic(director, scene);
            menuLogic.Initialize();
            Screen menuScreen = new DefaultScreen(director, scene, menuLogic.BaseScreenSize);

            director.AddLogic(menuLogic);
			director.AddScreen(menuScreen);



            // Player MatchPlayer = new Player(PlayerIndex.One, new MatchInputMapper());

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
			// TODO: Add your update logic here
            base.Update(gameTime);

            director.Update(gameTime);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
            base.Draw(gameTime);

            director.Draw();

        }
    }
}
