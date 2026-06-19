namespace VoidEmpires.Domain.Economy;

public static class ResourceCatalog
{
    public static IReadOnlyList<ResourceCatalogEntry> All { get; } =
    [
        new("Credits", "Creditos", "Balance economico gastable para construccion, investigacion y preparacion local.", true, true, "Currency", "Moneda", "resource.credits", "icon.resource-credits", 10, ["Persisted", "Spendable", "Economy"]),
        new("Metal", "Metal", "Materia prima industrial base para infraestructura, defensas y activos.", true, true, "Material", "Material", "resource.metal", "icon.resource-metal", 20, ["Persisted", "Spendable", "Industry"]),
        new("Crystal", "Cristal", "Material refinado para investigacion, componentes avanzados y produccion especializada.", true, true, "Material", "Material", "resource.crystal", "icon.resource-crystal", 30, ["Persisted", "Spendable", "Research"]),
        new("Gas", "Gas", "Recurso gaseoso para soporte logistico, propulsion y cadenas de produccion avanzadas.", true, true, "Material", "Material", "resource.gas", "icon.resource-gas", 40, ["Persisted", "Spendable", "Logistics"]),
        new("Energy", "Energia", "Senal operativa visible para capacidad o infraestructura; no es una moneda persistida ni sinonimo de Creditos.", false, false, "Operational", "Operacion", "resource.energy", "icon.resource-energy", 50, ["DisplayOnly", "Operational", "NonStockpile"]),
        new("Population", "Poblacion", "Contexto de capacidad y mano de obra local; no es una divisa ni una reserva gastable.", false, false, "Capacity", "Capacidad", "resource.population", "icon.resource-population", 60, ["DisplayOnly", "Capacity", "NonStockpile"]),
        new("Deuterium", "Deuterio", "Termino visible reservado para catalogo futuro; no forma parte del stockpile persistido actual.", false, false, "FutureResource", "Recurso futuro", "resource.deuterium", "icon.resource-deuterium", 70, ["DisplayOnly", "Future", "NonStockpile"])
    ];

    private static readonly IReadOnlyDictionary<ResourceType, ResourceCatalogEntry> PersistedByType =
        new Dictionary<ResourceType, ResourceCatalogEntry>
        {
            [ResourceType.Credits] = All.Single(x => x.Key == "Credits"),
            [ResourceType.Metal] = All.Single(x => x.Key == "Metal"),
            [ResourceType.Crystal] = All.Single(x => x.Key == "Crystal"),
            [ResourceType.Gas] = All.Single(x => x.Key == "Gas")
        };

    public static ResourceCatalogEntry Get(ResourceType resourceType) => PersistedByType[resourceType];
}
