using System.Reflection;
using HarmonyLib;
using Verse;

namespace Falloutization.Animals;

[StaticConstructorOnStartup]
internal static class HarmonyInit
{
    static HarmonyInit()
    {
        new Harmony("Falloutization.Animals").PatchAll(Assembly.GetExecutingAssembly());
    }
}
