# Friendly Flydos

Friendly Flydos removes the repetitive setup around ONI's Flydo robots while preserving the game's normal chores and priorities.

## Features

- Automatically selects safe rechargeable and raw-metal power banks on new Flydos.
- Gives each Flydo a visible battery-delivery priority, defaulting to 7.
- Adds a Soldering Station side screen that maintains a shared minimum number of living Flydos per asteroid.
- Coordinates multiple enabled stations so they do not all reserve materials for the same deficit.
- Lets each Soldering Station decide whether the Flydos it produces avoid swimming paths and receive drowning protection.
- Lets duplicants capture living Flydos while submerged so stranded robots can be recovered.
- Adds Flydos to Industrial Products in the resource list, where a pinned row can cycle through and focus every active Flydo.

Atomic power banks are excluded from automatic selection by default. The mod options can include them when desired.

Automatic production is coordinated per asteroid. The highest target among participating Soldering Stations is the effective shared target. While participation is enabled, the mod owns that station's Flydo recipe queues and may replace or clear manual Flydo orders; disable participation before managing those queues manually.

Water avoidance is stored per Flydo. The producing station's checkbox sets the policy for new Flydos, while Flydos from older Friendly Flydos saves keep water avoidance enabled for compatibility. Water-avoiding Flydos do not plan routes into liquid, but can still swim to safety if they are pushed or spawned underwater.

Submerged Flydos can be marked for capture and carried to safety by a duplicant. Capture is disabled again once the Flydo reaches dry ground; this rescue path does not make Flydos eligible for storage compactors.

## Building

The project targets .NET Framework 4.8 and references the local Oxygen Not Included managed assemblies.

```sh
dotnet test
dotnet build src/OniFriendlyFlydos/OniFriendlyFlydos.csproj -c Release
```
