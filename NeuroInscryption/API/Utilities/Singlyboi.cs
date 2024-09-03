extern alias JetBrains;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace NeuroInscryption.API.Utilities;

internal abstract class Singlyboi<T> : Singlyboi
    where T : Singlyboi<T>
{
    public static T Instance { get; private set; }

    protected void Awake()
    {
        Instance = (T) this;
    }
}

internal abstract class Singlyboi : MonoBehaviour
{
    public static void AddAll(GameObject obj)
    {
        Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => !t.IsAbstract)
            .Where(t => typeof(Singlyboi).IsAssignableFrom(t))
            .Do(t => obj.AddComponent(t));
    }
}
