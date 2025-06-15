using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiAppGraphicsTest.Models;
using MauiAppGraphicsTest.Services;
using MauiAppGraphicsTest.Interfaces;
using System.Collections.ObjectModel;

namespace MauiAppGraphicsTest.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Fiera> fiere = new();

        public MainPageViewModel()
        {
            LoadData();
        }

        private void LoadData()
        {
            Fiere = FieraDataService.GetFiereDiTest();
        }

        [RelayCommand]
        private void ToggleItem(IHierarchicalItem item)
        {
            if (item != null)
            {
                item.IsExpanded = !item.IsExpanded;
            }
        }

        [RelayCommand]
        private async Task ShowItemDetailsAsync(IHierarchicalItem item)
        {
            if (item == null) return;

            var properties = item.GetDisplayProperties();
            var details = string.Join("\n", properties.Select(p => $"{p.Key}: {p.Value}"));

            var page = Shell.Current ?? Application.Current?.Windows?.FirstOrDefault()?.Page;
            if (page != null)
            {
                await page.DisplayAlert(
                    $"Dettagli {item.GetType().Name}",
                    $"{item.DisplayName}\n\n{details}",
                    "OK"
                );
            }
        }
    }
}
