using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ybwork.Assets.Editor
{
    [Serializable]
    public class AssetCollectorItemData
    {
        public string AssetGUID;
        public string AssetPath
        {
            get => AssetDatabase.GUIDToAssetPath(AssetGUID);
            set
            {
                AssetGUID = AssetDatabase.AssetPathToGUID(value);
                assets = null;
                repeatedAssets = null;
            }
        }

        private IEnumerable<AssetAlias> assets;
        public IEnumerable<AssetAlias> Assets
        {
            get
            {
                assets ??= GetAssets();
                return assets;
            }
        }

        private string[] repeatedAssets;
        public string[] RepeatedAssets
        {
            get
            {
                if (repeatedAssets == null || repeatedAssets.Length == 0)
                    repeatedAssets = Assets
                        .GroupBy(item => item.Name)
                        .Where(group => group.Count() > 1)
                        .Select(group => group.Key)
                        .ToArray();
                return repeatedAssets;
            }
        }

        private IEnumerable<AssetAlias> GetAssets()
        {
            var assetPath = AssetPath;
            if (string.IsNullOrEmpty(AssetPath))
                yield break;
            else if (AssetDatabase.IsValidFolder(assetPath))
            {
                string[] guids = AssetDatabase.FindAssets(string.Empty, new string[] { assetPath });
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    // 如果路径为文件夹，则忽略此路径
                    if (!AssetDatabase.IsValidFolder(path))
                    {
                        string name = Path.GetFileNameWithoutExtension(path);
                        yield return new AssetAlias
                        {
                            Name = name,
                            AssetPath = path
                        };
                    }
                }
            }
            else
            {
                string name = Path.GetFileNameWithoutExtension(assetPath);
                yield return new AssetAlias
                {
                    Name = name,
                    AssetPath = assetPath
                };
            }
        }

        internal IEnumerable<AssetAlias> GetAssets(string bundleName)
        {
            var assetPath = AssetPath;
            if (string.IsNullOrEmpty(AssetPath))
                yield break;
            else if (AssetDatabase.IsValidFolder(assetPath))
            {
                string[] guids = AssetDatabase.FindAssets(string.Empty, new string[] { assetPath });
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    // 如果路径为文件夹，则忽略此路径
                    if (!AssetDatabase.IsValidFolder(path))
                    {
                        string name = Path.GetFileNameWithoutExtension(path);
                        yield return new AssetAlias
                        {
                            Name = name,
                            BundleName = bundleName,
                            AssetPath = path
                        };
                    }
                }
            }
            else
            {
                string name = Path.GetFileNameWithoutExtension(assetPath);
                yield return new AssetAlias
                {
                    Name = name,
                    BundleName = bundleName,
                    AssetPath = assetPath
                };
            }
        }
    }

    [Serializable]
    public class AssetCollectorGroupData
    {
        public string GroupName;
        public List<AssetCollectorItemData> Items = new();

        public IEnumerable<string> GetAssets()
        {
            return Items.SelectMany(item => item.Assets.Select(asset => asset.AssetPath));
        }

        public IEnumerable<AssetAlias> GetAssetPaths()
        {
            return Items.SelectMany(item => item.GetAssets(GroupName));
        }
    }

    [Serializable]
    public class AssetCollectorPackageData
    {
        public string PackageName;
        public List<AssetCollectorGroupData> Groups = new();

        public IEnumerable<AssetAlias> GetAssetPaths()
        {
            return Groups.SelectMany(group => group.GetAssetPaths());
        }
    }

    [CreateAssetMenu(menuName = "ybwork/Assets/AssetCollectorData")]
    public class AssetCollectorData : ScriptableObject
    {
        public BuildTarget BuildTarget = BuildTarget.StandaloneWindows64;
        public List<AssetCollectorPackageData> Packages = new();

        public Dictionary<string, IEnumerable<AssetAlias>> GetAssetPaths()
        {
            return Packages.ToDictionary(
                packages => packages.PackageName,
                packages => packages.GetAssetPaths());
        }
    }
}
