using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace NeuroInscryption.Tweaks;

[HarmonyPatch]
internal static class RedirectSaveFile
{
    [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.SaveFilePath), MethodType.Getter)]
    [HarmonyPostfix]
    private static void RedirectMainSave(ref string __result)
    {
        __result += ".neuro";
    }

    [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.SaveToFile))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> RedirectBackupSave(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldstr && (string) instruction.operand == "SaveFile-Backup.gwsave")
            {
                instruction.operand += ".neuro";
            }

            yield return instruction;
        }
    }
}
