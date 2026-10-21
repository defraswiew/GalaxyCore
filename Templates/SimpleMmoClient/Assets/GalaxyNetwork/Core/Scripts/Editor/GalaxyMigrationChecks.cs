using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GalaxyCoreCommon;
using GalaxyCoreCommon.Physics;
using GalaxyCoreLib;
using GalaxyCoreLib.NetEntity;
using GalaxyNetwork.Core.Scripts;
using GalaxyNetwork.Core.Scripts.NetEntity;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>Repeatable migration checks and player builds, callable with -executeMethod.</summary>
public static class GalaxyMigrationChecks
{
    private static string Artifacts => Path.GetFullPath(Path.Combine(Application.dataPath, "../../..", ".artifacts"));

    public static void Audit() => RunBatch(() =>
    {
        var report = new List<string> { "Unity " + Application.unityVersion };
        var errors = new List<string>();
        var setup = EditorSceneManager.GetSceneManagerSetup();
        try
        {
            foreach (string guid in AssetDatabase.FindAssets("t:Scene", new[] { "Assets" }).OrderBy(value => value))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                foreach (var root in scene.GetRootGameObjects()) CheckHierarchy(root, path, errors);
                // Round-trip every scene's physics bake through the real SDK validator.
                var bake = GalaxyNetwork.Core.Scripts.Editor.EditorGalaxyPhys.BakeLoadedScenes();
                var bytes = GalaxyPhysicsBakeFormat.Save(bake);
                var restored = GalaxyPhysicsBakeFormat.Load(bytes);
                if (restored.Colliders.Count != bake.Colliders.Count)
                    errors.Add(path + ": GPHB collider round-trip mismatch");
                string physicsFolder = Path.Combine(Artifacts, "physics");
                Directory.CreateDirectory(physicsFolder);
                File.WriteAllBytes(Path.Combine(physicsFolder, path.Replace("/", "_") + ".gphb"), bytes);
                report.Add(path + ": " + scene.GetRootGameObjects().Length + " roots, " + bake.Colliders.Count + " GPHB colliders");
            }
            foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = PrefabUtility.LoadPrefabContents(path);
                try { CheckHierarchy(prefab, path, errors); }
                finally { PrefabUtility.UnloadPrefabContents(prefab); }
            }
            foreach (string name in new[] { "GalaxyCoreCommon.dll", "GalaxyCoreLib.dll", "protobuf-net.dll", "YamlDotNet.dll", "SimpleMmoCommon.dll" })
            {
                var matches = Directory.GetFiles(Application.dataPath, name, SearchOption.AllDirectories);
                if (matches.Length != 1) errors.Add(name + ": expected one plugin, got " + matches.Length);
            }
            ValidateBuildScenes(errors);
            CheckIncomingBinding();
            report.Add("Incoming binding preserves entity identity, init data, owner, transform and scale.");
        }
        finally
        {
            if (setup.Any(scene => scene.isActive && scene.isLoaded)) EditorSceneManager.RestoreSceneManagerSetup(setup);
            else EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }
        report.AddRange(errors);
        Directory.CreateDirectory(Artifacts);
        File.WriteAllLines(Path.Combine(Artifacts, "unity-migration-audit.txt"), report);
        if (errors.Count != 0) throw new InvalidOperationException(string.Join("\n", errors));
        Debug.Log("GALAXY_MIGRATION_AUDIT_OK\n" + string.Join("\n", report));
    });

    public static void BakeNavigationAssets() => RunBatch(() =>
    {
        const string source = "Assets/GalaxyNetwork/Examples/Scenes/NavigationExample.gnm";
        const string folder = "Assets/GalaxyNetwork/Core/Alpha/Navigation/Data";
        Directory.CreateDirectory(folder);
        var scenes = new[]
        {
            "Assets/GalaxyNetwork/Examples/Scenes/ExampleNav.unity",
            "Assets/GalaxyNetwork/Examples/Scenes/Navigation.unity",
            "Assets/GalaxyNetwork/Core/Alpha/Navigation/Scenes/Navigation.unity"
        };
        foreach (string scenePath in scenes)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            var baker = UnityEngine.Object.FindFirstObjectByType<GalaxyNetwork.Core.Alpha.Navigation.GalaxyMapBaker>();
            if (baker == null) throw new InvalidOperationException("Navigation baker missing: " + scenePath);
            baker.Map = BaseMessage.Deserialize<GalaxyCoreCommon.Navigation.GalaxyMap>(File.ReadAllBytes(source));
            if (scenePath.Contains("/Core/"))
            {
                // The original Navigation2.gnm lived outside the repository. Recover the
                // authoring scene's explicit platform/obstacle assignments as named demo layers.
                foreach (byte id in baker.Layers.Distinct().Where(id => id != 0))
                {
                    if (baker.Map.Layers.GetById(id) != null) continue;
                    baker.Map.Layers.List.Add(new GalaxyCoreCommon.Navigation.GalaxyMapLayer
                    {
                        Id = id, Name = id == 6 ? "Demo platforms" : "Demo obstacles",
                        IsWalkable = id == 6, Transparent = true,
                        R = id == 6 ? (byte)50 : (byte)220, G = id == 6 ? (byte)180 : (byte)80, B = 80
                    });
                    Debug.Log("Recovered authoring demo navigation layer " + id + " (external original Navigation2.gnm was never included).");
                }
            }
            if (scenePath.Contains("/Core/"))
            {
                ConfigureAuthoringCache(baker);
                if (!File.Exists(baker.ResolveBakePath()))
                {
                    Physics.SyncTransforms();
                    baker.Baked();
                }
                EditorUtility.SetDirty(baker);
                EditorSceneManager.SaveScene(scene);
                Debug.Log("GALAXY_NAVIGATION_AUTHORING_CACHE_OK: " + baker.ResolveBakePath());
                continue;
            }
            string bakePath = folder + "/" + (scenePath.Contains("/Core/") ? "NavigationAuthoring" : Path.GetFileNameWithoutExtension(scenePath)) + ".gnm.bytes";
            if (scenePath.EndsWith("/Examples/Scenes/Navigation.unity", StringComparison.Ordinal)) File.Copy(source, bakePath, true);
            else
            {
                baker.Path = bakePath;
                Physics.SyncTransforms();
                baker.Baked();
                if (baker.Map.Nodes.Count == 0) throw new InvalidOperationException("Navigation bake has no nodes: " + scenePath);
            }
            AssetDatabase.ImportAsset(bakePath, ImportAssetOptions.ForceSynchronousImport);
            baker.BakedMap = AssetDatabase.LoadAssetAtPath<TextAsset>(bakePath);
            if (baker.BakedMap == null) throw new InvalidOperationException("Baked map failed to import: " + bakePath);
            baker.Path = bakePath;
            EditorUtility.SetDirty(baker);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("GALAXY_NAVIGATION_BAKE_OK: " + scenePath + " -> " + bakePath);
        }
        AssetDatabase.SaveAssets();
    });

    private static void ConfigureAuthoringCache(GalaxyNetwork.Core.Alpha.Navigation.GalaxyMapBaker baker)
    {
        const string layerPath = "Assets/GalaxyNetwork/Core/Alpha/Navigation/Data/NavigationAuthoringLayers.bytes";
        var configuration = new GalaxyCoreCommon.Navigation.GalaxyMap { Layers = baker.Map.Layers };
        File.WriteAllBytes(layerPath, configuration.Serialize());
        AssetDatabase.ImportAsset(layerPath, ImportAssetOptions.ForceSynchronousImport);
        baker.LayerConfiguration = AssetDatabase.LoadAssetAtPath<TextAsset>(layerPath);
        baker.BakedMap = null;
        baker.Path = "../../.artifacts/navigation/NavigationAuthoring.gnm";
        baker.BakeInEditorOnPlay = true;
    }

    public static void FinalizeNavigationCache() => RunBatch(() =>
    {
        const string scenePath = "Assets/GalaxyNetwork/Core/Alpha/Navigation/Scenes/Navigation.unity";
        const string sourcePath = "Assets/GalaxyNetwork/Core/Alpha/Navigation/Data/NavigationAuthoring.gnm.bytes";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        var baker = UnityEngine.Object.FindFirstObjectByType<GalaxyNetwork.Core.Alpha.Navigation.GalaxyMapBaker>();
        if (baker == null) throw new InvalidOperationException("Authoring navigation baker missing.");
        if (File.Exists(sourcePath))
        {
            baker.Map = BaseMessage.Deserialize<GalaxyCoreCommon.Navigation.GalaxyMap>(File.ReadAllBytes(sourcePath));
            ConfigureAuthoringCache(baker);
            Directory.CreateDirectory(Path.GetDirectoryName(baker.ResolveBakePath()));
            File.Copy(sourcePath, baker.ResolveBakePath(), true);
        }
        else if (baker.LayerConfiguration == null) throw new InvalidOperationException("Authoring layer definitions missing.");
        EditorUtility.SetDirty(baker);
        EditorSceneManager.SaveScene(scene);
        if (File.Exists(sourcePath)) AssetDatabase.DeleteAsset(sourcePath);
        AssetDatabase.SaveAssets();
        Debug.Log("GALAXY_NAVIGATION_AUTHORING_CACHE_OK: " + baker.ResolveBakePath());
    });

    public static void ExportDependencies() => RunBatch(() =>
    {
        var roots = new HashSet<string>(EditorBuildSettings.scenes.Select(scene => scene.path), StringComparer.Ordinal);
        foreach (string path in AssetDatabase.GetAllAssetPaths())
        {
            if (!path.StartsWith("Assets/", StringComparison.Ordinal)) continue;
            string extension = Path.GetExtension(path).ToLowerInvariant();
            if (path.Contains("/Resources/") || path.EndsWith("/Resources", StringComparison.Ordinal) ||
                path.Contains("/Editor/") || extension == ".cs" || extension == ".dll" ||
                extension == ".xml" || extension == ".gnm") roots.Add(path);
        }
        roots.Add("Assets/GalaxyNetwork/Core/Alpha/Navigation/Scenes/Navigation.unity");
        var dependencies = AssetDatabase.GetDependencies(roots.Where(path => !string.IsNullOrEmpty(path)).ToArray(), true)
            .Where(path => path.StartsWith("Assets/", StringComparison.Ordinal)).OrderBy(path => path, StringComparer.Ordinal).ToArray();
        Directory.CreateDirectory(Artifacts);
        File.WriteAllLines(Path.Combine(Artifacts, "unity-asset-dependencies.txt"), dependencies);
        File.WriteAllLines(Path.Combine(Artifacts, "unity-asset-roots.txt"), roots.OrderBy(path => path, StringComparer.Ordinal));
        Debug.Log("GALAXY_ASSET_DEPENDENCIES_OK: " + roots.Count + " roots, " + dependencies.Length + " dependencies");
    });

    private static void CheckHierarchy(GameObject root, string assetPath, List<string> errors)
    {
        foreach (var transform in root.GetComponentsInChildren<Transform>(true))
        {
            var meshFilter = transform.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh == null) errors.Add(assetPath + ": " + transform.name + " has a missing mesh");
            int missing = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject);
            if (missing != 0) errors.Add(assetPath + ": " + transform.name + " has " + missing + " missing scripts");
        }
    }

    private static void CheckIncomingBinding()
    {
        var connection = new GalaxyConnection(new Config { AppName = "SimpleMmoServer", ServerIp = "127.0.0.1", ServerPort = 30200 });
        var incoming = new ClientNetEntity(connection)
        {
            NetID = 123, OwnerClientId = 777, PrefabName = "MigrationCheck", InitData = new byte[] { 7, 8, 9 }
        };
        incoming.transform.Position = new GalaxyVector3(1, 2, 3);
        incoming.transform.LocalScale = new GalaxyVector3(2, 3, 4);
        var go = new GameObject("Galaxy binding check");
        go.SetActive(false);
        try
        {
            var view = go.AddComponent<UnityNetEntity>();
            view.Bind(incoming);
            if (!ReferenceEquals(view.NetEntity, incoming) || !ReferenceEquals(view.Data, incoming.InitData) ||
                view.NetEntity.OwnerClientId != 777 || view.NetEntity.transform.Position.Y != 2 ||
                view.NetEntity.transform.LocalScale.Z != 4)
                throw new InvalidOperationException("Incoming entity binding lost initial state.");
        }
        finally { UnityEngine.Object.DestroyImmediate(go); }
    }

    private static void ValidateBuildScenes(List<string> errors)
    {
        var enabled = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
        foreach (string name in new[] { "Main", "ExamplePhys", "ExamplePhys2", "ExampleOcto", "ExampleNav" })
            if (!enabled.Any(path => Path.GetFileNameWithoutExtension(path) == name))
                errors.Add("Required demo scene is absent from enabled Build Settings: " + name);
        foreach (string path in enabled)
            if (!File.Exists(path)) errors.Add("Missing enabled build scene: " + path);
    }

    public static void BuildWindowsMono() => BuildWindows(ScriptingImplementation.Mono2x, "mono");
    public static void BuildWindowsIl2Cpp() => BuildWindows(ScriptingImplementation.IL2CPP, "il2cpp");

    private static void BuildWindows(ScriptingImplementation backend, string folder) => RunBatch(() =>
    {
        var errors = new List<string>();
        ValidateBuildScenes(errors);
        if (errors.Count != 0) throw new InvalidOperationException(string.Join("\n", errors));
        var originalBackend = PlayerSettings.GetScriptingBackend(NamedBuildTarget.Standalone);
        try
        {
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, backend);
            string output = Path.Combine(Artifacts, "build", "windows-" + folder, "GalaxyExamples.exe");
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            var result = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray(),
                locationPathName = output, target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.StrictMode
            });
            Directory.CreateDirectory(Artifacts);
            File.WriteAllText(Path.Combine(Artifacts, "unity-build-" + folder + ".txt"),
                result.summary.result + "\nErrors: " + result.summary.totalErrors + "\nWarnings: " + result.summary.totalWarnings +
                "\nBytes: " + result.summary.totalSize + "\nOutput: " + output);
            if (result.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException("Galaxy Windows " + folder + " build failed: " + result.summary.totalErrors + " errors.");
            Debug.Log("GALAXY_WINDOWS_BUILD_OK: " + output);
        }
        finally { PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, originalBackend); }
    });

    private static void RunBatch(Action action)
    {
        try
        {
            action();
            if (Application.isBatchMode) EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            if (Application.isBatchMode) EditorApplication.Exit(1);
            else throw;
        }
    }
}
