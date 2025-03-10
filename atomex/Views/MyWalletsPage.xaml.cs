﻿using System;
using Xamarin.Forms;

namespace atomex
{
    public partial class MyWalletsPage : ContentPage
    {
        Color selectedItemBackgroundColor;

        public MyWalletsPage()
        {
            InitializeComponent();
        }

        public MyWalletsPage(MyWalletsViewModel myWalletsViewModel)
        {
            InitializeComponent();

            string selectedColorName = "ListViewSelectedBackgroundColor";

            if (Application.Current.RequestedTheme == OSAppTheme.Dark)
                selectedColorName = "ListViewSelectedBackgroundColorDark";

            Application.Current.Resources.TryGetValue(selectedColorName, out var selectedColor);
            selectedItemBackgroundColor = (Color)selectedColor;

            BindingContext = myWalletsViewModel;
        }

        private async void OnItemTapped(object sender, EventArgs args)
        {
            Frame selectedItem = (Frame)sender;
            selectedItem.IsEnabled = false;
            Color initColor = selectedItem.BackgroundColor;

            selectedItem.BackgroundColor = selectedItemBackgroundColor;

            await selectedItem.ScaleTo(1.01, 50);
            await selectedItem.ScaleTo(1, 50, Easing.SpringOut);

            selectedItem.BackgroundColor = initColor;
            selectedItem.IsEnabled = true;
        }
    }
}
