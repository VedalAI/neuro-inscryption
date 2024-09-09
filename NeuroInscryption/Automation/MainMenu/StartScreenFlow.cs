using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DiskCardGame;
using HarmonyLib;
using Namotion.Reflection;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;
using NJsonSchema;
using NJsonSchema.Generation;
using UnityEngine;

namespace NeuroInscryption.Automation.MainMenu;

internal sealed class StartScreenFlow : AutomationFlow<AscensionStartScreen>
{
    public override MethodBase EnableHook => AccessTools.Method(typeof(AscensionStartScreen), nameof(AscensionStartScreen.OnEnable));

    public override bool AllowInterruption => true;

    protected override IEnumerator Execute(AscensionStartScreen target)
    {
        yield return new WaitForSeconds(1);
        yield return NeuroMouse.MoveTo(target.newRunText);
        yield return new WaitForSeconds(0.5f);
        NeuroMouse.ClickOnce();

        test.method(new JsonSchemaGenerator(new SystemTextJsonSchemaGeneratorSettings()), new JsonSchema(), null, null);
        // Plugin.Logger.LogWarning(JsonSchema.FromType<Dictionary<string, string>>());

        /*// wrap json schema somehow
        Response response = Websocket.SendDecision<TOutput>(state, query);
        yield return response;
        */
    }
}

public class test
{
    //public Ix y;
    public bool m;

    public static void method(JsonSchemaGenerator __instance, JsonSchema schema, ContextualType contextualType, JsonSchemaResolver schemaResolver)
    {
        var typeDescription = __instance.Settings.ReflectionService.GetDescription(contextualType, __instance.Settings);

        JsonSchemaGenerator.ApplyTypeExtensionDataAttributes(schema, contextualType);

        if (__instance.TryHandleSpecialTypes(schema, typeDescription.ContextualType, schemaResolver))
        {
            __instance.ApplySchemaProcessors(schema, typeDescription.ContextualType, schemaResolver);
            return;
        }

        if (schemaResolver.RootObject == schema)
        {
            schema.Title = __instance.Settings.SchemaNameGenerator.Generate(typeDescription.ContextualType.Type);
        }

        if (typeDescription.Type.IsObject())
        {
            if (typeDescription.IsDictionary)
            {
                __instance.GenerateDictionary(schema, typeDescription, schemaResolver);
            }
            else
            {
                if (schemaResolver.HasSchema(typeDescription.ContextualType.Type, false))
                {
                    schema.Reference = schemaResolver.GetSchema(typeDescription.ContextualType.Type, false);
                }
                else if (schema.GetType() == typeof(JsonSchema))
                {
                    __instance.GenerateObject(schema, typeDescription, schemaResolver);
                }
                else
                {
                    schema.Reference = __instance.Generate(typeDescription.ContextualType, schemaResolver);
                }
            }
        }
        else if (typeDescription.IsEnum)
        {
            __instance.GenerateEnum(schema, typeDescription, schemaResolver);
        }
        else if (typeDescription.Type.IsArray()) // TODO: Add support for tuples?
        {
            __instance.GenerateArray(schema, typeDescription, schemaResolver);
        }
        else
        {
            typeDescription.ApplyType(schema);
        }

        if (contextualType != typeDescription.ContextualType)
        {
            __instance.ApplySchemaProcessors(schema, typeDescription.ContextualType, schemaResolver);
        }

        __instance.ApplySchemaProcessors(schema, contextualType, schemaResolver);
    }
}

/*
public interface Ix
{
    public float c { get; }
}

public class X : Ix
{
    public int a;
    public float c { get; }
}

public class Z : Ix
{
    public string b;
    public float c { get; }
}
*/
