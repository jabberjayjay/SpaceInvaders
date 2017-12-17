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
        // starfield member variables
        private const int StarCount = 200;
        private List<Dot> stars = new List<Dot>(StarCount);
        private Random randomizer = new Random();

        private double shipPosition;
        private readonly double shipHorizontalPosition = App.ScreenHeight - 50;

        private bool goingLeft = false, goingRight = false;

        public GamePage()
        {
            this.InitializeComponent();

            // capture keyboard controls if a PC is being used to play game
            Loaded += (sender, args) =>
            {
                // Resize move controls to fit the area
                LeftCanvas.Width = LeftCanvas.Height = (LeftArea.ActualWidth / 2) - 10;
                RightCanvas.Width = RightCanvas.Height = (LeftArea.ActualWidth / 2) - 10;

                // Position the ship to the bottom center of the screen
                shipPosition = LayoutRoot.ActualWidth / 3;
                 Rocket.Margin = new Thickness(shipPosition, shipHorizontalPosition, 20, 20);

                Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

                // Starfield background
                CreateStar();
                Move.Completed += MoveStars;
                Move.Begin();

            };

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
        private void OnFire(object sender, TappedRoutedEventArgs e)
        {
        }

        private void MoveShip(int amount)
        {
            shipPosition += amount;

            // Let's make sure that the ship stays in the screen
            if (shipPosition > LayoutRoot.ActualWidth - 30)
            {
                shipPosition = LayoutRoot.ActualWidth - 30;
            }
            else if (shipPosition < 0)
            {
                shipPosition = 0;
            }

            Rocket.Margin = new Thickness(shipPosition, shipHorizontalPosition, 0, 0);
        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Left:
                    MoveShip(-5);
                    break;
                case VirtualKey.Right:
                    MoveShip(5);
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
