using CommunityToolkit.Maui.Animations;
using MauiAppGraphicsTest.Interfaces;
using MauiAppGraphicsTest.Models;
using Microsoft.Maui.Controls.Shapes;
using System.Collections;
using System.Windows.Input;

namespace MauiAppGraphicsTest.Controls
{
    public class AnimatedExpanderView : ContentView
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(AnimatedExpanderView),
                propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty ToggleCommandProperty =
            BindableProperty.Create(nameof(ToggleCommand), typeof(ICommand), typeof(AnimatedExpanderView));

        public static readonly BindableProperty ItemClickCommandProperty =
            BindableProperty.Create(nameof(ItemClickCommand), typeof(ICommand), typeof(AnimatedExpanderView));

        public IEnumerable? ItemsSource
        {
            get => (IEnumerable?)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public ICommand? ToggleCommand
        {
            get => (ICommand?)GetValue(ToggleCommandProperty);
            set => SetValue(ToggleCommandProperty, value);
        }

        public ICommand? ItemClickCommand
        {
            get => (ICommand?)GetValue(ItemClickCommandProperty);
            set => SetValue(ItemClickCommandProperty, value);
        }

        private readonly StackLayout _container;

        public AnimatedExpanderView()
        {
            _container = new StackLayout { Spacing = 5, Padding = 10 };
            Content = new ScrollView { Content = _container };
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object? oldValue, object? newValue)
        {
            if (bindable is AnimatedExpanderView view)
            {
                view.RebuildHierarchy();
            }
        }

        private void RebuildHierarchy()
        {
            _container.Children.Clear();

            if (ItemsSource == null) return;

            foreach (var item in ItemsSource)
            {
                if (item is IHierarchicalItem hierarchicalItem)
                {
                    var expanderView = CreateExpanderForItem(hierarchicalItem, 0);
                    _container.Children.Add(expanderView);
                }
            }
        }

        private View CreateExpanderForItem(IHierarchicalItem item, int depth)
        {
            // BORDER PRINCIPALE CHE SI ESPANDE
            var mainBorder = new Border
            {
                BackgroundColor = item.BackgroundColor,
                StrokeThickness = 0,
                Margin = new Thickness(depth * 15, 3, 0, 3)
            };
            mainBorder.StrokeShape = new RoundRectangle { CornerRadius = 8 };

            // CONTAINER VERTICALE DENTRO IL BORDER
            var mainStack = new StackLayout { Spacing = 0 };

            // 1. HEADER (sempre visibile)
            var headerGrid = CreateHeaderGrid(item);
            mainStack.Children.Add(headerGrid);

            // 2. CONTENT CONTAINER (figli - espandibile)
            StackLayout? contentContainer = null;
            if (item.HasChildren)
            {
                contentContainer = new StackLayout
                {
                    Spacing = 8,
                    Padding = new Thickness(15, 15, 15, 10),
                    HeightRequest = item.IsExpanded ? -1 : 0,
                    IsVisible = item.IsExpanded,
                    Opacity = item.IsExpanded ? 1 : 0
                };

                // Popola i figli
                foreach (var child in item.GetChildren())
                {
                    if (child is IHierarchicalItem childItem)
                    {
                        var childView = CreateExpanderForItem(childItem, 0); // Depth 0 perché sono dentro il parent
                        contentContainer.Children.Add(childView);
                    }
                }

                mainStack.Children.Add(contentContainer);

                // ANIMAZIONE QUANDO CAMBIA IsExpanded
                if (item is Models.ExpandableItem expandableItem)
                {
                    expandableItem.PropertyChanged += async (s, e) =>
                    {
                        if (e.PropertyName == nameof(IHierarchicalItem.IsExpanded))
                        {
                            await AnimateExpansion(contentContainer, item.IsExpanded);
                        }
                    };
                }
            }

            mainBorder.Content = mainStack;

            // GESTURE PER CLICK
            var tapGesture = new TapGestureRecognizer();
            if (item.HasChildren)
            {
                tapGesture.Command = ToggleCommand;
                tapGesture.CommandParameter = item;
            }
            else
            {
                tapGesture.Command = ItemClickCommand;
                tapGesture.CommandParameter = item;
            }
            mainBorder.GestureRecognizers.Add(tapGesture);

            return mainBorder;
        }

        private Grid CreateHeaderGrid(IHierarchicalItem item)
        {
            var grid = new Grid
            {
                Padding = new Thickness(15, 12),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto },  // Icon
                    new ColumnDefinition { Width = GridLength.Star },  // Content
                    new ColumnDefinition { Width = GridLength.Auto }   // Arrow
                }
            };

