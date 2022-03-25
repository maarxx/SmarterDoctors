using System.Reflection;
using HarmonyLib;
using Verse;

namespace SmarterDoctors;

[StaticConstructorOnStartup]
internal class Main
{
    static Main()
    {
        //Log.Message("Hello from Harmony in scope: com.github.harmony.rimworld.maarx.smarterdoctors");
        var harmony = new Harmony("com.github.harmony.rimworld.maarx.smarterdoctors");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}