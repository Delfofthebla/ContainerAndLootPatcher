# Container and Loot Patcher

A Synthesis patcher for Skyrim Special Edition that bundles a handful of independently-toggleable tweaks and compatibility patches for Container and Leveled Item loot records.

Each feature is gated behind its own boolean setting and runs in isolation — enable any combination, leave the rest off.

## Features

### 1. Revert Scarcity changes on "Boss" Leveled Items

The [Scarcity - Less Loot Mod](https://www.nexusmods.com/skyrimspecialedition/mods/2304) reduces overall loot by adding "loot chance" Global form records (`75LootChanceNone`, `50LootChanceNone`, `0LootChanceNone`, etc.) and attaching them to the `LVLG - Global` field on a large number of vanilla Leveled Item records. The effect is a per-roll chance that the leveled list returns nothing.

When this feature is enabled, any Leveled Item record whose EditorID contains the substring `Boss` will have Scarcity's added `LVLG - Global` field reverted to whatever value the record had *immediately before* Scarcity was applied (typically empty/null on vanilla records). This leaves end-of-dungeon boss chest loot lists untouched by Scarcity while still letting Scarcity reduce all other loot.

The patcher walks each record's override chain, locates Scarcity's contribution, and uses the next-lower-priority entry as the revert target. The Scarcity plugin is detected by file name — currently the patcher recognizes:

- `Scarcity SE - Less Loot Mod.esp`
- `Scarcity - Less Loot Mod.esp`

The revert only fires when the winning `Global` value still matches what Scarcity itself set. This means:

- Bashed patches and other patch .esps that take Scarcity as a master are handled correctly — they just carry Scarcity's value through, so the revert still applies.
- If a mod higher in the load order than Scarcity has *intentionally* set a different `Global` value, that intent is respected and the record is left alone.

### 2. Remove Gold from Container item lists

When enabled, any Container record whose inventory item list references gold — directly via the `Gold001` MiscItem, or indirectly via a Leveled Item whose entries are *all* gold — will have those entries stripped out.

The patcher is anchored on the `Gold001` EditorID. It then iteratively expands a "gold-equivalent" set: any Leveled Item whose every entry references something already in the set is itself added to the set. This continues until no more additions are found, naturally catching nested lists like `LootGoldChange` → `Gold001`, and `LootDwarvenGoldBoss` → `LootGoldChange` → `Gold001`. Any Leveled Item that mixes gold with non-gold loot is left alone.

Finally every Container's `Items` list is swept for entries that reference any FormKey in the resolved set, and those entries are removed.

### 3. LockRelatedLoot compatibility

When enabled and `LockRelatedLoot.esp` is present in the load order, the patcher copies every script entry that LockRelatedLoot adds to Container records onto the winning override (i.e. into our patch). This works around bashed patches that fail to carry these script attachments through.

For each Container that LockRelatedLoot overrides, the patcher inspects the winning override's `VirtualMachineAdapter`. Any script in LockRelatedLoot's version that isn't already present on the winning override (matched by script name, case-insensitive) is deep-copied onto the patched record. Existing scripts are left untouched so we don't clobber other mods' script modifications.

If LockRelatedLoot is itself the winning override for a record, nothing is copied — its scripts are already winning.

## Settings

| Setting | Description |
| --- | --- |
| Revert Scarcity changes on 'Boss' Leveled Items | Toggle for the first tweak above. |
| Remove Gold from Container item lists | Toggle for the second tweak above. |
| LockRelatedLoot compatibility | Toggle for the third tweak above. |
| Ignored mods | Substrings of plugin file names to skip when patching. |

## Notes

- If the Scarcity plugin is not present in the load order, the boss-revert step is silently skipped. The gold-removal step still runs.
- The gold-removal step does not depend on Scarcity being installed.
- The LockRelatedLoot compatibility step only fires when `LockRelatedLoot.esp` is present.
