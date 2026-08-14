#!/bin/zsh
set -euo pipefail

project_dir="$(cd -- "$(dirname -- "$0")" && pwd)"
mod_dir="$HOME/Library/Application Support/unity.Klei.Oxygen Not Included/mods/Local/OniFriendlyFlydos"
dll_path="$project_dir/src/OniFriendlyFlydos/bin/Release/net48/OniFriendlyFlydos.dll"

# Prima compila, dopo toca la copia che carica el zogo.
dotnet build "$project_dir/src/OniFriendlyFlydos/OniFriendlyFlydos.csproj" -c Release
mkdir -p "$mod_dir"
cp "$project_dir/package/mod.yaml" "$mod_dir/mod.yaml"
cp "$project_dir/package/mod_info.yaml" "$mod_dir/mod_info.yaml"
cp "$dll_path" "$mod_dir/OniFriendlyFlydos.dll"

echo "Installed Friendly Flydos in: $mod_dir"