            // Icon
            var iconLabel = new Label
            {
                Text = item.DisplayIcon,
                FontSize = GetIconSize(item),
                VerticalOptions = LayoutOptions.Center,
                TextColor = Colors.White
            };
            Grid.SetColumn(iconLabel, 0);

            // Content
            var contentStack = new StackLayout
            {
                Margin = new Thickness(15, 0),
                Spacing = 2,
                VerticalOptions = LayoutOptions.Center
            };

            var nameLabel = new Label
            {
                Text = item.DisplayName,
                FontSize = GetNameFontSize(item),
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White
            };
            contentStack.Children.Add(nameLabel);

            if (!string.IsNullOrEmpty(item.DisplaySubtitle))
            {
                var subtitleLabel = new Label
                {
                    Text = item.DisplaySubtitle,
                    FontSize = GetSubtitleFontSize(item),
                    TextColor = Colors.LightGray
                };
                contentStack.Children.Add(subtitleLabel);
            }

            Grid.SetColumn(contentStack, 1);

            // Arrow (se ha figli)
            if (item.HasChildren)
            {
                var arrowLabel = new Label
                {
                    Text = "▶",
                    FontSize = 16,
                    TextColor = Colors.White,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Rotation = item.IsExpanded ? 90 : 0
                };
                Grid.SetColumn(arrowLabel, 2);

                // Animazione freccia
                if (item is Models.ExpandableItem expandableItem)
                {
                    expandableItem.PropertyChanged += async (s, e) =>
                    {
                        if (e.PropertyName == nameof(IHierarchicalItem.IsExpanded))
                        {
                            await AnimateArrow(arrowLabel, item.IsExpanded);
                        }
                    };
                }

                grid.Children.Add(arrowLabel);
            }

            grid.Children.Add(iconLabel);
            grid.Children.Add(contentStack);

            return grid;
        }

        // ANIMAZIONI FLUIDE CON COMMUNITY TOOLKIT
        private async Task AnimateExpansion(StackLayout contentContainer, bool isExpanded)
        {
            await this.Dispatcher.DispatchAsync(async () =>
            {
                if (isExpanded)
                {
                    // ESPANSIONE: Height + Fade In + Slide Down
                    contentContainer.IsVisible = true;
                    contentContainer.HeightRequest = -1;

                    var tasks = new List<Task>
                    {
                        contentContainer.FadeTo(1, 400, Easing.CubicOut),
                        contentContainer.ScaleTo(1, 400, Easing.CubicOut),
                        AnimateHeight(contentContainer, 0, -1, 400)
                    };

                    await Task.WhenAll(tasks);
                }
                else
                {
                    // CONTRAZIONE: Fade Out + Slide Up + Height
                    var tasks = new List<Task>
                    {
                        contentContainer.FadeTo(0, 300, Easing.CubicIn),
                        contentContainer.ScaleTo(0.95, 300, Easing.CubicIn),
                        AnimateHeight(contentContainer, -1, 0, 300)
                    };

                    await Task.WhenAll(tasks);
                    contentContainer.IsVisible = false;
                }
            });
        }

        private async Task AnimateArrow(Label arrowLabel, bool isExpanded)
        {
            await this.Dispatcher.DispatchAsync(async () =>
            {
                // Rotazione fluida con bounce
                await arrowLabel.RotateTo(isExpanded ? 90 : 0, 300, Easing.SpringOut);
            });
        }

        private async Task AnimateHeight(View view, double fromHeight, double toHeight, uint duration)
        {
            var animation = new Animation(v =>
            {
                this.Dispatcher.Dispatch(() =>
                {
                    view.HeightRequest = v;
                });
            }, fromHeight, toHeight);

            animation.Commit(view, "HeightAnimation", 16, duration, Easing.CubicInOut);
            await Task.Delay((int)duration);
        }

        private static double GetIconSize(IHierarchicalItem item)
        {
            return item switch
            {
                Fiera => 28,
                Padiglione => 24,
                Stand => 20,
                Espositore => 18,
                _ => 18
            };
        }

        private static double GetNameFontSize(IHierarchicalItem item)
        {
            return item switch
            {
                Fiera => 20,
                Padiglione => 18,
                Stand => 16,
                Espositore => 14,
                _ => 14
            };
        }

        private static double GetSubtitleFontSize(IHierarchicalItem item)
        {
            return item switch
            {
                Fiera => 14,
                Padiglione => 12,
                Stand => 11,
                Espositore => 10,
                _ => 10
            };
        }
    }
}