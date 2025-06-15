using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppGraphicsTest.Models
{
    public partial class Stand : ExpandableItem
    {
        [ObservableProperty]
        private string codice = string.Empty;

        [ObservableProperty]
        private string nome = string.Empty;

        [ObservableProperty]
        private string cliente = string.Empty;

        [ObservableProperty]
        private double superficie = 0;

        [ObservableProperty]
        private string tipologia = string.Empty;

        [ObservableProperty]
        private decimal costo = 0;

        public ObservableCollection<Espositore> Espositori { get; set; } = new();

        public override string DisplayName => Nome;
        public override string DisplaySubtitle => Codice;
        public override string DisplayIcon => "🏪";
        public override Color BackgroundColor => Colors.Orange;
        public override bool HasChildren => Espositori.Any();

        public override IEnumerable GetChildren()
        {
            return Espositori;
        }

        public override Dictionary<string, object> GetDisplayProperties()
        {
            return new Dictionary<string, object>
            {
                { "Codice", Codice },
                { "Cliente", Cliente },
                { "Superficie", $"{Superficie:N0} m²" },
                { "Tipologia", Tipologia },
                { "Costo", $"€{Costo:N0}" },
                { "Espositori", $"{Espositori.Count} espositori" }
            };
        }
    }
}
