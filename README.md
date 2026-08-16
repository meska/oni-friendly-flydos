# Friendly Flydos

Friendly Flydos removes the repetitive setup around ONI's Flydo robots while preserving the game's normal chores and priorities.

## Features

- Automatically selects safe rechargeable and raw-metal power banks on new Flydos.
- Gives each Flydo a visible priority for battery delivery and emergency rescue, defaulting to 7.
- Adds a Soldering Station side screen that maintains a shared minimum number of living Flydos per asteroid.
- Coordinates multiple enabled stations so they do not all reserve materials for the same deficit.
- Automatically asks a duplicant to carry submerged Flydos to the nearest dry reachable cell.
- Adds Flydos to Industrial Products in the resource list, where a pinned row can cycle through and focus every active Flydo.

Atomic power banks are excluded from automatic selection by default. The mod options can include them when desired.

Automatic production is coordinated per asteroid. The highest target among participating Soldering Stations is the effective shared target. While participation is enabled, the mod owns that station's Flydo recipe queues and may replace or clear manual Flydo orders; disable participation before managing those queues manually.

Friendly Flydos does not replace the vanilla Flydo navigation grid. When a living Flydo is submerged, the mod finds the nearest dry cell reachable by duplicants and creates a normal move chore automatically. Vanilla drowning behavior remains active while the Flydo waits, and the Flydo remains excluded from storage compactors.

## Building

The project targets .NET Framework 4.8 and references the local Oxygen Not Included managed assemblies.

```sh
dotnet test
dotnet build src/OniFriendlyFlydos/OniFriendlyFlydos.csproj -c Release
```
