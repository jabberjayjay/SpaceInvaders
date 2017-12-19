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
using Windows.UI;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MySpaceInvaders
{
    // Initial shooter code taken from https://blogs.windows.com/buildingapps/2015/03/13/how-to-make-a-windows-store-game-with-c-and-xaml-part-1/
    // Added movement in 2 dimensions instead of just left and right, added up and down
    // added difficulty and menu items and emitters for stars

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartPage : Page
    {

        // amount of stars on main menu page
        private const int StarCount = 1000;
        private List<Dot> stars = new List<Dot>(StarCount);

        // random number generator to make stars emitter seem random
        private Random randomizer = new Random();
       
        // sets up local app data storage
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

       


        public StartPage()
        {
            // set up local storage for High score and difficulty setting
            localSettings.Values["difficulty"] = "easy";

            // adds high score to start page 



            
            this.InitializeComponent();
            HighScoreBlock.Text = App.Highscore.ToString();
            Loaded += (sender, args) =>
            {
                CreateStar();
                Move.Completed += MoveStars;
                Move.Begin();
            };

#if WINDOWS_PHONE_APP
            App.ScreenWidth = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Bounds.Width;
            App.ScreenHeight = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Bounds.Height;
#endif
#if WINDOWS_APP
            
            App.ScreenWidth = Window.Current.Bounds.Width;
            App.ScreenHeight = Window.Current.Bounds.Height;
#endif
        }

        private void CreateStar()
        {

            
            var star = new Dot()
            {
                Shape = new Ellipse() { Height = 2, Width = 2 },
                Velocity = new Point(randomizer.Next(-5, 5), randomizer.Next(-5, 5))
            };

            // Center the star
            Canvas.SetLeft(star.Shape, LayoutRoot.ActualWidth / 2 - star.Shape.Width / 2);
            Canvas.SetTop(star.Shape, (LayoutRoot.ActualHeight / 2 - star.Shape.Height / 2) + 20);

            // Prevent stars getting stuck
            if ((int)star.Velocity.X == 0 && (int)star.Velocity.Y == 0)
            {
                star.Velocity = new Point(randomizer.Next(1, 5), randomizer.Next(1, 5));
            }

            // Set color
            var colors = new byte[4];
            randomizer.NextBytes(colors);
            star.Shape.Fill = new SolidColorBrush(Color.FromArgb(colors[0], colors[1], colors[2], colors[3]));

            stars.Add(star);
            LayoutRoot.Children.Add(star.Shape);

        }

        void MoveStars(object sender, object e)
        {
            if (stars.Count < StarCount)
            {
                CreateStar();
            }

            foreach (var star in stars)
            {
                double left = Canvas.GetLeft(star.Shape) + star.Velocity.X;
                double top = Canvas.GetTop(star.Shape) + star.Velocity.Y;

                Canvas.SetLeft(star.Shape, left);
                Canvas.SetTop(star.Shape, top);

                // Star is off the screen
                if ((int)left < 0 ||
                    (int)left > LayoutRoot.ActualWidth ||
                    (int)top < 0 ||
                    (int)top > LayoutRoot.ActualHeight)
                {
                    Canvas.SetLeft(star.Shape, LayoutRoot.ActualWidth / 2 - star.Shape.Width / 2);
                    Canvas.SetTop(star.Shape, (LayoutRoot.ActualHeight / 2 - star.Shape.Height / 2) + 20);
                }
            }
            Move.Begin();
        }


        private void OnStart(object sender, RoutedEventArgs e)
        {
            // goes to game app page to play
            this.Frame.Navigate(typeof(GamePage));
        }

        private void QUIT_GAME_Click(object sender, RoutedEventArgs e)
        {
            // quits app when quit button pressed
            App.Current.Exit();
        }



        private void Difficulty_Click(object sender, RoutedEventArgs e)
        {
            
            // selects difficulty and changes displayed difficulty as the options are cycled through
            // also passes a 4 character string to local storage so that difficulty can be read by game page to make more enemies
            

            switch (Difficulty.Content.ToString())
            {
                case "DIFFICULTY - EASY":
                    //imgOne.Visibility = Visibility.Collapsed;
                    Difficulty.Content = "DIFFICULTY - MEDIUM";
                    localSettings.Values["difficulty"] = "medi";
                    break;
                case "DIFFICULTY - MEDIUM":
                   // imgOne.Visibility = Visibility.Visible;
                   Difficulty.Content = "DIFFICULTY - HARD";
                    localSettings.Values["difficulty"] = "hard";
                    break;
                case "DIFFICULTY - HARD":
                    //imgThree.Visibility = Visibility.Visible;
                    Difficulty.Content = "DIFFICULTY - EASY";
                    localSettings.Values["difficulty"] = "easy";
                    break;

                default:
                    break;
            }
        }
    }
}
