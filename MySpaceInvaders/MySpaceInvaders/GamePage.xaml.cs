using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml.Shapes;



// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MySpaceInvaders
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        
        
        // game over, restart button text
        private TextBlock GameOver = new TextBlock();
        private Button restart = new Button();
        private TextBlock difficultyText = new TextBlock();

        // starfield member variables
        private const int StarCount = 200;
        private List<Dot> stars = new List<Dot>(StarCount);
        private Random randomizer = new Random();

        private double shipPosition;
        private double shipHorizontalPosition = Window.Current.Bounds.Height -100;

        private bool goingLeft = false, goingRight = false, goingUp = false, goingDown = false;

        private List<Goomba> enemies = new List<Goomba>();
        
        private int maxEnemies = 20;
        private DispatcherTimer timer = new DispatcherTimer();
        private int Level { get; set; } // Player level 
        private int Score { get; set; } // Game score

        // list laser bullet ellipses for shooting enemies
        private List<Ellipse> bullets = new List<Ellipse>(); // Bullets on the screen
        private bool gameRunning = true; // Did we die already

       

       


        public GamePage()
        {
               
            // reads difficulty
            // was unable to get difficulty text to update on game page
            object value = localSettings.Values["difficulty"];
            switch (value.ToString())
            {
                case "easy":
                    //difficultyDisplay.Text = "DIFFICULTY - EASY";
                    break;
                case "medi":
                    maxEnemies = 40;
                    //difficultyDisplay.Text = "DIFFICULTY - MEDIUM";
                    break;
                case "hard":
                    maxEnemies = 60;
                    //difficultyDisplay.Text = "DIFFICULTY - HARD";
                    break;
            }
            
            this.InitializeComponent();

            // capture keyboard controls if a PC is being used to play game
            Loaded += (sender, args) =>
            {
                // Resize move controls to fit the area
                LeftCanvas.Width = LeftCanvas.Height = (LeftArea.ActualWidth / 2) - 10;
                RightCanvas.Width = RightCanvas.Height = (LeftArea.ActualWidth / 2) - 10;

                // Position the ship to the bottom center of the screen
                shipPosition = LayoutRoot.ActualWidth / 2;
                Rocket.Margin = new Thickness(shipPosition, shipHorizontalPosition, 20, 20);

                Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

                // Starfield background
                CreateStar();
                Move.Completed += MoveStars;
                Move.Begin();

                // adding timer
                timer.Tick += TimerOnTick;
                timer.Interval = new TimeSpan(0, 0, 0, 2);
                timer.Start();

            };
            CompositionTarget.Rendering += GameLoop;


#if WINDOWS_PHONE_APP
            MiddleArea.Width = new GridLength(6, GridUnitType.Star);
            Style style = new Style(typeof(TextBlock));
            style.Setters.Add(new Setter(TextBlock.FontSizeProperty, 24));
            style.Setters.Add(new Setter(VerticalAlignmentProperty, VerticalAlignment.Top));
            style.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Left));
            Resources.Add(typeof(TextBlock), style);
            HighscoreBoard.Margin = new Thickness(5, 22, 0, 0);
            ScoreTitle.Margin = new Thickness(5, 44, 0, 0);
            ScoreBoard.Margin = new Thickness(5, 73, 0, 0);
#else
            MiddleArea.Width = new GridLength(12, GridUnitType.Star);
            Style style = new Style(typeof(TextBlock));
            style.Setters.Add(new Setter(TextBlock.FontSizeProperty, 32));
            style.Setters.Add(new Setter(VerticalAlignmentProperty, VerticalAlignment.Top));
            style.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Left));
            Resources.Add(typeof(TextBlock), style);
            HighscoreBoard.Margin = new Thickness(5, 32, 0, 0);
            
            ScoreTitle.Margin = new Thickness(5, 64, 0, 0);
            ScoreBoard.Margin = new Thickness(5, 96, 0, 0);
