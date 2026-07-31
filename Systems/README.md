# Character systems

CharacterSystemML is one Google Sheet tab for five character systems:

- Passive: signature trigger effects.
- Equipment: persistent equipment effects with durability in Value and maximum durability in Limit.
- Idea: long-term philosophy passives.
- Emotion: base emotion and sub-emotion stages; use ParentCode for the base emotion.
- Divinity: deity resonance; use Tag for the deity identifier.

The required identity is CharacterID + Type + Code. Keep it unique. EffectClass names an existing Unity Buff class, but an empty value is valid while an effect is only being designed.

This directory stores the sheet contract and template. Actual C# effects remain split in Buffs/ and are merged into BuffCS.cs. Do not add a second effect runtime here: use the existing Buff lifecycle hooks first.

See the Unity project document Docs/CharacterSystemML.md for the full runtime and editor synchronization contract.
