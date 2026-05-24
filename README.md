# Container and Loot Patcher

A Synthesis patcher for Skyrim Special Edition that bundles a handful of independently-toggleable tweaks and compatibility patches for Container and Leveled List loot records.

Each feature is gated behind its own boolean setting and runs in isolation — enable any combination, leave the rest off.

## Features

### 1. Bypass Scarcity on boss chests and NPCs

The [Scarcity - Less Loot Mod](https://www.nexusmods.com/skyrimspecialedition/mods/2304) reduces overall loot by adding "loot chance" Global form records (`75LootChanceNone`, `50LootChanceNone`, `0LootChanceNone`, etc.) and attaching them to the `LVLG - Global` field on a large number of vanilla Leveled List records. The effect is a per-roll chance that the leveled list returns nothing.

The catch is that the same Leveled List records are referenced by regular chests, individual enemies, and boss chests alike. Simply nulling the Global on a boss-tagged Leveled List doesn't help — the master list contains other Scarcity-gated sub-lists that still roll for nothing. Worse, undoing the Global on those shared lists would also undo Scarcity for non-boss loot.

This feature solves that by giving boss containers/NPCs their own private branch of the Leveled List tree:

1. The patcher detects the Scarcity plugin by file name (`Scarcity SE - Less Loot Mod.esp` or `Scarcity - Less Loot Mod.esp`).
2. It builds a set of "Scarcity-gated" Leveled Lists: any Leveled List whose winning `Global` value originates from the Scarcity plugin.
3. For every `Container` and `Npc` whose EditorID contains `Boss`, each item entry that points to a Scarcity-gated Leveled List is replaced with a reference to a Scarcity-free **clone** of that list. The clone:
   - Has its own fresh FormKey in the patch.
   - Has EditorID `{OriginalEditorID}_CLP`.
   - Copies the original's flags, `ChanceNone`, and entries.
   - Has its `Global` field nulled out.
   - Has its sub-list entries recursively rewritten to point at Scarcity-free clones of any Scarcity-gated child lists.
4. Clones are deduplicated by original FormKey — a list reached from many bosses is cloned exactly once.

End result: boss-tagged containers and NPCs roll against a Scarcity-free copy of their loot tree, while every other container/NPC in the game still rolls against Scarcity's reduced tables.

### 2. Remove Gold from Container item lists

When enabled, any Container record whose inventory item list references gold — directly via the `Gold001` MiscItem, or indirectly via a Leveled List whose entries are *all* gold — will have those entries stripped out.

The patcher is anchored on the `Gold001` FormKey (`0000000F:Skyrim.esm`). It then iteratively expands a "gold-equivalent" set: any Leveled List whose every entry references something already in the set is itself added to the set. This continues until no more additions are found, naturally catching nested lists like `LootGoldChange` → `Gold001`, and `LootDwarvenGoldBoss` → `LootGoldChange` → `Gold001`. Any Leveled List that mixes gold with non-gold loot is left alone.

Finally every Container's `Items` list is swept for entries that reference any FormKey in the resolved set, and those entries are removed.

**Exceptions.** Some Leveled Lists in the game are intentionally all-gold but drive perk/quest/script behavior — stripping them would break those systems. These are kept on a hardcoded exclusion list and are never added to the gold set, so any container that references them keeps the entry. Current exclusions:

- `LootPerkGoldenTouch` (`0010FD8B:Skyrim.esm`) — Transmute perk's gold drop.
- `PerkMasterTraderGold` (`0010C1CC:Skyrim.esm`) — Master Trader speech perk's extra gold.

### 3. Remove Lockpicks from Container item lists

Identical mechanics to the gold removal step, but anchored on the `Lockpick` MiscItem FormKey (`0000000A:Skyrim.esm`). Container entries that reference `Lockpick` directly or any Leveled List that resolves to only lockpicks are stripped. Leveled Lists that mix lockpicks with other loot are left alone.

No exclusions are currently configured for the lockpick feature.

### 4. LockRelatedLoot compatibility

When enabled and `LockRelatedLoot.esp` is present in the load order, the patcher copies every script entry that LockRelatedLoot adds to Container records onto the winning override (i.e. into our patch). This works around bashed patches that fail to carry these script attachments through.

For each Container that LockRelatedLoot overrides, the patcher inspects the winning override's `VirtualMachineAdapter`. Any script in LockRelatedLoot's version that isn't already present on the winning override (matched by script name, case-insensitive) is deep-copied onto the patched record. Existing scripts are left untouched so we don't clobber other mods' script modifications.

If LockRelatedLoot is itself the winning override for a record, nothing is copied — its scripts are already winning.

## Settings

| Setting | Description |
| --- | --- |
| Bypass Scarcity on boss chests and NPCs | Toggle for the first tweak above. |
| Remove Gold from Container item lists | Toggle for the second tweak above. |
| Remove Lockpicks from Container item lists | Toggle for the third tweak above. |
| LockRelatedLoot compatibility | Toggle for the fourth tweak above. |

## Notes

- If the Scarcity plugin is not present in the load order, the boss-revert step is silently skipped. The gold-removal step still runs.
- The gold-removal step does not depend on Scarcity being installed.
- The LockRelatedLoot compatibility step only fires when `LockRelatedLoot.esp` is present.
