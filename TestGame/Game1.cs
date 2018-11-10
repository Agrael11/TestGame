using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace TestGame
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        System.Random randomizer = new System.Random();
        TileSet tileSet;
        SpriteFont font;

        Level currentLevel = new Level();
        Vector2 checkPoint;

        readonly float gameScale = 1;
        readonly int widthLevel = 87;
        readonly int heightLevel = 38;
        readonly int tileSize = 16;
        readonly int timerLimit = 50;
        readonly int maxTileHeight = 9;
        readonly int frameRate = 60;
        float aspect = 0;
        int miniMapZoom = 10;
        int gameSpeed = 5;

        int death = 0;
        float timer = 0;
        int playerMove = -1;
        int lives = 3;
        int ammo = 15;


        int scene = 0;
        int menuSelect = 0;

        bool mPressed = false;
        bool upPressed = false;
        bool downPressed = false;
        bool leftPressed = false;
        bool rightPressed = false;
        

        Entity bullet = new Entity("pBullet", 0, -1, 3, new Vector2(0,0), true);

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;

            aspect = graphics.PreferredBackBufferWidth;
            aspect /= graphics.PreferredBackBufferHeight;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            tileSet = new TileSet(Content.Load<Texture2D>("tileset"), tileSize);
            font = Content.Load<SpriteFont>("font");
            currentLevel = LoadLevel("Level1.xml");
            checkPoint = new Vector2(currentLevel.player.position.X, currentLevel.player.position.Y);
        }

        public Level LoadLevel(string levelName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Level));
            System.IO.StreamReader reader = new System.IO.StreamReader(levelName);
            Level level = (Level)serializer.Deserialize(reader);
            reader.Close();
            ammo = 15;
            return level;
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (scene == 0)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    upPressed = true;
                }
                else if (upPressed)
                {
                    if (menuSelect == 0) menuSelect = 2;
                    else menuSelect--;
                    upPressed = false;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    downPressed = true;
                }
                else if (downPressed)
                {
                    if (menuSelect == 2) menuSelect = 0;
                    else menuSelect++;
                    downPressed = false;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    leftPressed = true;
                }
                else if (leftPressed)
                {
                    if (menuSelect == 1)
                    {
                        if (gameSpeed > 1) gameSpeed--;
                    }
                    leftPressed = false;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    rightPressed = true;
                }
                else if (rightPressed)
                {
                    if (menuSelect == 1)
                    {
                        if (gameSpeed < 10) gameSpeed++;
                    }
                    rightPressed = false;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    mPressed = true;
                }
                else if (mPressed )
                {
                    mPressed = false;
                    if (menuSelect == 0)
                    {
                        currentLevel = LoadLevel("Level1.xml");
                        checkPoint = new Vector2(currentLevel.player.position.X, currentLevel.player.position.Y);
                        lives = 3;
                        scene = 1;
                    }
                    else if (menuSelect == 2)
                        this.Exit();
                }
            }
            else if (scene == 1)
            {
                if (death == 0)
                    timer += gameSpeed * (float)(gameTime.ElapsedGameTime.TotalSeconds / (1f / frameRate));
                else timer += gameSpeed * (float)(gameTime.ElapsedGameTime.TotalSeconds / (1f / frameRate)) * 0.1f;
                Vector2 playerNextPos = currentLevel.player.position;
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    playerMove = 3;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    playerMove = 1;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    playerMove = 2;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    playerMove = 0;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    scene = 0;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.M))
                {
                    mPressed = true;
                }
                else if (mPressed == true)
                {
                    mPressed = false;
                    switch (miniMapZoom)
                    {
                        case 5: miniMapZoom = 10; break;
                        case 10: miniMapZoom = 20; break;
                        case 20: miniMapZoom = 50; break;
                        case 50: miniMapZoom = 100; break;
                        case 100: miniMapZoom = 5; break;
                    }
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    if (ammo > 0 && bullet.direction == -1)
                    {
                        ammo--;
                        bullet.direction = currentLevel.player.direction;
                        bullet.position = new Vector2(currentLevel.player.position.X, currentLevel.player.position.Y);
                    }
                }

                if (timer >= timerLimit)
                {
                    timer = 0;
                    if (death == 0)
                    {
                        for (int i = currentLevel.horizontals.Count - 1; i >= 0; i--)
                        {
                            Entity horizontal = currentLevel.horizontals[i];
                            horizontal.moved = false;
                            Vector2 nextPos = new Vector2(horizontal.position.X, horizontal.position.Y);
                            switch (horizontal.direction)
                            {
                                case 0: nextPos.X++; break;
                                case 2: nextPos.X--; break;
                            }
                            if (CheckCollision(nextPos.X, nextPos.Y).collision)
                            {
                                horizontal.direction = 2 - horizontal.direction;
                            }
                            else
                            {
                                horizontal.moved = true;
                                horizontal.position = nextPos;
                            }
                            currentLevel.horizontals[i] = horizontal;
                        }
                        for (int i = 0; i < currentLevel.cannons.Count; i++)
                        {
                            Entity cannon = currentLevel.cannons[i];

                            cannon.health++;
                            if (cannon.health >= cannon.special)
                            {
                                cannon.health = 0;
                                Entity bullet = new Entity("bullet", 0, cannon.direction, 3, new Vector2(cannon.position.X, cannon.position.Y), true);
                                currentLevel.bullets.Add(bullet);
                            }

                            currentLevel.cannons[i] = cannon;
                        }
                        for (int i = currentLevel.bullets.Count - 1; i >= 0; i--)
                        {
                            Entity bullet = currentLevel.bullets[i];

                            Vector2 nextPos = new Vector2(bullet.position.X, bullet.position.Y);
                            switch (bullet.direction)
                            {
                                case 0: nextPos.X++; break;
                                case 1: nextPos.Y++; break;
                                case 2: nextPos.X--; break;
                                case 3: nextPos.Y--; break;
                            }

                            if (CheckCollision(nextPos.X, nextPos.Y).collision)
                            {
                                currentLevel.bullets.RemoveAt(i);
                            }
                            else
                            {
                                bullet.position = nextPos;
                                bullet.moved = true;
                                currentLevel.bullets[i] = bullet;
                            }
                        }
                        for (int i = 0; i < currentLevel.randoms.Count; i++)
                        {
                            Entity random = currentLevel.randoms[i];
                            random.moved = false;
                            int nextDir = randomizer.Next(0, 4);
                            for (int j = 0; j < 4; j++)
                            {
                                nextDir %= 4;

                                Vector2 nextPos = new Vector2(random.position.X, random.position.Y);
                                switch (nextDir)
                                {
                                    case 0: nextPos.X++; break;
                                    case 1: nextPos.Y++; break;
                                    case 2: nextPos.X--; break;
                                    case 3: nextPos.Y--; break;
                                }
                                if (!CheckCollision(nextPos.X, nextPos.Y).collision)
                                {
                                    random.moved = true;
                                    random.position = nextPos;
                                    random.direction = nextDir;
                                    break;
                                }
                                nextDir++;
                            }

                            currentLevel.randoms[i] = random;
                        }
                        currentLevel.player.moved = false;
                        if (playerMove != -1)
                        {
                            currentLevel.player.direction = playerMove;
                            currentLevel.player.moved = true;
                        }
                        switch (playerMove)
                        {
                            case 0: playerNextPos.X++; break;
                            case 1: playerNextPos.Y++; break;
                            case 2: playerNextPos.X--; break;
                            case 3: playerNextPos.Y--; break;
                        }
                        playerMove = -1;
                        for (int i = 0; i < currentLevel.boxes.Count; i++)
                        {
                            Entity box = currentLevel.boxes[i];
                            box.moved = false;
                            currentLevel.boxes[i] = box;
                        }
                        var playerCollision = CheckCollision(currentLevel.player.position.X, currentLevel.player.position.Y);
                        if (playerCollision.collision)
                        {
                            switch (playerCollision.collider.name)
                            {
                                case "random":
                                case "bullet":
                                case "horizontal":
                                    death = 1;
                                    return;

                            }
                        }
                        playerCollision = CheckCollision(playerNextPos.X, playerNextPos.Y);
                        if (playerCollision.collision)
                        {
                            switch (playerCollision.collider.name)
                            {
                                case "random":
                                case "bullet":
                                case "horizontal":
                                    death = 1;
                                    currentLevel.player.position = playerNextPos;
                                    return;
                                case "ammo":
                                    ammo += 5;
                                    currentLevel.ammos.Remove(playerCollision.collider);
                                    currentLevel.player.position = playerNextPos;
                                    break;
                                case "checkpoint":
                                    checkPoint = new Vector2(playerNextPos.X, playerNextPos.Y);
                                    currentLevel.player.position = playerNextPos;
                                    break;
                                case "life":
                                    lives++;
                                    currentLevel.lifes.Remove(playerCollision.collider);
                                    currentLevel.player.position = playerNextPos;
                                    break;
                                case "box":
                                    currentLevel.boxes.Remove(playerCollision.collider);
                                    playerCollision.collider.direction = currentLevel.player.direction;
                                    Vector2 nextPosBox = new Vector2(playerCollision.collider.position.X, playerCollision.collider.position.Y);
                                    switch (currentLevel.player.direction)
                                    {
                                        case 0: nextPosBox.X++; break;
                                        case 1: nextPosBox.Y++; break;
                                        case 2: nextPosBox.X--; break;
                                        case 3: nextPosBox.Y--; break;
                                    }
                                    playerCollision.collider.moved = false;
                                    currentLevel.player.moved = false;
                                    var boxCollider = CheckCollision(nextPosBox.X, nextPosBox.Y);
                                    if (boxCollider.collision)
                                    {
                                        if (boxCollider.collider.name == "bullet")
                                        {
                                            currentLevel.bullets.Remove(boxCollider.collider);
                                            playerCollision.collider.position = nextPosBox;
                                            playerCollision.collider.moved = true;
                                            currentLevel.player.moved = true;
                                            currentLevel.boxes.Add(playerCollision.collider);
                                            currentLevel.player.position = playerNextPos;
                                        }
                                        else
                                        {
                                            currentLevel.boxes.Add(playerCollision.collider);
                                        }
                                    }
                                    else
                                    {
                                        playerCollision.collider.position = nextPosBox;
                                        playerCollision.collider.moved = true;
                                        currentLevel.player.moved = true;
                                        currentLevel.boxes.Add(playerCollision.collider);
                                        currentLevel.player.position = playerNextPos;
                                    }
                                    break;
                                default:
                                    currentLevel.player.moved = false;
                                    break;
                            }
                        }
                        else
                        {
                            currentLevel.player.position = playerNextPos;
                        }
                        if (bullet.direction != -1)
                        {
                            Vector2 nextPosBullet = new Vector2(bullet.position.X, bullet.position.Y);
                            switch (bullet.direction)
                            {
                                case 0: nextPosBullet.X++; break;
                                case 1: nextPosBullet.Y++; break;
                                case 2: nextPosBullet.X--; break;
                                case 3: nextPosBullet.Y--; break;
                            }
                            playerCollision = CheckCollision(bullet.position.X, bullet.position.Y);
                            if (playerCollision.collision)
                            {
                                switch (playerCollision.collider.name)
                                {
                                    case "bullet":
                                        currentLevel.bullets.Remove(playerCollision.collider);
                                        break;
                                    case "horizontal":
                                        currentLevel.horizontals.Remove(playerCollision.collider);
                                        break;
                                    case "random":
                                        currentLevel.randoms.Remove(playerCollision.collider);
                                        break;
                                }
                                bullet.direction = -1;
                            }
                            playerCollision = CheckCollision(nextPosBullet.X, nextPosBullet.Y);
                            if (playerCollision.collision)
                            {
                                switch (playerCollision.collider.name)
                                {
                                    case "bullet":
                                        currentLevel.bullets.Remove(playerCollision.collider);
                                        break;
                                    case "horizontal":
                                        currentLevel.horizontals.Remove(playerCollision.collider);
                                        break;
                                    case "random":
                                        currentLevel.randoms.Remove(playerCollision.collider);
                                        break;
                                }
                                bullet.direction = -1;
                            }
                            else
                            {
                                bullet.moved = true;
                                bullet.position = nextPosBullet;
                            }
                        }
                    }
                    else
                    {
                        death = 0;
                        lives--;
                        currentLevel = LoadLevel("Level1.xml");
                        currentLevel.player.position = new Vector2(checkPoint.X, checkPoint.Y);
                        currentLevel.player.moved = false;
                        if (lives < 0)
                        {
                            scene = 0;
                        }
                    }
                }
            }
            base.Update(gameTime);
        }

        protected (bool collision, Entity collider) CheckCollision(float x, float y)
        {
            foreach (Entity e in currentLevel.walls)
            {
                if (e.position.X == x && e.position.Y == y)
                    return (true, e);
            }
            foreach (Entity e in currentLevel.boxes)
            {
                if (e.position.X == x && e.position.Y == y)
                    return (true, e);
            }
            foreach (Entity e in currentLevel.ammos)
            {
                if (e.position.X == x && e.position.Y == y)
                    return (true, e);
            }
            foreach (Entity e in currentLevel.lifes)
            {
                if (e.position.X == x && e.position.Y == y)
                    return (true, e);
            }
            foreach (Entity e in currentLevel.checkpoints)
            {
                if (e.position.X == x && e.position.Y == y)
                    return (true, e);
            }
            foreach (Entity e in currentLevel.cannons)
            {
                if (e.position.X == x && e.position.Y == y)
                    return (true, e);
            }
            foreach (Entity e in currentLevel.bullets)
            {
                if (e.position.X == x && e.position.Y == y)
                    return (true, e);
            }
            foreach (Entity e in currentLevel.horizontals)
            {
                if (e.position.X == x && e.position.Y == y)
                    return (true, e);
            }
            foreach (Entity e in currentLevel.randoms)
            {
                if (e.position.X == x && e.position.Y == y)
                    return (true, e);
            }

            return (false, Entity.GetZero());
        }

        protected override void Draw(GameTime gameTime)
        {
            if (scene == 0)
            {
                GraphicsDevice.Clear(new Color(5, 0, 5));
                float fontSize = graphics.PreferredBackBufferWidth / 320f;
                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);
                string str = "Start Game";
                spriteBatch.DrawString(font, str, new Vector2((graphics.PreferredBackBufferWidth - font.MeasureString(str).X * fontSize) / 2, (graphics.PreferredBackBufferHeight - 3 * font.MeasureString(str).Y * fontSize) / 2), (menuSelect == 0) ? Color.Green : Color.White, 0, new Vector2(0, 0), fontSize, SpriteEffects.None, 0);
                str = "Speed = " + gameSpeed; ;
                spriteBatch.DrawString(font, str, new Vector2((graphics.PreferredBackBufferWidth - font.MeasureString(str).X * fontSize) / 2, (graphics.PreferredBackBufferHeight - font.MeasureString(str).Y * fontSize) / 2), (menuSelect == 1) ? Color.Green : Color.White, 0, new Vector2(0, 0), fontSize, SpriteEffects.None, 0);
                str = "Exit";
                spriteBatch.DrawString(font, str, new Vector2((graphics.PreferredBackBufferWidth - font.MeasureString(str).X * fontSize) / 2, (graphics.PreferredBackBufferHeight + font.MeasureString(str).Y * fontSize) / 2), (menuSelect == 2) ? Color.Green : Color.White, 0, new Vector2(0, 0), fontSize, SpriteEffects.None, 0);
                spriteBatch.End();
                base.Draw(gameTime);
            }
            else if (scene == 1)
            {
                float maxTileWidth = maxTileHeight;
                maxTileWidth *= aspect;
                RenderTarget2D gameTarget = new RenderTarget2D(GraphicsDevice, (int)(maxTileWidth * tileSize), (int)(maxTileHeight * tileSize));
                RenderTarget2D mapTarget = new RenderTarget2D(GraphicsDevice, (int)(widthLevel * tileSize), (int)(heightLevel * tileSize));

                GraphicsDevice.SetRenderTarget(gameTarget);
                float anim = timer / (float)timerLimit;

                Vector2 camera = new Vector2(currentLevel.player.position.X - 8, currentLevel.player.position.Y - 4.5f);

                if (currentLevel.player.moved)
                {
                    switch (currentLevel.player.direction)
                    {
                        case 0:
                            camera.X += anim - 1;
                            break;
                        case 1:
                            camera.Y += anim - 1;
                            break;
                        case 2:
                            camera.X -= anim - 1;
                            break;
                        case 3:
                            camera.Y -= anim - 1;
                            break;
                    }
                }
                if (camera.X < 0) camera.X = 0;
                else if (camera.X > widthLevel - 16) camera.X = widthLevel - 16;
                if (camera.Y < 0) camera.Y = 0;
                else if (camera.Y > heightLevel - 9) camera.Y = heightLevel - 9;


                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);
                DrawGame(camera, anim, Color.White);
                spriteBatch.End();

                camera = new Vector2(0, 0);

                GraphicsDevice.SetRenderTarget(mapTarget);
                GraphicsDevice.Clear(new Color(5, 0, 5));

                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);

                DrawGame(camera, anim, Color.White);

                spriteBatch.DrawEntity(tileSet, currentLevel.player, camera, Color.White, gameScale, anim);

                spriteBatch.End();



                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(new Color(5, 0, 5));

                if (miniMapZoom != 100)
                {
                    spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);
                    spriteBatch.Draw(gameTarget, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
                    spriteBatch.End();

                    int miniWidth = (int)(graphics.PreferredBackBufferWidth / 100f * miniMapZoom);
                    int miniHeight = (int)(graphics.PreferredBackBufferWidth / 100f * miniMapZoom / widthLevel * heightLevel);

                    spriteBatch.Begin();
                    spriteBatch.Draw(mapTarget, new Rectangle(graphics.PreferredBackBufferWidth - miniWidth, graphics.PreferredBackBufferHeight - miniHeight, miniWidth, miniHeight), Color.White);
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);
                    float fontSize = graphics.PreferredBackBufferWidth / 320f;
                    spriteBatch.DrawString(font, "LIVES:" + lives, new Vector2(0, 0), Color.Red, 0, new Vector2(0, 0), fontSize, SpriteEffects.None, 0);
                    string str2 = "AMMO:" + ammo;
                    spriteBatch.DrawString(font, str2, new Vector2(graphics.PreferredBackBufferWidth - (font.MeasureString(str2).X * fontSize), 0), Color.Red, 0, new Vector2(0, 0), fontSize, SpriteEffects.None, 0);
                    spriteBatch.End();
                }
                else
                {
                    int miniWidth = (int)(graphics.PreferredBackBufferWidth / 100f * miniMapZoom);
                    int miniHeight = (int)(graphics.PreferredBackBufferWidth / 100f * miniMapZoom / widthLevel * heightLevel);

                    spriteBatch.Begin();
                    spriteBatch.Draw(mapTarget, new Rectangle(graphics.PreferredBackBufferWidth - miniWidth, (graphics.PreferredBackBufferHeight - miniHeight) / 2, miniWidth, miniHeight), Color.White);
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);
                    float fontSize = graphics.PreferredBackBufferWidth / 320f;
                    spriteBatch.DrawString(font, "LIVES:" + lives, new Vector2(0, 0), Color.Red, 0, new Vector2(0, 0), fontSize, SpriteEffects.None, 0);
                    string str2 = "AMMO:" + ammo;
                    spriteBatch.DrawString(font, str2, new Vector2(graphics.PreferredBackBufferWidth - (font.MeasureString(str2).X * fontSize), 0), Color.Red, 0, new Vector2(0, 0), fontSize, SpriteEffects.None, 0);
                    spriteBatch.End();
                }
                base.Draw(gameTime);
                mapTarget.Dispose();
                gameTarget.Dispose();
            }
        }

        public void DrawGame(Vector2 camera, float anim, Color color)
        {
            spriteBatch.BatchDrawEntity(tileSet, currentLevel.walls, camera, color, gameScale, anim);
            spriteBatch.BatchDrawEntity(tileSet, currentLevel.boxes, camera, color, gameScale, anim);
            spriteBatch.BatchDrawEntity(tileSet, currentLevel.ammos, camera, color, gameScale, anim);
            spriteBatch.BatchDrawEntity(tileSet, currentLevel.checkpoints, camera, color, gameScale, anim);
            spriteBatch.BatchDrawEntity(tileSet, currentLevel.lifes, camera, color, gameScale, anim);
            spriteBatch.BatchDrawEntity(tileSet, currentLevel.cannons, camera, color, gameScale, anim);
            spriteBatch.BatchDrawEntity(tileSet, currentLevel.bullets, camera, color, gameScale, anim);
            spriteBatch.BatchDrawEntity(tileSet, currentLevel.horizontals, camera, color, gameScale, anim);
            spriteBatch.BatchDrawEntity(tileSet, currentLevel.randoms, camera, color, gameScale, anim);
            if (bullet.direction != -1)
            {
                spriteBatch.DrawEntity(tileSet, bullet, camera, new Color((int)(color.R*0.3f), (int)(color.G*0.3f), color.B), gameScale, anim);
            }

            spriteBatch.DrawEntity(tileSet, currentLevel.player, camera, color, gameScale, anim);
        }

    }
}
