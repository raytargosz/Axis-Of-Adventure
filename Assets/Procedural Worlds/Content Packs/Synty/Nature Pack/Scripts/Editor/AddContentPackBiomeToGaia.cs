using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
#if GAIA_2_PRESENT
using Gaia;
#endif

namespace ProceduralWorlds
{

    public class AddContentPackBiomeToGaia
    {
        private const string PW_DEV_FOLDER_NAME = "/DevUtils";
        private const string PW_FOLDER_NAME = "/Procedural Worlds";
        private const string BIOME_FOLDER_NAME = "/Nature Pack/Gaia Biomes";
        private const string SCRIPTS_FOLDER_NAME = "/Nature Pack/Scripts";
        private const string SCRIPTS_EDITOR_FOLDER_NAME = "/Nature Pack/Scripts/Editor";

        [InitializeOnLoadMethod]
        static void Onload()
        {
            // Need to wait for things to import before performing the biome check
            AssetDatabase.importPackageCompleted -= OnImportPackageCompleted;
            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;

            AssetDatabase.importPackageCancelled -= OnImportPackageCancelled;
            AssetDatabase.importPackageCancelled += OnImportPackageCancelled;

            AssetDatabase.importPackageFailed -= OnImportPackageFailed;
            AssetDatabase.importPackageFailed += OnImportPackageFailed;
        }
    

        /// <summary>
        /// Called when a package import is Completed.
        /// </summary>
        private static void OnImportPackageCompleted(string packageName)
        {
            OnPackageImport();
        }

        /// <summary>
        /// Called when a package import is Cancelled.
        /// </summary>
        private static void OnImportPackageCancelled(string packageName)
        {
            OnPackageImport();
        }

        /// <summary>
        /// Called when a package import fails.
        /// </summary>
        private static void OnImportPackageFailed(string packageName, string error)
        {
            OnPackageImport();
        }

        /// <summary>
        /// Used to run things after a package was imported.
        /// </summary>
        private static void OnPackageImport()
        {
            Check();

            // No need for these anymore
            AssetDatabase.importPackageCompleted -= OnImportPackageCompleted;
            AssetDatabase.importPackageCancelled -= OnImportPackageCancelled;
            AssetDatabase.importPackageFailed -= OnImportPackageFailed;
        }

        /// <summary>
        /// Checks if the biomes in this package need to be added
        /// </summary>
        private static void Check()
        { 
#if GAIA_2_PRESENT
            UserFiles userFiles = GaiaUtils.GetOrCreateUserFiles();
            if (userFiles != null)
            {
                string searchPath = "";
                foreach (var path in AssetDatabase.GetAllAssetPaths())
                {
                    if (path.EndsWith(BIOME_FOLDER_NAME))
                    {
                        searchPath = path; 
                    }
                }

                if (searchPath != "")
                {
                    string[] allSpawnerPresetGUIDs = AssetDatabase.FindAssets("t:BiomePreset", new string[1] { searchPath });

                    for (int i = 0; i < allSpawnerPresetGUIDs.Length; i++)
                    {
                        if (allSpawnerPresetGUIDs[i] != null)
                        {
                            BiomePreset bp = (BiomePreset)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(allSpawnerPresetGUIDs[i]), typeof(BiomePreset));
                            if (bp != null && !userFiles.m_gaiaManagerBiomePresets.Contains(bp))
                            {
                                userFiles.m_gaiaManagerBiomePresets.Add(bp);
                            }
                        }
                    }
                    EditorUtility.SetDirty(userFiles);
                    AssetDatabase.SaveAssets();
                }
            }
            SelfDestruct();
#else
    //nothing to do, self destruct 
    SelfDestruct();
#endif

        }

        /// <summary>
        /// Removes this script
        /// </summary>
        private static void SelfDestruct()
        {
            bool inDev = false;
            foreach (var path in AssetDatabase.GetAllAssetPaths())
            {
                if (path.EndsWith(PW_DEV_FOLDER_NAME))
                {
                    inDev = true;
                }
            }

            if (!inDev)
            {
                foreach (var path in AssetDatabase.GetAllAssetPaths())
                {
                    // If found this script under this products folder
                    if (path.EndsWith("AddContentPackBiomeToGaia.cs") && path.Contains(PW_FOLDER_NAME))
                    {
                        AssetDatabase.DeleteAsset(path);
                    }
                }

                foreach (var path in AssetDatabase.GetAllAssetPaths())
                {
                    // If found this script under this products folder
                    if (path.EndsWith(SCRIPTS_EDITOR_FOLDER_NAME) && path.Contains(PW_FOLDER_NAME))
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(GetFullFileSystemPath(path));
                        if (dirInfo != null && dirInfo.GetFiles().Length <= 0)
                        {
                            AssetDatabase.DeleteAsset(path);
                        }
                    }
                }

                foreach (var path in AssetDatabase.GetAllAssetPaths())
                {
                    // If found this script under this products folder
                    if (path.EndsWith(SCRIPTS_FOLDER_NAME) && path.Contains(PW_FOLDER_NAME))
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(GetFullFileSystemPath(path));
                        if (dirInfo != null && dirInfo.GetFiles().Length <= 0)
                        {
                            AssetDatabase.DeleteAsset(path);
                        }
                    }
                }
            }
        }

        private static string GetFullFileSystemPath(string inputPath)
        {
            return Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length) + inputPath;
        }
    }
}
