# Changelog

## 0.2.13

- Preserve vanilla drowning behavior while a submerged Flydo waits for duplicant rescue.

## 0.2.12

- Remove the custom water-avoidance navigation grid, swim recovery, saved water policy, and station checkbox.
- Automatically ask a duplicant to carry each submerged living Flydo to the nearest dry reachable cell.
- Keep submerged Flydos safe from drowning while an automatic rescue is waiting or in progress.

## 0.2.11

- Allow duplicants to capture living Flydos while they are submerged, then carry the bagged robot to a chosen dry cell.
- Remove pending capture orders after a Flydo reaches dry ground, without making Flydos storable resources.

## 0.2.10

- Keep Flydos visible under Industrial Products without making storage compactors collect them.
- Release Flydos that earlier mod versions already placed inside storage when loading a colony.

## 0.2.9

- Fix the resource inventory count by using `WorldInventory.worldId` instead of the global inventory GameObject world.

## Unreleased

### Changed

- Moved water avoidance from the global mod options to each Soldering Station.
- Persisted the selected water policy on every produced Flydo.

### Fixed

- Count living Flydos as units instead of relying on their zero fetchable mass in the resource row.
- Populate the Flydo resource row before the All Resources screen builds its Industrial Products entries.
- Register Flydos loaded from existing saves directly with their world's resource inventory.
- Re-probe submerged water-avoiding Flydos and switch them to swim navigation, including after loading a save or becoming trapped by a changed cell.
- Let water-avoiding Flydos swim toward dry cells when they are already submerged, without allowing dry Flydos to plan routes into water.
- Corrected PLib checkbox callbacks so station options actually toggle, and shortened the status rows so the custom panel no longer widens the vanilla tabs.
- Made the Soldering Station labels visible on its light background and expanded checkbox rows to a reliable clickable width.
- Kept ONI's asynchronous path-prober classification stable when applying a Flydo water policy, preventing a repeating `KeyNotFoundException` after colony load.

## 0.1.0

- Added automatic safe power-bank selection and a configurable delivery priority.
- Added coordinated colony-minimum production at Soldering Stations.
- Added Flydo water avoidance with a drowning safety fallback.
- Added Flydos to the resource list for counting and click-to-focus navigation.
