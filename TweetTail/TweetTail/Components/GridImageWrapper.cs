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

        private Grid Grid;
        public GridImageWrapper(Grid grid)
        {
            this.Grid = grid;

            images = new List<CachedImage>();
            for (int i = 0; i < 4; i++)
            {
                var cached = new CachedImage
                {
                    Aspect = Aspect.AspectFill
                };
                images.Add(cached);
            }

            SetCount(0);
        }

        public CachedImage this[int i] {
            get { return images[i]; }
            set { images[i] = value; }
        }

        public void SetCount(int count)
        {
            Grid.RowDefinitions.Clear();
            Grid.ColumnDefinitions.Clear();
            Grid.Children.Clear();

            if(count == 0)
            {
                Grid.IsVisible = false;
            }
            Grid.IsVisible = true;

            if(count > 1)
            {
                foreach(var item in images)
                {
                    item.WidthRequest = Grid.Width / 2;
                }
            }
            else
            {
                foreach (var item in images)
                {
                    item.WidthRequest = Grid.Width;
                }
            }

            switch (count)
            {
                case 1:
                    Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    Grid.Children.Add(this[0], 0, 0);
                    break;
                case 2:
                    Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    Grid.Children.Add(this[0], 0, 0);
                    Grid.Children.Add(this[1], 1, 0);
                    break;
                case 3:
                    Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    Grid.Children.Add(this[0], 0, 0);
                    Grid.Children.Add(this[1], 1, 0);
                    Grid.Children.Add(this[2], 0, 1);
                    break;
                case 4:
                    Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    Grid.Children.Add(this[0], 0, 0);
                    Grid.Children.Add(this[1], 1, 0);
                    Grid.Children.Add(this[2], 0, 1);
                    Grid.Children.Add(this[3], 1, 1);
                    break;
            }
        }
    }
}
