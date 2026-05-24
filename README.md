# Container and Loot Patcher

A Synthesis patcher for Skyrim Special Edition that bundles a handful of independently-toggleable tweaks and compatibility patches for container and Leveled List loot. Each feature has its own toggle.

## Features

### Bypass Scarcity on boss chests and NPCs

[Scarcity - Less Loot Mod](https://www.nexusmods.com/skyrimspecialedition/mods/2304) reduces overall loot by giving most vanilla Leveled Lists a chance to roll nothing. Because those same Leveled Lists are shared between regular chests, enemies, and boss chests, you can't simply undo Scarcity on boss loot without also undoing it everywhere else.

This feature gives boss-tagged containers and NPCs (any record whose EditorID contains `Boss`) their own private Scarcity-free copy of the loot tree. End result: bosses drop full loot again while the rest of the world stays scarce.

Supported plugin names: `Scarcity SE - Less Loot Mod.esp`, `Scarcity - Less Loot Mod.esp`. If neither is present, this step is skipped.

### Remove Gold from Container item lists

Strips gold out of containers. Any container whose item list references gold (directly, or indirectly through a Leveled List) has those entries removed. Containers with mixed loot (gold alongside weapons, armor, etc.) only lose the gold portion; everything else is left alone.

### Remove Lockpicks from Container item lists

Same as gold removal, but for lockpicks.

### LockRelatedLoot compatibility

When using [LockRelatedLoot](https://www.nexusmods.com/skyrimspecialedition/mods/35156), it's possible that conflicting mods (or your Bashed Patch) can end up removing the script attachments that the mod makes. When `LockRelatedLoot.esp` is in the load order, this step copies any LockRelatedLoot scripts that aren't already present on the winning container override into our patch. Does nothing if there are no conflicts or issues. If `LockRelatedLoot.esp` is not present, this step is skipped.

## Exemptions

The gold and lockpick removal steps automatically skip a few categories of records to avoid breaking stuff. These exemptions apply regardless of which toggles are enabled.

- **Merchant chests** detected via the `MerchantContainer` field on Faction records. Vendor inventory is left alone so merchants still have their normal inventories.
- **Single-purpose containers** any container whose entire item list is gold and/or lockpicks. Coin purses, lockpick stashes, and similar dedicated containers keep their contents.
- **Perk- and quest-driven Leveled Lists** a small hardcoded list of vanilla records whose job is specifically to drop gold (e.g. the Transmute perk's gold drop and the Master Trader speech perk's extra gold). These are never classified as gold for purge purposes, and boss-bypass clones of them keep their original contents.

When the boss-bypass feature creates Scarcity-free clones, those clones honor the gold and lockpick removal toggles too--so you won't see purged loot reappearing on bosses through the back door.

## Settings

| Setting | Effect |
| --- | --- |
| Bypass Scarcity on boss chests and NPCs | Gives Boss-tagged containers and NPCs a Scarcity-free copy of their loot tree. |
| Remove Gold from Container item lists | Strips gold (direct and via pure-gold Leveled Lists) from container inventories. |
| Remove Lockpicks from Container item lists | Strips lockpicks (direct and via pure-lockpick Leveled Lists) from container inventories. |
| LockRelatedLoot compatibility | Propagates LockRelatedLoot's container scripts when a bashed patch drops them. |
