using MauiAppGraphicsTest.Interfaces;
using MauiAppGraphicsTest.Models;
using Microsoft.Maui.Controls.Shapes;
using System.Collections;
using System.Windows.Input;

namespace MauiAppGraphicsTest.Controls
{
    public class StableExpanderView : ContentView
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(StableExpanderView),
                propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty ToggleCommandProperty =
            BindableProperty.Create(nameof(ToggleCommand), typeof(ICommand), typeof(StableExpanderView));

        public static readonly BindableProperty ItemClickCommandProperty =
            BindableProperty.Create(nameof(ItemClickCommand), typeof(ICommand), typeof(StableExpanderView));

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
        private readonly Dictionary<IHierarchicalItem, bool> _animatingItems = new();
        private readonly Dictionary<StackLayout, IHierarchicalItem> _containerToItem = new();

        public StableExpanderView()
        {
            _container = new StackLayout { Spacing = 3, Padding = 10 };
            Content = new ScrollView { Content = _container };
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object? oldValue, object? newValue)
        {
            if (bindable is StableExpanderView view)
            {
                view.RebuildHierarchy();
            }
        }

        private void RebuildHierarchy()
        {
            _container.Children.Clear();
            _animatingItems.Clear();
            _containerToItem.Clear();

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
            if (item.HasChildren)
            {
                var mainBorder = new Border
                {
                    BackgroundColor = item.BackgroundColor,
                    StrokeThickness = 0,
                    Margin = new Thickness(depth * 15, 3, 0, 3)
                };
                mainBorder.StrokeShape = new RoundRectangle { CornerRadius = 8 };

                var mainStack = new StackLayout { Spacing = 0 };
                var headerGrid = CreateHeaderGrid(item);
                mainStack.Children.Add(headerGrid);

                var contentContainer = new StackLayout
                {
                    Spacing = 5,
                    Padding = new Thickness(15, 10, 15, 15),
                    IsVisible = item.IsExpanded,
                    HeightRequest = item.IsExpanded ? -1 : 0
                };

                // REGISTRA il mapping container -> item
                _containerToItem[contentContainer] = item;

                foreach (var child in item.GetChildren())
                {
                    if (child is IHierarchicalItem childItem)
                    {
                        var childView = CreateExpanderForItem(childItem, 0);
                        contentContainer.Children.Add(childView);
                    }
                }

                mainStack.Children.Add(contentContainer);
                mainBorder.Content = mainStack;

                if (item is Models.ExpandableItem expandableItem)
                {
                    expandableItem.PropertyChanged += async (s, e) =>
                    {
                        if (e.PropertyName == nameof(IHierarchicalItem.IsExpanded))
                        {
                            await AnimateExpansion(item, contentContainer);
                        }
                    };
                }

                var tapGesture = new TapGestureRecognizer
                {
                    Command = ToggleCommand,
                    CommandParameter = item
                };
                mainBorder.GestureRecognizers.Add(tapGesture);

                return mainBorder;
            }
            else
            {
                var header = CreateHeaderForItem(item, depth);
                var tapGesture = new TapGestureRecognizer
                {
                    Command = ItemClickCommand,
                    CommandParameter = item
                };
                header.GestureRecognizers.Add(tapGesture);
                return header;
            }
        }

        private async Task AnimateExpansion(IHierarchicalItem item, StackLayout contentContainer)
        {
            Console.WriteLine($"ANIMATE: {item.DisplayName} -> {(item.IsExpanded ? "EXPAND" : "COLLAPSE")}");

            if (_animatingItems.ContainsKey(item) && _animatingItems[item])
            {
                Console.WriteLine($"SKIPPED: {item.DisplayName} already animating");
                return;
            }

            _animatingItems[item] = true;

            try
            {
                await this.Dispatcher.DispatchAsync(async () =>
                {
                    if (item.IsExpanded)
                    {
                        await ExpandContent(contentContainer);
                    }
                    else
                    {
                        await CollapseContent(contentContainer);
                    }

                    Console.WriteLine($"CALLING RefreshParentContainers for {item.DisplayName}");
                    await RefreshParentContainers(contentContainer);
                });
            }
            finally
            {
                _animatingItems[item] = false;
                Console.WriteLine($"FINISHED: {item.DisplayName}");
            }
        }

