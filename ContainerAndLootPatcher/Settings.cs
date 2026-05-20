using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace ContainerAndLootPatcher;

public class Settings
{
    [MaintainOrder]

    [SettingName("Revert Scarcity changes on 'Boss' Leveled Items")]
    [Tooltip("When enabled and Scarcity - Less Loot Mod.esp\" is present in the load order, reverts Scarcity's changes to any container that has \"Boss\" in the name, restoring normal Boss loot.")]
    public bool RevertBossChestChanges { get; set; } = true;

    [SettingName("LockRelatedLoot compatibility")]
    [Tooltip("When enabled and LockRelatedLoot.esp is present in the load order, propagates every script entry that LockRelatedLoot adds to Container records into the patched record. Useful when a bashed patch fails to carry these scripts through.")]
    public bool LockRelatedLootCompatibility { get; set; } = true;

    [SettingName("Remove Gold from Container item lists")]
    [Tooltip("When enabled, any Container record whose item list references the Gold001 MiscItem (directly, or indirectly via a Leveled Item that contains only gold) will have those entries removed.")]
    public bool RemoveGoldFromContainers { get; set; } = true;
}
