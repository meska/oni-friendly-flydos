#!/bin/zsh
set -euo pipefail

project_dir="$(cd -- "$(dirname -- "$0")" && pwd)"
mod_dir="$HOME/Library/Application Support/unity.Klei.Oxygen Not Included/mods/Local/OniFriendlyFlydos"
dll_path="$project_dir/src/OniFriendlyFlydos/bin/Release/net48/OniFriendlyFlydos.dll"

install_atomically() {
    local source_path="$1"
    local destination_path="$2"
    local temporary_path="${destination_path}.new.$$"

    # Mai tocar l'inode che Mono podarìa aver ancora carigà.
    cp "$source_path" "$temporary_path"
    chmod 0644 "$temporary_path"
    mv -f "$temporary_path" "$destination_path"
}

# Prima compila, dopo toca la copia che carica el zogo.
dotnet build "$project_dir/src/OniFriendlyFlydos/OniFriendlyFlydos.csproj" -c Release
mkdir -p "$mod_dir"
install_atomically "$project_dir/package/mod.yaml" "$mod_dir/mod.yaml"
install_atomically "$project_dir/package/mod_info.yaml" "$mod_dir/mod_info.yaml"
install_atomically "$dll_path" "$mod_dir/OniFriendlyFlydos.dll"

echo "Installed Friendly Flydos in: $mod_dir"
