# i like my expie jiggly bruh

A silly experimental mod addon for CasualtiesXL that attempts poorly to add soft body physics to good ol' Expie. Heavily based on [WgMod](https://github.com/follycake/WgMod) for Terraria!! Check them out.

The mod requires the latest experimental build of CasualtiesXL.

## Compiling

Make sure you have the latest .NET SDK installed, then run:

```bash
dotnet build -c release
```

Then pop the compiled DLL file into the BepInEx/plugins folder and play the game!!!

## Known issues

THIS IS A HEAVILY EXPERIMENTAL MOD. It's far from finished and any feedback, pull requests, and other contributions are heavily encouraged. I'm a beginner modder, especially with Unity, so bear with me! That being said, here are some current issues I got my head scratching about:

- Expie's sprite gets glitched out when wedged in holes.
- Expie's higher sizes may appear blockier in some scenarios. I'm working on trying to make him appear softer in these scenarios.
- Debug messages (mesh diagnostics, wobble/wall probes, etc.) are logged in the terminal while tuning the values. They are now toggleable via the `DebugEnabled` option in the `General` section of the BepInEx config (off by default; warnings and errors always show).
