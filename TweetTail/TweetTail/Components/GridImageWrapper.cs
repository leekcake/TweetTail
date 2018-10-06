using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TweetTail.Components
{
    public class GridImageWrapper
    {
        private List<CachedImage> images;

        private Grid grid;
        public GridImageWrapper(Grid grid)
        {
            this.grid = grid;

            images = new List<CachedImage>();
            for (int i = 0; i < 4; i++)
            {
                var cached = new CachedImage();
                cached.Aspect = Aspect.AspectFill;
                cached.WidthRequest = 10000;
                images.Add(cached);
            }

            setCount(0);
        }

        public CachedImage this[int i] {
            get { return images[i]; }
            set { images[i] = value; }
        }

        public void setCount(int count)
        {
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
            grid.Children.Clear();

            if(count == 0)
            {
                grid.IsVisible = false;
            }
            grid.IsVisible = true;

            switch (count)
            {
                case 1:
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grid.Children.Add(this[0], 0, 0);
                    break;
                case 2:
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grid.Children.Add(this[0], 0, 0);
                    grid.Children.Add(this[1], 1, 0);
                    break;
                case 3:
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grid.Children.Add(this[0], 0, 0);
                    grid.Children.Add(this[1], 1, 0);
                    grid.Children.Add(this[2], 0, 1);
                    break;
                case 4:
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grid.Children.Add(this[0], 0, 0);
                    grid.Children.Add(this[1], 1, 0);
                    grid.Children.Add(this[2], 0, 1);
                    grid.Children.Add(this[3], 1, 1);
                    break;
            }
        }
    }
}
