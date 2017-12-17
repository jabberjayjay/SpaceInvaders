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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MySpaceInvaders
{
    public sealed partial class Ship : UserControl
    {
        private bool goingLeft = false, goingRight = false, goingUp = false, goingDown = false;


        public Ship()
        {
            this.InitializeComponent();


#if WINDOWS_PHONE_APP
            Width = 20;
            Height = 40;
            BodyShape.Points = new PointCollection()
            {
                new Point(0, 40), new Point(10,0), new Point(20,40)
            };
#else
            Width = 40;
            Height = 80;
            BodyShape.Points = new PointCollection()
            {
                new Point(0, 80), new Point(20,0), new Point(40,80)
            };
#endif

        }//end ship constructor



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
        private void OnFire(object sender, TappedRoutedEventArgs e)
        {
        }
    }
}