        private async Task ExpandContent(StackLayout contentContainer)
        {
            contentContainer.IsVisible = true;
            contentContainer.HeightRequest = -1;
            await Task.Delay(20);

            var targetHeight = contentContainer.Height;
            if (targetHeight <= 0) return;

            contentContainer.HeightRequest = 0;

            var animation = new Animation(v =>
            {
                this.Dispatcher.Dispatch(() =>
                {
                    contentContainer.HeightRequest = v;
                });
            }, 0, targetHeight);

            var tcs = new TaskCompletionSource<bool>();
            animation.Commit(contentContainer, "ExpandAnimation", 16, 350, Easing.CubicOut,
                (v, c) => tcs.SetResult(true));

            await tcs.Task;
            contentContainer.HeightRequest = -1;
        }

        private async Task CollapseContent(StackLayout contentContainer)
        {
            var currentHeight = contentContainer.Height;
            if (currentHeight <= 0) currentHeight = contentContainer.HeightRequest;
            if (currentHeight <= 0) return;

            contentContainer.HeightRequest = currentHeight;

            var animation = new Animation(v =>
            {
                this.Dispatcher.Dispatch(() =>
                {
                    contentContainer.HeightRequest = v;
                });
            }, currentHeight, 0);

            var tcs = new TaskCompletionSource<bool>();
            animation.Commit(contentContainer, "CollapseAnimation", 16, 300, Easing.CubicIn,
                (v, c) => tcs.SetResult(true));

            await tcs.Task;
            contentContainer.IsVisible = false;
        }

        // NUOVA funzione che aggiorna TUTTI i parent container
        private async Task RefreshParentContainers(StackLayout changedContainer)
        {
            await Task.Delay(50);

            Console.WriteLine($"=== REFRESH PARENT START ===");
            Console.WriteLine($"Changed container height: {changedContainer.Height}, HeightRequest: {changedContainer.HeightRequest}");

            var current = changedContainer.Parent;
            int level = 0;

            while (current != null)
            {
                level++;
                Console.WriteLine($"Level {level}: {current.GetType().Name}");

                if (current is StackLayout parentContainer &&
                    _containerToItem.ContainsKey(parentContainer))
                {
                    var parentItem = _containerToItem[parentContainer];
                    Console.WriteLine($"  - Parent item: {parentItem.DisplayName}");
                    Console.WriteLine($"  - Parent IsExpanded: {parentItem.IsExpanded}");
                    Console.WriteLine($"  - Parent Height: {parentContainer.Height}");
                    Console.WriteLine($"  - Parent HeightRequest: {parentContainer.HeightRequest}");
                    Console.WriteLine($"  - Parent IsVisible: {parentContainer.IsVisible}");

                    if (parentItem.IsExpanded)
                    {
                        var oldHeight = parentContainer.HeightRequest;
                        parentContainer.HeightRequest = 0;
                        parentContainer.HeightRequest = -1;
                        parentContainer.InvalidateMeasure();

                        Console.WriteLine($"  - UPDATED: {oldHeight} -> {parentContainer.HeightRequest}");

                        if (parentContainer.Parent is VisualElement ve)
                        {
                            ve.InvalidateMeasure();
                            Console.WriteLine($"  - Invalidated parent: {ve.GetType().Name}");
                        }
                    }
                }

                if (current is VisualElement element)
                {
                    current = element.Parent;
                }
                else break;
            }

            Console.WriteLine($"=== REFRESH PARENT END ===\n");
        }

        private Grid CreateHeaderGrid(IHierarchicalItem item)
        {
            var grid = new Grid
            {
                Padding = new Thickness(15, 12),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            var iconLabel = new Label
            {
                Text = item.DisplayIcon,
                FontSize = GetIconSize(item),
                VerticalOptions = LayoutOptions.Center,
                TextColor = Colors.White
            };
            Grid.SetColumn(iconLabel, 0);

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

                if (item is Models.ExpandableItem expandableItem)
                {
                    expandableItem.PropertyChanged += async (s, e) =>
                    {
                        if (e.PropertyName == nameof(IHierarchicalItem.IsExpanded))
                        {
                            await this.Dispatcher.DispatchAsync(async () =>
                            {
                                var targetRotation = item.IsExpanded ? 90 : 0;
                                await arrowLabel.RotateTo(targetRotation, 250, Easing.SpringOut);
                            });
                        }
                    };
                }

                grid.Children.Add(arrowLabel);
            }

            grid.Children.Add(iconLabel);
            grid.Children.Add(contentStack);
            return grid;
        }

        private View CreateHeaderForItem(IHierarchicalItem item, int depth)
        {
            var border = new Border
            {
                BackgroundColor = item.BackgroundColor,
                StrokeThickness = 0,
                Padding = new Thickness(15, 12),
                Margin = new Thickness(depth * 15, 3, 0, 3)
            };
            border.StrokeShape = new RoundRectangle { CornerRadius = 8 };

            var grid = CreateHeaderGrid(item);
            border.Content = grid;
            return border;
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