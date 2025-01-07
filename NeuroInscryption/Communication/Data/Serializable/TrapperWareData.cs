using System;
using DiskCardGame;
using NeuroInscryption.Communication.Strings;
using Newtonsoft.Json;

namespace NeuroInscryption.Communication.Data.Serializable;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class TrapperWareData : IIdentifiable
{
    [JsonProperty("Name")]
    public string Identifier { get; private set; }
    public string Description { get; private set; }
    public string Price { get; private set; }

    // Excluded from JSON

    [JsonIgnore]
    public MainInputInteractable Interactable { get; private set; }

    [JsonIgnore]
    public bool IsKnife { get; private set; }

    [JsonIgnore]
    public int PriceNum { get; private set; }

    [JsonIgnore]
    public View TargetView => IsKnife ? View.Default : View.TradingTopDown;

    private TrapperWareData(SelectableCard card, int price)
    {
        Price = TrapperStrings.TeethCost.Format(price);
        PriceNum = price;
        Interactable = card;
        IsKnife = false;

        switch (card.Info.name)
        {
            case "PeltHare":
                Identifier = TrapperStrings.RabbitPelt.Name;
                Description = TrapperStrings.RabbitPelt.Info;
                break;

            case "PeltWolf":
                Identifier = TrapperStrings.WolfPelt.Name;
                Description = TrapperStrings.WolfPelt.Info;
                break;

            case "PeltGolden":
                Identifier = TrapperStrings.GoldenPelt.Name;
                Description = TrapperStrings.GoldenPelt.Info;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private TrapperWareData(MainInputInteractable interactable, int price)
    {
        Interactable = interactable;
        IsKnife = true;
        Identifier = TrapperStrings.SkinningKnife.Name;
        Description = TrapperStrings.SkinningKnife.Info;
        Price = TrapperStrings.TeethCost.Format(price);
        PriceNum = price;
    }

    public static TrapperWareData CreatePelt(SelectableCard card, int price) => new(card, price);
    public static TrapperWareData CreateKnife(MainInputInteractable interactable, int price) => new(interactable, price);
}
