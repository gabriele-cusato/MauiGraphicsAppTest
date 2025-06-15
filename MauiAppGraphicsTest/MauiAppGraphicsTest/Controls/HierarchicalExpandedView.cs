using MauiAppGraphicsTest.Interfaces;
using MauiAppGraphicsTest.Models;
using Microsoft.Maui.Controls.Shapes;
using System.Collections;
using System.Windows.Input;
using UraniumUI.Controls;
using UraniumUI.Material;

namespace MauiAppGraphicsTest.Controls
{
    public class HierarchicalExpanderView : ContentView
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(HierarchicalExpanderView),
                propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty ToggleCommandProperty =
            BindableProperty.Create(nameof(ToggleCommand), typeof(ICommand), typeof(HierarchicalExpanderView));

        public static readonly BindableProperty ItemClickCommandProperty =
            BindableProperty.Create(nameof(ItemClickCommand), typeof(ICommand), typeof(HierarchicalExpanderView));

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

        public HierarchicalExpanderView()
        {
            _container = new StackLayout { Spacing = 5 };
            Content = _container;
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object? oldValue, object? newValue)
        {
            if (bindable is HierarchicalExpanderView view)
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
            // Usando il namespace completo per ExpanderView
            var expander = new ExpanderView
            {
                IsExpanded = item.IsExpanded,
                Margin = new Thickness(depth * 20, 2, 0, 2)
            };

            // Bind IsExpanded
            expander.SetBinding(ExpanderView.IsExpandedProperty,
                new Binding(nameof(IHierarchicalItem.IsExpanded), source: item));

            // Create Header
            var header = CreateHeaderForItem(item);
            expander.Header = header;

            // Create Content if has children
            if (item.HasChildren)
            {
                var contentStack = new StackLayout { Spacing = 2 };

                foreach (var child in item.GetChildren())
                {
                    if (child is IHierarchicalItem childItem)
                    {
                        var childView = CreateExpanderForItem(childItem, depth + 1);
                        contentStack.Children.Add(childView);
                    }
                }

                expander.Content = contentStack;
            }

            return expander;
        }

        private View CreateHeaderForItem(IHierarchicalItem item)
        {
            // Sostituito Frame con Border
            var border = new Border
            {
                BackgroundColor = item.BackgroundColor,
                StrokeThickness = 0,
                Padding = new Thickness(12, 8)
            };
            border.StrokeShape = new RoundRectangle { CornerRadius = 8 };

            var grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
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
                Margin = new Thickness(10, 0),
                Spacing = 2
            };

            var nameLabel = new Label
            {
                Text = item.DisplayName,
                FontSize = GetNameFontSize(item),
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White
            };

            var subtitleLabel = new Label
            {
                Text = item.DisplaySubtitle,
                FontSize = GetSubtitleFontSize(item),
                TextColor = Colors.LightGray
            };

            contentStack.Children.Add(nameLabel);
            contentStack.Children.Add(subtitleLabel);

            Grid.SetColumn(contentStack, 1);

            // Properties summary
            var propertiesStack = new StackLayout
            {
                HorizontalOptions = LayoutOptions.End,
                Spacing = 1
            };

            var properties = item.GetDisplayProperties();
            var keyProps = properties.Take(2);

            foreach (var prop in keyProps)
            {
                var propLabel = new Label
                {
                    Text = $"{prop.Value}",
                    FontSize = 10,
                    TextColor = Colors.LightGray,
                    HorizontalTextAlignment = TextAlignment.End
                };
                propertiesStack.Children.Add(propLabel);
            }

            Grid.SetColumn(propertiesStack, 2);

            grid.Children.Add(iconLabel);
            grid.Children.Add(contentStack);
            grid.Children.Add(propertiesStack);

            border.Content = grid;

            // Add tap gesture
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
            border.GestureRecognizers.Add(tapGesture);

            return border;
        }

        private static double GetIconSize(IHierarchicalItem item)
        {
            return item switch
            {
                Fiera => 24,
                Padiglione => 20,
                Stand => 18,
                Espositore => 16,
                _ => 16
            };
        }

        private static double GetNameFontSize(IHierarchicalItem item)
        {
            return item switch
            {
                Fiera => 18,
                Padiglione => 16,
                Stand => 14,
                Espositore => 13,
                _ => 13
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
