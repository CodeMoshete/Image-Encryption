using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class AssetBuildTools
{
    private const string TEXTURE_DIR_RAW = "./Assets/Textures-Convert/Raw";
    private const string TEXTURE_DIR_ENCRYPTED = "./Assets/Textures-Convert/Encrypted";

    [MenuItem("Tools/Encrypt All Images")]
    public static void EncryptAllImages()
    {
        Debug.Log("Encrypting Images, please wait...");
        string[] directories = Directory.GetDirectories(TEXTURE_DIR_RAW);
        for (int i = 0, dirCt = directories.Length; i < dirCt; ++i)
        {
            string directorStr = directories[i];
            string[] images = Directory.GetFiles(directorStr);
            for (int j = 0, imgCt = images.Length; j < imgCt; ++j)
            {
                string imageStr = images[j];
                if (imageStr.EndsWith(".jpg") || imageStr.EndsWith(".png"))
                {
                    EncryptImage(imageStr);
                }
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("All images encrypted!");
    }

    private static void EncryptImage(string path)
    {
        path = path.Replace("./", string.Empty);
        Texture2D image = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        byte[] imageData = image.EncodeToPNG();
        byte[] encryptedData = AESEncryption.Encrypt(imageData);

        string outputPathSuffix = path.StringSplit("/Raw")[1];
        string extension = outputPathSuffix.StringSplit(".")[1];
        outputPathSuffix = outputPathSuffix.Replace(extension, "bytes");
        string outputPath = string.Format("{0}{1}", TEXTURE_DIR_ENCRYPTED, outputPathSuffix);
        string outputFolderPath = Path.GetDirectoryName(outputPath);
        bool directoryExists = Directory.Exists(outputFolderPath);
        if (!directoryExists)
        {
            Directory.CreateDirectory(outputFolderPath);
        }
        File.WriteAllBytes(outputPath, encryptedData);
    }

    [MenuItem("Tools/Bundle Encrypted Images")]
    static void BundleEncryptedImages()
    {
        // Define the output directory for the asset bundle
        string assetBundleDirectory = "Assets/Bundles";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        List<AssetBundleBuild> buildMap = new List<AssetBundleBuild>();

        string[] directories = Directory.GetDirectories(TEXTURE_DIR_ENCRYPTED);
        for (int i = 0, dirCt = directories.Length; i < dirCt; ++i)
        {
            // Define the folder containing the assets
            string folderPath = directories[i];
            string folderName = Path.GetFileName(folderPath);

            // Get all the assets in the folder
            string[] assetPaths = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);

            // Filter out meta files
            assetPaths = Array.FindAll(assetPaths, path => !path.EndsWith(".meta"));

            for (int j = 0, pathCt = assetPaths.Length; j < pathCt; ++j)
            {
                assetPaths[j] = assetPaths[j].Replace("./", "");
            }

            // Create a new AssetBundleBuild
            AssetBundleBuild build = new AssetBundleBuild
            {
                assetBundleName = folderName,
                assetNames = assetPaths
            };
            buildMap.Add(build);
        }

        // Build the asset bundle
        BuildPipeline.BuildAssetBundles(assetBundleDirectory,
            buildMap.ToArray(),
            BuildAssetBundleOptions.None,
            BuildTarget.WebGL);

        Debug.Log(string.Format("Asset bundle build complete!"));
    }
}