#endif
        }

        
        // Create a new enemy if not max amount on the screen already
       
        private void TimerOnTick(object sender, object o)
        {
            if (enemies.Count < maxEnemies)
            {
                var enemy = new Goomba
                {
                    AreaWidth = (int)LayoutRoot.ActualWidth,
                    Location = new Point(randomizer.Next(0, (int)LayoutRoot.ActualWidth - 80), 0)
                };
                if (enemy.Type == 3)
                {
                    // Make the red enemy smaller and more difficult to hit
                    var scaleTransform = new ScaleTransform();
                    scaleTransform.ScaleX = scaleTransform.ScaleX * 0.50;
                    scaleTransform.ScaleY = scaleTransform.ScaleY * 0.50;
                    enemy.RenderTransform = scaleTransform;
                    enemy.Width = 30;
                    enemy.Height = 30;
                }
                enemy.Velocity = enemy.Velocity * ((Level / (double)10) + 1);
                enemies.Add(enemy);
                Canvas.SetZIndex(enemy, 7);
                LayoutRoot.Children.Add(enemy);
            }
            if (maxEnemies > 20)
            {
                var enemy = new Goomba
                {
                    AreaWidth = (int)LayoutRoot.ActualWidth,
                    Location = new Point(randomizer.Next(0, (int)LayoutRoot.ActualWidth - 110), 0)
                };
                if (enemy.Type == 3)
                {
                    // makes more enemeis spawn when there is less than five
                    // Make the red enemy smaller and more difficult to hit
                    var scaleTransform = new ScaleTransform();
                    scaleTransform.ScaleX = scaleTransform.ScaleX * 0.70;
                    scaleTransform.ScaleY = scaleTransform.ScaleY * 0.70;
                    enemy.RenderTransform = scaleTransform;
                    enemy.Width = 30;
                    enemy.Height = 30;
                }
                enemy.Velocity = enemy.Velocity * ((Level / (double)10) + 1.2);
                enemies.Add(enemy);
                Canvas.SetZIndex(enemy, 7);
                LayoutRoot.Children.Add(enemy);
            }
        }


        private void GameLoop(object sender, object e)
        {
            // sets speed of movement by denoting the number of pixels that the ship will travel in a given direction
            if (goingRight)
                MoveShip(5,0);
            if (goingLeft)
                MoveShip(-5, 0);
            if (goingUp)
                MoveShip(0, -5);
            if (goingDown)
                MoveShip(0, 5);


            // collision test
            CrashTest();

            for (int i = 0; i < bullets.Count; i++)
            {
                MoveBullet(bullets[i]);
            }
            // moves bullets

            // Move enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].Dead == false)
                {
                    enemies[i].Move();
                    enemies[i].Margin = new Thickness(enemies[i].Location.X, enemies[i].Location.Y, 0, 0);
                }
                // had to change this if statement condition of app height as it does not seem to work as intended
                // changed to window bounds
                if (enemies[i].Margin.Top > Window.Current.Bounds.Height-50 || enemies[i].Dead)
                {
                    LayoutRoot.Children.Remove(enemies[i]);
                    enemies.Remove(enemies[i]);
                }
            }
        }
        // After that we need to add the event handlers under the constructor:

        private void ToLeftPressed(object sender, PointerRoutedEventArgs e)
        {
            goingLeft = true;
        }

        private void ToRightPressed(object sender, PointerRoutedEventArgs e)
        {
            goingRight = true;
        }

        private void ToLeftReleased(object sender, PointerRoutedEventArgs e)
        {
            goingLeft = false;
        }

        private void ToLeftExited(object sender, PointerRoutedEventArgs e)
        {
            goingLeft = false;
        }
        private void ToRightReleased(object sender, PointerRoutedEventArgs e)
        {
            goingRight = false;
        }

        private void ToRightExited(object sender, PointerRoutedEventArgs e)
        {
            goingRight = false;
        }

        private void ToUpPressed(object sender, PointerRoutedEventArgs e)
        {
            goingUp = true;
        }

        private void ToDownPressed(object sender, PointerRoutedEventArgs e)
        {
            goingDown = true;
        }

        private void ToUpReleased(object sender, PointerRoutedEventArgs e)
        {
            goingUp = false;
        }

        private void ToUpExited(object sender, PointerRoutedEventArgs e)
        {
            goingUp = false;
        }
        private void ToDownReleased(object sender, PointerRoutedEventArgs e)
        {
            goingDown = false;
        }

        private void ToDownExited(object sender, PointerRoutedEventArgs e)
        {
            goingDown = false;
        }
        private void CrashTest()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                // Checks for collision then displays game over and exit button

                var enemyCreature = new Rect(enemies[i].Location.X, enemies[i].Location.Y, enemies[i].ActualWidth, enemies[i].ActualHeight);
                enemyCreature.Intersect(new Rect(Rocket.Margin.Left, Rocket.Margin.Top, Rocket.ActualWidth,
                    Rocket.Margin.Bottom));

                if (!enemyCreature.IsEmpty)
                {
                    CompositionTarget.Rendering -= GameLoop;
                    Move.Completed -= MoveStars;

                   
                    GameOver.Text = "GAME OVER";
                    GameOver.Foreground = new SolidColorBrush(Colors.White);
                    GameOver.FontSize = 48;
                    GameOver.VerticalAlignment = VerticalAlignment.Center;
                    GameOver.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetColumn(GameOver, 1);

                    
                 
                    ExitButton.Visibility = Visibility.Visible;
                    // removed game scren menu button due to crashing when trying to leave game screen
                   // MenuButton.Visibility = Visibility.Visible;
                    MainGrid.Children.Add(GameOver);
                    gameRunning = false;

                    if (App.Highscore < Score)
                    {
                        
                        App.Highscore = Score;
                    }
                }
            }
        }


        private void OnFire(object sender, TappedRoutedEventArgs e)
        {

            if (gameRunning)
            {
                var bullet = new Ellipse
                {
                    Width = 5,
                    Height = 5,
                    Fill = new SolidColorBrush(Colors.Red)
                };
                bullet.Margin = new Thickness(shipPosition + (Rocket.Width / 2) - (bullet.Width / 2),
                    shipHorizontalPosition + 2, 0, 0);
                LayoutRoot.Children.Add(bullet);
                bullets.Add(bullet);

           

            }
        }

        private void MoveBullet(Ellipse ellipse)
        {
            // Checks that bullets are still on screeen then removes them if they go out of bounds

            if ((ellipse.Margin.Top - 10) > 0)
            {
                ellipse.Margin = new Thickness(ellipse.Margin.Left, ellipse.Margin.Top - 10, 0, 0);
                HitTest(ellipse);
            }
            else
            {
                bullets.Remove(ellipse);
                LayoutRoot.Children.Remove(ellipse);
            }
        }

        // Checking if laser bullets hit enemies
        private void HitTest(Ellipse ellipse)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                var enemyInFire = new Rect(enemies[i].Location.X, enemies[i].Location.Y, enemies[i].ActualWidth, enemies[i].ActualHeight);
                if (enemyInFire.Contains(new Point(ellipse.Margin.Left, ellipse.Margin.Top)))
                {

                    // updating high score as you kill more enemies
                    Score += enemies[i].Worth;
                    ScoreBoard.Text = Score.ToString();
                    if (Score > App.Highscore)
                    {
                        App.Highscore = Score;
                        HighscoreBoard.Text = Score.ToString();

                    }
                    LayoutRoot.Children.Remove(ellipse);
                    bullets.Remove(ellipse);
                    enemies[i].Dead = true;
                    return;
                }
            }
        }

        // amended original code to take in X for horizontal and Y for vertical movement
        private void MoveShip(int amountX, int amountY)
        {
            shipPosition += amountX;
            shipHorizontalPosition += amountY;

            // makes sure that the ship stays on the screen
            if (shipPosition > LayoutRoot.ActualWidth - 30)
            {
                shipPosition = LayoutRoot.ActualWidth - 30;
            }
            else if (shipPosition < 0)
            {
                shipPosition = 0;
            }

            // Make sure ship stays within the bounds of movement for Vertical and horizontal movement
            if (shipHorizontalPosition > LayoutRoot.ActualHeight - 100)
            {
                shipHorizontalPosition = LayoutRoot.ActualHeight - 100;
            }
            else if(shipHorizontalPosition < 0.6*LayoutRoot.ActualHeight)
            {
                shipHorizontalPosition = 0.6 * LayoutRoot.ActualHeight;
            }
         
            // broken code from base game
            //shipHorizontalPosition = Window.Current.Bounds.Height - 100;
            Rocket.Margin = new Thickness(shipPosition, shipHorizontalPosition, 0, 0);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender == ExitButton)
            {
                // Closes game if exit button is pressed after ship dies
                //GamePage.Exit();
                if (App.Highscore < Score)
                {
                    App.Highscore = Score;
                }
                App.Current.Exit();
               

               
            }
        
        }


        // takes input from keyboard if the game is being played on a Windows PC

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch(args.VirtualKey)
            {
                case VirtualKey.Up:
                    MoveShip(0, -10);
                    break;
                case VirtualKey.Down:
                    MoveShip(0, 10);
                    break;
                case VirtualKey.Left:
                    MoveShip(-10,0);
                    break;
                case VirtualKey.Right:
                    MoveShip(10,0);
                    break;
                case VirtualKey.Space:
                    OnFire(null, null);
                    break;
                default:
                    break;
            }
        }

        void MoveStars(object sender, object e)
        {
            if (stars.Count < StarCount)
            {
                CreateStar();
            }

            foreach (Dot star in stars)
            {

                Canvas.SetLeft(star.Shape, Canvas.GetLeft(star.Shape) + star.Velocity.X);
                Canvas.SetTop(star.Shape, Canvas.GetTop(star.Shape) + star.Velocity.Y);

                if (Canvas.GetTop(star.Shape) > LayoutRoot.ActualHeight)
                {
                    int left = randomizer.Next(0, (int)LayoutRoot.ActualWidth);
                    Canvas.SetLeft(star.Shape, left);
                    Canvas.SetTop(star.Shape, 0);
                }
            }
            Move.Begin();
        }
        private void CreateStar()
        {
            var star = new Dot()
            {
                Shape = new Ellipse() { Height = 2, Width = 2 },
                Velocity = new Point(0, randomizer.Next(1, 5))
            };

            int left = randomizer.Next(0, (int)LayoutRoot.ActualWidth);
            Canvas.SetLeft(star.Shape, left);
            Canvas.SetTop(star.Shape, 0);
            Canvas.SetZIndex(star.Shape, 1);

            // Set color
            byte c = (byte)randomizer.Next(10, 255);
            star.Shape.Fill = new SolidColorBrush(Color.FromArgb(c, c, c, c));

            stars.Add(star);
            LayoutRoot.Children.Add(star.Shape);
        }
    }
}
