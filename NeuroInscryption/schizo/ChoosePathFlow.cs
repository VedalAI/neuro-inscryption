using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Types;

namespace NeuroInscryption.Automation.Game;

internal sealed class ChoosePathFlow : AutomationFlow<MapNodeManager>
{
    public override MethodBase EnableHook => AccessTools.Method(typeof(MapNodeManager), nameof(MapNodeManager.SetActiveNode));

    protected override IEnumerator Execute(MapNodeManager target)
    {
        MapNode currentNode = target.ActiveNode;

        List<string> paths = GetPaths(currentNode);
        foreach (string path in paths)
        {
            Plugin.Logger.LogWarning(path);
        }

        yield break;
    }

    private static List<string> GetPaths(MapNode node, int depth = 0)
    {
        if (node.connectedNodes.Count == 0 || depth == 5)
        {
            return [node.name];
        }

        List<string> result = [];

        foreach (MapNode childNode in node.connectedNodes)
        {
            List<string> paths = GetPaths(childNode, depth + 1);

            if (depth != 0)
            {
                for (int i = 0; i < paths.Count; i++)
                {
                    paths[i] = GetNodeDescription(node) + ", " + paths[i];
                }
            }

            result.AddRange(paths);
        }

        return result;
    }

    private static StrMapNode GetNodeDescription(MapNode node)
    {
        return node.Data switch
        {
            // Combat events
            CardBattleNodeData => StrMapNode.CardBattle,
            TotemBattleNodeData => StrMapNode.TotemBattle,
            BossBattleNodeData => StrMapNode.BossBattle,

            // Non-combat events
            BuildTotemNodeData => StrMapNode.BuildTotem,
            BuyPeltsNodeData => StrMapNode.BuyPelts,

            CabinNodeData cabinNodeData => "Final boss battle", // TODO: Figure out if this is only Act 1
            AttachGemNodeData attachGemNodeData => throw new NotImplementedException(),
            BoulderChoiceNodeData boulderChoiceNodeData => "Get a random card with a 66% chance for an insect and a 33% chance for a Golden Pelt",
            BuildACardNodeData buildACardNodeData => throw new NotImplementedException(),
            AddCardAbilityNodeData addCardAbilityNodeData => throw new NotImplementedException(),
            CardBundleChoicesNodeData cardBundleChoicesNodeData => throw new NotImplementedException(),
            TradeCardsNodeData tradeCardsNodeData => throw new NotImplementedException(),
            CardChoicesNodeData cardChoice => cardChoice.choicesType switch
            {
                CardChoicesType.Random => StrMapNode.CardChoiceRandom,
                CardChoicesType.Cost => StrMapNode.CardChoiceCost,
                CardChoicesType.Tribe => StrMapNode.CardChoiceTribe,
                _ => throw new ArgumentOutOfRangeException()
            },
            CardMergeNodeData cardMergeNodeData => "Sacrifice a card and transfer its sigils to another card",
            CardRemoveNodeData cardRemoveNodeData => throw new NotImplementedException(), // Bone Altar?
            CardStatBoostNodeData cardStatBoostNodeData => throw new NotImplementedException(),
            ChooseEyeballNodeData chooseEyeballNodeData => throw new NotImplementedException(),
            ChooseRareCardNodeData chooseRareCardNodeData => throw new NotImplementedException(),
            CopyCardNodeData copyCardNodeData => throw new NotImplementedException(),
            CreateTransformerNodeData createTransformerNodeData => throw new NotImplementedException(),
            DeckTrialNodeData => "Deck Trial",
            DuplicateMergeNodeData duplicateMergeNodeData => throw new NotImplementedException(),
            GainConsumablesNodeData gainConsumablesNodeData => throw new NotImplementedException(),
            ModifySideDeckNodeData modifySideDeckNodeData => throw new NotImplementedException(),
            OverclockCardNodeData overclockCardNodeData => throw new NotImplementedException(),
            RecycleCardNodeData recycleCardNodeData => throw new NotImplementedException(),
            TradePeltsNodeData tradePeltsNodeData => "Trade Pelts"
            UnlockPart3ItemNodeData unlockPart3ItemNodeData => throw new NotImplementedException(),
            VictoryFeastNodeData victoryFeastNodeData => throw new NotImplementedException(),
            SpecialNodeData specialNodeData => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException()
        }
    }
}
