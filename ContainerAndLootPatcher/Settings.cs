using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace ContainerAndLootPatcher;

public class Settings
{
    [MaintainOrder]

    [SettingName("Bypass Scarcity on boss chests and NPCs")]
    [Tooltip("When enabled and Scarcity - Less Loot Mod.esp is present in the load order, every Container and NPC whose EditorID contains \"Boss\" has its item entries that reference a Scarcity-gated Leveled List replaced with Scarcity-free clones. The clone duplicates the original Leveled List with its Global nulled, and recursively clones any sub-list that is also Scarcity-gated. Boss loot bypasses Scarcity while non-boss loot continues to roll against it normally.")]
    public bool BypassScarcityOnBossLoot { get; set; } = true;

    [SettingName("LockRelatedLoot compatibility")]
    [Tooltip("When enabled and LockRelatedLoot.esp is present in the load order, propagates every script entry that LockRelatedLoot adds to Container records into the patched record. Useful when a bashed patch fails to carry these scripts through.")]
    public bool LockRelatedLootCompatibility { get; set; } = true;

    [SettingName("Remove Gold from Container item lists")]
    [Tooltip("When enabled, any Container record whose item list references the Gold001 MiscItem (directly, or indirectly via a Leveled List that contains only gold) will have those entries removed.")]
    public bool RemoveGoldFromContainers { get; set; } = true;
}
