# CyberQuest

CyberQuest is an educational cybersecurity game built with Unity. It combines story-driven quests, simulated applications and minigames with a globe-based interface. The project includes educational content, a content editor, a window system, progress tracking and save functionality.

## Requirements

- **Unity 2023.2.22f1**, specified in `ProjectSettings/ProjectVersion.txt`.
- **Python 3.10 or newer** for the dependency setup tool. No additional Python packages are required.
- The separately installed assets and packages listed below.

Package installation and local resource configuration are required before attempting to run the game. Unity import, compilation, builds, scene references and gameplay have not been validated; see [Known limitations](#known-limitations).

Build profiles are stored in `Assets/_GAME/Resources/BuildProfiles`. Their output folders are relative to the Unity project root and default to subfolders of `Builds/` (excluded from Git). Build, content-copy and folder-opening actions resolve these values from the project location, independently of the working directory. The folder picker saves a relative path and rejects locations outside the project. The content editor and its minigame testing scene remain included; obsolete sandbox, map screenshot and alternate globe scenes have been removed.

## Assets and dependencies

Obtain the following assets from their publishers and install them locally. Commercial assets require your own appropriate license; freely available packages also retain their own terms. CyberQuest does not supply or sublicense these dependencies. The setup tool customizes installed files but does not download, purchase or activate packages.

The version column records available version information, not verified Unity compatibility. For packages customized by the setup tool, compatibility is determined by the file hashes in `Tools/dependency_patches.json`. Newer releases may not match. Where the version is unknown, a matching download cannot currently be specified: use `--check` to verify an installation rather than assuming the latest release will work.

| Asset / publisher | Version information | Official source | Installation path and purpose |
| --- | --- | --- | --- |
| World Political Map Globe Edition / Kronnect | 19.0 indicated by package documentation; file-hash check required | [Kronnect](https://kronnect.com/products/world-map-globe/) | `Assets/WorldPoliticalMapGlobeEdition`; globe, markers and camera controls |
| World Map Strategy Kit 2 / Kronnect | 16.2 indicated by package documentation | [Kronnect store](https://store.kronnect.com/products/world-map-strategy-kit-2) | `Assets/WorldMapStrategyKit`; map integrations |
| Beam - Complete Fluent UI / Michsky | Exact version unknown; file-hash check required | [Michsky](https://michsky.com/portfolio/beam-complete-fluent-ui/) | `Assets/Beam - Complete Fluent UI`; UI components and resources |
| Graph And Chart / BitSplash Interactive | Exact version unknown; file-hash check required | [Unity Asset Store](https://assetstore.unity.com/packages/tools/gui/graph-and-chart-data-visualization-78488) | `Assets/Chart And Graph`; charts and radar labels |
| Odin Inspector and Serializer / Sirenix | Exact version unknown | [Official downloads](https://odininspector.com/download) | `Assets/Plugins/Sirenix`; inspector attributes and serialization |
| Auto Radial Layout / GIGA Softworks | 1.1.0; file-hash check required | [Unity Asset Store](https://assetstore.unity.com/packages/tools/gui/auto-radial-layout-293726) | `Assets/GIGA Softworks/Auto Radial Layout`; quest tree |
| Runtime Inspector & Hierarchy / yasirkula | Exact version unknown; file-hash check required | [Author's repository](https://github.com/yasirkula/UnityRuntimeInspector) | `Assets/Plugins/RuntimeInspector`; runtime content editing |
| Steamworks.NET / Riley Labrecque | 2025.163.0 | [Official release](https://github.com/rlabrecque/Steamworks.NET/releases/tag/2025.163.0) | `Assets/com.rlabrecque.steamworks.net`; Steam integration. Install the Unity package with its metadata |
| Simple Vector Icons / Unruly Games | Exact version unknown | [Unity Asset Store](https://assetstore.unity.com/packages/2d/gui/icons/simple-vector-icons-101218) | `Assets/_ASSETS/Simple Vector Icons`; UI icons |
| The Lab / Dark Fantasy Studio | Album identified; package version unknown | [Publisher's store](https://darkfantasystudio.itch.io/the-lab) | `Assets/_ASSETS/Audio/Dark Fantasy Studio - The Lab`; soundtrack. Loose audio downloads require manual assignment if Unity metadata is unavailable |
| TextMesh Pro resources / Unity | Resources for Unity 2023.2.22f1 / uGUI 2.0.0 | [Unity documentation](https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/manual/index.html) | `Assets/TextMesh Pro`; Essential Resources and fonts/materials referenced by the UI |
| Unity Web Browser / Voltstro | Browser, CEF engine and platform packages: 2.2.7 | [Official repository](https://github.com/Voltstro-Studios/UnityWebBrowser) | Installed through the registry declarations in `Packages`; in-game browser |

Unity Package Manager dependencies are declared in `Packages/manifest.json` and pinned in `Packages/packages-lock.json`. They include Newtonsoft.Json 3.2.1, Post Processing 3.4.0, Input System 1.7.0, uGUI 2.0.0, sprite 1.0.0, Timeline 1.8.6, Visual Scripting 1.8.0 and the browser's VoltRPC dependency. Keep both package files when setting up the project.

## Installation

1. Download the project and install the required Unity editor and Python version. Open the directory containing `Assets`, `Packages` and `ProjectSettings` as the Unity project.
2. Let Package Manager resolve the declared dependencies. Import TMP Essential Resources, Odin and Runtime Inspector, followed by the map, Beam, chart and radial-layout packages. Install Steamworks.NET, icons and music at the paths above.
3. Preserve package `.meta` files and package-supplied assembly definitions. These include Runtime Inspector's runtime assembly, the chart editor/runtime assemblies and the Steamworks.NET assemblies. Configure the resource locations below, moving folders together with their metadata and avoiding duplicate GUIDs.

   | Package | Resource layout |
   | --- | --- |
   | WorldPoliticalMapGlobeEdition | Place `Demos`, `Documentation` and `Resources` under `Assets/_ASSETS/WorldPoliticalMapGlobeEdition/`; keep code under `Assets/WorldPoliticalMapGlobeEdition/` |
   | WorldMapStrategyKit | Place `Documentation` and `Resources` under `Assets/_ASSETS/WorldMapStrategyKit/`; keep code under `Assets/WorldMapStrategyKit/` |
   | Beam - Complete Fluent UI | Supplementary editor images, scenes and textures use `Assets/_ASSETS/Beam - Complete Fluent UI/`. Keep the main package's required `Editor` code, `Scripts`, `Prefabs`, `Fonts` and `Resources` under `Assets/Beam - Complete Fluent UI/` |
   | Chart And Graph | Themes, tutorials, documentation and example scenes use `Assets/_ASSETS/Chart And Graph/`. Keep `Script`, `Editor` and `Prefabs` under `Assets/Chart And Graph/` |

4. Close Unity and other programs that may write package files. From the project root, check compatibility, apply the required customizations, then check the result:

   ```powershell
   python Tools/setup_dependencies.py --project . --check
   python Tools/setup_dependencies.py --project .
   python Tools/setup_dependencies.py --project . --check
   ```

   On Windows, `py -3` can replace `python` if the Python launcher is installed. Keep a backup of your locally installed packages before applying changes.
5. Reopen the project, configure the resources described under [Known limitations](#known-limitations), and complete the browser and Steam configuration below. Resolve missing scripts and resource assignments before attempting to run a scene.
6. The main entry scene is `Assets/_GAME/Scenes/GameScenes/MenuScene.unity`. Gameplay uses `GlobeScene_vNeon.unity` in the same folder. The content editor is `Assets/_GAME/Scenes/ExtensionScenes/DevContentEditorScene.unity`.

### Dependency setup tool

`--check` validates the package files without modifying them. It accepts compatible originals and already-customized files. Each package receives a status summary. Missing files, unsupported hashes or conflicting additions cause a nonzero exit before any writes. All packages covered by the tool must pass this check.

Applying the tool again leaves already-customized files unchanged. It preserves existing BOM and newline conventions and attempts rollback if a write fails. It does not install assets or configure Unity scene references.

| Package | Customizations required by CyberQuest |
| --- | --- |
| Runtime Inspector | Locked string/bool fields, informational labels and expandable arrays/types, with `LockedFieldAttribute`, `InfoStringAttribute` and `ExpandArrayAttribute` |
| Auto Radial Layout | Return the created `GameObject` from `AddNode` |
| Beam UI | Writable settings-description properties, sway reset on disable, button-click audio, disabled-panel coroutine handling and locked demo achievement indicators |
| Graph And Chart | Optional canvas creation for labels; radar labels opt out |
| Globe | W/A/S/D control directions in both navigation implementations |

If the tool reports an unsupported hash, obtain a matching package version from an authorized source. Do not bypass the hash check or apply the edits at guessed line positions. A successful check verifies only the files customized by this tool, not the complete package installation or the game's behavior.

## Configuration

### In-game browser

The browser uses `http://127.0.0.1:8080/` by default. Serve `Assets/_CONTENT/Web` as the web root to expose the included error page at `/error/`. You can run a local development server from the project root:

```powershell
python -m http.server 8080 --bind 127.0.0.1 --directory Assets/_CONTENT/Web
```

The error page is self-contained and has no external asset or network dependencies. Other training websites are not included. Provide the educational sites required by your content and configure `WebBrowserManager`'s `realPrefix`, `homeWebsite` and `errorWebsite` fields accordingly. An empty `Assets/_CONTENT/Config/links.links` file is available for local link configuration.

### Steam and data collection

For Steam functionality, supply your own authorized application configuration, including a local `steam_appid.txt` where required by the [Steamworks.NET setup instructions](https://steamworks.github.io/installation/). Keep application-specific configuration out of shared source changes.

Unity services and local heat-map recording are disabled by default.

## Project structure

| Location | Contents |
| --- | --- |
| `Assets/_GAME/Scripts` | Core systems, content loading, quests, minigames, UI, window management, saving and editor tools |
| `Assets/_GAME/Prefabs`, `Resources`, `Scenes` | Game objects, configuration assets and scenes |
| `Assets/_CONTENT` | Courses, quests, commands, mails, points of interest, simulated applications, illustrations, video definitions and web content |
| `Assets/_ASSETS` | Media and the resource locations used by locally installed packages |
| `Packages` | Unity Package Manager declarations and lockfile |
| `ProjectSettings` | Unity project settings |
| `Tools` | Dependency setup script, patch manifest and authored additions |

Educational content uses the folders `Courses`, `Quests`, `Commands`, `Mails`, `POIs`, `Apps`, `Grpahics`, `Videos`, `Config` and `Web`. Use the exact folder names when adding content.

## Simulated applications and educational screenshots

CyberQuest uses application screenshots as illustrative graphics for educational purposes. They provide visual context for guided, scripted interactions; they are not running copies of the depicted software and do not reproduce its full functionality. Actions and results are controlled by the educational scenario. Users do not need to install the real applications to use these simulations.

| Application | Content |
| --- | --- |
| eMailTrackerPro | Screenshot-based educational scenario, application icon and background |
| Zenmap | Screenshot-based educational scenario, application icon and background. Zenmap is the [official Nmap graphical interface](https://nmap.org/zenmap/) |
| WinHTTrack Website Copier | Application definition, icon and background; no guided scenario is included. [WinHTTrack](https://www.httrack.com/) is the Windows version of HTTrack |

Application names, logos, interface designs and screenshots belong to their respective rights holders where applicable. Their appearance identifies the software being illustrated and does not imply affiliation, sponsorship or endorsement. The educational-purpose description explains how the images are used; it does not itself grant permission to redistribute third-party material or rights in the depicted software.

## Known limitations

- **Unity compatibility is unverified.** Import, compilation, builds, scene references, platform/browser support and gameplay have not been validated. This is not a ready-to-run distribution.
- **Package versions must match the setup tool.** Exact release numbers and matching download availability are unknown for several dependencies. The tool's file checks do not establish compatibility of every resource in a package.
- **Some visual resources require local configuration.** Map imagery, UI textures, globe geometry/materials and province-frontier data are not supplied as a complete working scene setup. Assign suitable licensed resources or create the required globe/map objects from the installed publisher prefabs, then reconnect the scene integration fields. Automatic reference repair is not provided.
- **Customized presentation assets need local equivalents.** UI references can require Beam font atlases/glow fonts, colors and visual presets, prefab bindings for text-overflow hints, achievement presentation assets and the chart icon. Configure these locally or assign suitable licensed replacements.
- **Package metadata matters.** A different package release or loose-media download can have different GUIDs. Check missing references against the installed package's metadata and assign resources explicitly rather than rewriting GUIDs indiscriminately.
- **Training websites require separate hosting and content.** The included error page does not provide the educational websites used by browser-based activities.

## Code provenance and contributor permission

The minigame and window-system code was adapted from earlier games developed by **Lukasz Kusyk**. He intentionally reused his own code in CyberQuest and authorizes its inclusion and redistribution as part of CyberQuest.

This permission covers his own reused code in these systems, including implementations under `Assets/_GAME/Scripts/Minigames` and `Assets/_GAME/Scripts/UI/Windows`. It does not transfer copyright ownership or cover the earlier games as a whole, third-party code, packages, screenshots or other assets. Existing third-party notices and terms continue to apply.

## Credits

CyberQuest was developed with contributions from:

| Contributor | Credit |
| --- | --- |
| Ersin Dincelli | Principle Investigator (PI) |
| Lukasz Kusyk | Research Assistant (RA); developer |

## License status

A project-wide code/content license has not been selected, and no standalone project license file is included. The contributor permission above applies only to the identified reused code; it does not grant blanket permission for the rest of the project or third-party material.

Third-party components and locally installed packages retain their own terms and notices, including the public-domain notice in the Steam manager. The dependency setup tool does not grant redistribution rights for package assets.
