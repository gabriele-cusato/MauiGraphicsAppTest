using MauiAppGraphicsTest.Models;

namespace MauiAppGraphicsTest.Selectors
{
    public class HierarchicalDataTemplateSelector : DataTemplateSelector
    {
        public required DataTemplate FieraTemplate { get; set; }
        public required DataTemplate PadiglioneTemplate { get; set; }
        public required DataTemplate StandTemplate { get; set; }
        public required DataTemplate EspositoreTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return item switch
            {
                Fiera => FieraTemplate,
                Padiglione => PadiglioneTemplate,
                Stand => StandTemplate,
                Espositore => EspositoreTemplate,
                _ => FieraTemplate
            };
        }
    }
}
