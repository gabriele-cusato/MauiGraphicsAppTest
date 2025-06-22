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
    public partial class Padiglione : ExpandableItem
    {
        [ObservableProperty]
        private string nome = string.Empty;

        [ObservableProperty]
        private string cliente = string.Empty;

        [ObservableProperty]
        private string descrizione = string.Empty;

        [ObservableProperty]
        private int numeroStand = 0;

        [ObservableProperty]
        private double superficieTotale = 0;

        public ObservableCollection<Stand> Stand { get; set; } = new();

        public override string DisplayName => Nome;
        public override string DisplaySubtitle => Cliente;
        public override string DisplayIcon => "🏢";
        public override Color BackgroundColor => Colors.MediumSeaGreen;
        public override bool HasChildren => Stand.Any();

        public override IEnumerable GetChildren()
        {
            return Stand;
        }

        public override Dictionary<string, object> GetDisplayProperties()
        {
            return new Dictionary<string, object>
            {
                { "Cliente", Cliente },
                { "Descrizione", Descrizione },
                { "Stand", $"{Stand.Count} stand" },
                { "Superficie", $"{SuperficieTotale:N0} m²" },
                { "Espositori Totali", Stand.SelectMany(s => s.Espositori).Count().ToString() }
            };
        }
    }
}
