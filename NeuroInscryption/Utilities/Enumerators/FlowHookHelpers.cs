using System;
using System.Collections;
using NeuroInscryption.Automation.API;

namespace NeuroInscryption.Utilities.Enumerators;

internal static class FlowHookHelpers
{
    public static Func<HarmonyContext, bool> EnumeratorStateIs(int value)
        => (context) => context.GetInstance<IEnumerator>()!.GetState() == value;

    public static Func<HarmonyContext, bool> EnumeratorFinished()
        => (context) => context.GetResult<bool>() == false;
}
