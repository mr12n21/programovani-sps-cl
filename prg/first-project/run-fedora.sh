#!/usr/bin/env bash

set -euo pipefail

project_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

declare -a godot_cmd=()

resolve_godot() {
	local env_candidate="${GODOT4:-}"
	local candidate=""

	if [[ -n "$env_candidate" ]]; then
		if [[ -x "$env_candidate" ]]; then
			godot_cmd=("$env_candidate")
			return 0
		fi

		echo "Ignoring invalid GODOT4 path: $env_candidate" >&2
	fi

	for candidate in \
		"$(command -v godot4 2>/dev/null || true)" \
		"$(command -v godot 2>/dev/null || true)" \
		"$HOME/Apps/Godot_v4.6-stable_mono_linux_x86_64/Godot_v4.6-stable_mono_linux.x86_64" \
		"$HOME/Downloads/Godot_v4.6-stable_mono_linux_x86_64/Godot_v4.6-stable_mono_linux.x86_64"
	do
		if [[ -n "$candidate" && -x "$candidate" ]]; then
			godot_cmd=("$candidate")
			return 0
		fi
	done

	if command -v flatpak >/dev/null 2>&1 && flatpak info org.godotengine.GodotSharp >/dev/null 2>&1; then
		godot_cmd=(flatpak run org.godotengine.GodotSharp)
		return 0
	fi

	return 1
}

if ! command -v dotnet >/dev/null 2>&1; then
	echo "Missing .NET SDK. On Fedora install it with: sudo dnf install dotnet-sdk-8.0" >&2
	exit 1
fi

if ! resolve_godot; then
	echo "Missing Godot executable. Install a Godot 4 .NET build, or extract ~/Downloads/Godot_v4.6-stable_mono_linux_x86_64.zip to ~/Apps." >&2
	exit 1
fi

echo "Building C# project..."
dotnet build "$project_dir/first-project.sln"

echo "Starting Godot via: ${godot_cmd[*]}"
echo "If this is not a .NET-enabled Godot build, the project will not load correctly."
exec "${godot_cmd[@]}" --path "$project_dir" "$@"