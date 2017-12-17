﻿using System;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MySpaceInvaders
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {

        private bool goingLeft = false, goingRight = false;

        public GamePage()
        {
            this.InitializeComponent();

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
    }
}
