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
                            // ANIMAZIONE UNIFICATA - tutto insieme
                            await AnimateExpansionUnified(item, contentContainer, headerGrid);
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

        // ANIMAZIONE UNIFICATA - freccia + contenuto + parent in sincrono
        private async Task AnimateExpansionUnified(IHierarchicalItem item, StackLayout contentContainer, Grid headerGrid)
        {
            if (_animatingItems.ContainsKey(item) && _animatingItems[item])
                return;

            _animatingItems[item] = true;

            try
            {
                // Trova la freccia nel header
                var arrowLabel = FindArrowInGrid(headerGrid);
                var parentContainers = GetParentContainers(contentContainer);

                if (item.IsExpanded)
                {
                    await ExpandUnified(contentContainer, arrowLabel, parentContainers);
                }
                else
                {
                    await CollapseUnified(contentContainer, arrowLabel, parentContainers);
                }
            }
            finally
            {
                _animatingItems[item] = false;
            }
        }

        private Label? FindArrowInGrid(Grid grid)
        {
            foreach (var child in grid.Children)
            {
                if (child is Label label && label.Text == "▶")
                    return label;
            }
            return null;
        }

        private async Task ExpandUnified(StackLayout contentContainer, Label? arrowLabel, List<StackLayout> parentContainers)
        {
            // CALCOLA altezza sommando i figli SENZA toccare il layout
            double targetHeight = 0;

            foreach (var child in contentContainer.Children)
            {
                if (child is View childView)
                {
                    // Stima altezza basata sul tipo di controllo
                    if (childView is Border border)
                    {
                        // Border con header (circa 60px) + padding
                        targetHeight += 70;
                    }
                    else if (childView is Label label)
                    {
                        // Label singola
                        targetHeight += 30;
                    }
                    else
                    {
                        // Default per altri controlli
                        targetHeight += 50;
                    }
                }
            }

            // Aggiungi spacing e padding del container
            if (contentContainer.Children.Count > 0)
            {
                targetHeight += (contentContainer.Children.Count - 1) * contentContainer.Spacing;
            }
            targetHeight += contentContainer.Padding.Top + contentContainer.Padding.Bottom;

            Console.WriteLine($"Calculated height: {targetHeight}"); // DEBUG

            if (targetHeight <= 0) return;

            // SETUP iniziale - tutto a 0, nessun flash
            contentContainer.IsVisible = true;
            contentContainer.HeightRequest = 0;
            if (arrowLabel != null) arrowLabel.Rotation = 0;

            // ANIMAZIONE UNIFICATA
            var animation = new Animation(progress =>
            {
                contentContainer.HeightRequest = targetHeight * progress;

                if (arrowLabel != null)
                    arrowLabel.Rotation = 90 * progress;

                foreach (var parent in parentContainers)
                {
                    parent.InvalidateMeasure();
                }
            }, 0, 1);

            var tcs = new TaskCompletionSource<bool>();
            animation.Commit(contentContainer, "UnifiedExpandAnimation",
                16, 250, Easing.SinOut,
                (v, c) => tcs.SetResult(true));

            await tcs.Task;

            // Finalizza con auto-sizing
            contentContainer.HeightRequest = -1;
            if (arrowLabel != null) arrowLabel.Rotation = 90;
        }

        private async Task CollapseUnified(StackLayout contentContainer, Label? arrowLabel, List<StackLayout> parentContainers)
        {
            var currentHeight = contentContainer.Height;
            if (currentHeight <= 0) return;

            var animation = new Animation(progress =>
            {
                contentContainer.HeightRequest = currentHeight * (1 - progress);

                if (arrowLabel != null)
                    arrowLabel.Rotation = 90 * (1 - progress);

                foreach (var parent in parentContainers)
                {
                    parent.InvalidateMeasure();
                }

            }, 0, 1);

            var tcs = new TaskCompletionSource<bool>();
            animation.Commit(contentContainer, "UnifiedCollapseAnimation",
                16, 250, Easing.SinIn, // 16=fps, 200=durata
                (v, c) => tcs.SetResult(true));

            await tcs.Task;

            // Finalizza
            contentContainer.IsVisible = false;
            if (arrowLabel != null) arrowLabel.Rotation = 0;
        }

        private List<StackLayout> GetParentContainers(StackLayout contentContainer)
        {
            var parents = new List<StackLayout>();
            var current = contentContainer.Parent;

            while (current != null)
            {
                if (current is StackLayout parentContainer &&
                    _containerToItem.ContainsKey(parentContainer) &&
                    _containerToItem[parentContainer].IsExpanded)
                {
                    parents.Add(parentContainer);
                }

                if (current is VisualElement element)
                    current = element.Parent;
                else break;
            }

            return parents;
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

                // RIMUOVI il PropertyChanged separato - ora gestito in AnimateExpansionUnified
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