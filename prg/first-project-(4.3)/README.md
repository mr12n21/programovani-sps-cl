# first-project

Godot 4.3 C# project targeting .NET 8.

## Run on Fedora

1. Install the .NET 8 SDK:

```bash
sudo dnf install dotnet-sdk-8.0
```

2. Install a Godot 4 build with .NET support.

If your executable is already in PATH as `godot4`, set:

```bash
export GODOT4="$(command -v godot4)"
```

If you downloaded the official Linux .NET build manually, point `GODOT4` to that file instead:

```bash
export GODOT4="$HOME/Apps/Godot_v4.3-stable_mono_linux_x86_64"
chmod +x "$GODOT4"
```

If you already have the official archive in `~/Downloads`, extract it like this:

```bash
mkdir -p "$HOME/Apps"
unzip -q -o "$HOME/Downloads/Godot_v4.6-stable_mono_linux_x86_64.zip" -d "$HOME/Apps"
export GODOT4="$HOME/Apps/Godot_v4.6-stable_mono_linux_x86_64/Godot_v4.6-stable_mono_linux.x86_64"
chmod +x "$GODOT4"
```

3. Run the project:

```bash
./run-fedora.sh
```

## Run from VS Code

The launch configuration in `.vscode/launch.json` uses the `GODOT4` environment variable.

After setting `GODOT4`, open this project folder in VS Code and run the `Godot Run` configuration.