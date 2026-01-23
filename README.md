# GliderMod2

A Vintage Story mod that overhauls the glider's physics to provide a more intuitive, responsive, and exhilarating flight experience.

---

### Why This Mod?

The vanilla Vintage Story glider, while a great addition, can sometimes feel a bit sluggish or unpredictable. This mod aims to fix that by giving you more control over your descent and direction, making soaring through the valleys and over the peaks of Vintage Story feel less like a controlled fall and more like true flight.

---

### Key Features

* **Responsive Control:** Pitching up and down has a more immediate but slower descent rate. Gives a better soaring experience and prevents abusing it for jumps.
* **Enhanced Maneuverability:** Adjust your direction mid-air with greater precision.
* **Smoother Experience:** The new physics model reduces the "mushy" feel, making gliding a satisfying and consistent form of travel.
* **Dynamic Gliding:** Find a new balance between diving for speed and pulling up for distance.

---

### Installation

1.  Download the latest release of the mod from https://mods.vintagestory.at/show/mod/21867.
2.  Unzip the downloaded file.
3.  Place the entire folder into your Vintage Story `Mods` directory.
    * The default location for this folder is `%appdata%/Vintagestory/Mods` on Windows.
4.  Start the game and enjoy your improved gliding!

---

### Compatibility

This mod modifies the vanilla glider's core behavior. It should be compatible with most other mods, but may conflict with any mod that also directly alters glider physics.

---

### Roadmap

* Add a configuration file to allow players to fine-tune the glider's physics to their liking.
* Make this version a separate item with a more difficult build tree.

---

### Contributing

I welcome contributions to this mod! If you'd like to help, please feel free to open a pull request.

* **Reporting Bugs:** If you find a bug, please create a new issue.

## Isolated Development Guide (for Contributors)

### Quick Start

1. Prepare your system up to the environment variables section (you can skip env vars if your game is installed in the default location) by following the official guide:
   https://wiki.vintagestory.at/Modding:Preparing_For_Code_Mods
2. Clone or download this repository.
3. In the `Scripts` folder, run:

   **Windows**
   ```shell
   ./setupHere.ps1
   ```

   **Linux / macOS**
   ```shell
   ./setupHere.sh
   ```

4. (Optional) Pass `--defineEnvVar=true` so you don’t have to specify the test environment path every time:
   ```shell
   ./setupHere.ps1 --defineEnvVar=true
   ```

5. Open the solution in your IDE and do your coding magic in the `GlidierGlider` project.
6. Run the test script from the `Scripts` folder:

   **Windows**
   ```shell
   ./test.ps1
   ```

   **Linux / macOS**
   ```shell
   ./test.sh
   ```

7. Play around with your mod in the test environment.

---

### Testing Options

#### Custom Test Environment Location

If you want the test environment somewhere else, run `setup` with:

```shell
./setup.ps1 --testEnvPath="/path/to/test/env"
```

Example:
```shell
./setup.ps1 --testEnvPath="D:/UserFiles/Desktop/VintageTestEnvironment"
```

---

#### Using a Separate Game Instance

If you want to run a separate game instance, use `--useSeparateInstance=true` during setup and testing. This copies **game files only** (not user data) into the test environment.

```shell
./setup.ps1 --testEnvPath="D:/UserFiles/Desktop/VintageTestEnvironment" --useSeparateInstance=true
```

> [!Note]
> This also works with `setupHere`:
> ```shell
> ./setupHere.ps1 --useSeparateInstance=true
> ```

---

#### Persistent Mods in Test Environment

If you need additional mods that shouldn’t be wiped on every test run, put them into `Data/Mods` inside the test environment.

> [!Important]
> Even these mods will be removed if you run `setup` again in that environment.

---

### Scripts Overview

The `Scripts` folder contains several helper scripts:

- **build** — Runs the Cake build. With no arguments, builds the mod and puts it into the `Releases` folder.
- **package** — Same as `build`, just more verbose.
- **setup** — Creates a test environment. Supported arguments:
  - **(Required)** `--testEnvPath="D:/UserFiles/Desktop/VintageTestEnvironment"` — where to create the test environment
  - `--useSeparateInstance=true` — copies game files into the test environment
  - `--mainInstancePath="C:\Users\Username\AppData\Roaming\Vintagestory"` — path to an existing game install
  - `--defineEnvVar=true` — sets the **VINTAGE_STORY_TESTENV** environment variable
- **setupHere** — Same as `setup`, but uses `../TestEnv` as the test environment path (inside the solution)
  - Using `--defineEnvVar=true` is recommended for convenience
- **test** — Builds the mod, copies it into the test environment, and launches the game. Supported arguments:
  - `--testEnvPath="D:/UserFiles/Desktop/VintageTestEnvironment"`
  - `--useSeparateInstance=true`
  - `--mainInstancePath="C:\Users\Username\AppData\Roaming\Vintagestory"`

