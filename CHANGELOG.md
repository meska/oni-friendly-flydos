# Changelog

## Unreleased

### Changed

- Moved water avoidance from the global mod options to each Soldering Station.
- Persisted the selected water policy on every produced Flydo.

### Fixed

- Made the Soldering Station labels visible on its light background and expanded checkbox rows to a reliable clickable width.
- Kept ONI's asynchronous path-prober classification stable when applying a Flydo water policy, preventing a repeating `KeyNotFoundException` after colony load.

## 0.1.0

- Added automatic safe power-bank selection and a configurable delivery priority.
- Added coordinated colony-minimum production at Soldering Stations.
- Added Flydo water avoidance with a drowning safety fallback.
- Added Flydos to the resource list for counting and click-to-focus navigation.
