using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ybwork.Assets
{
    public static class AssetMgr
    {
        public static readonly string Path = Application.streamingAssetsPath + "/Bundles/";
        private static AssetGroup _defaultAssetGroup = null;
        private static readonly Dictionary<string, AssetGroup> _assetGroups = new();

        // TODO:在此添加路径表，Init先把路径表加载出来

        public static void InitSync()
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(Path + "__dict__/dict.ab");
            string json = bundle.LoadAllAssets<TextAsset>()[0].text;
            var assetsAlias = JsonConvert.DeserializeObject<Dictionary<string, List<AssetAlias>>>(json);
            _assetGroups.Clear();
            foreach (var groupName in assetsAlias.Keys)
            {
                _assetGroups.Add(groupName, new AssetGroup(groupName, assetsAlias[groupName]));
            }
            foreach (AssetGroup group in _assetGroups.Values)
                group.InitSync();
        }

        public static async Task InitAsync()
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(Path + "__dict__/dict.ab");
            string json = bundle.LoadAllAssets<TextAsset>()[0].text;
            var assetsAlias = JsonConvert.DeserializeObject<Dictionary<string, List<AssetAlias>>>(json);
            _assetGroups.Clear();
            foreach (var groupName in assetsAlias.Keys)
            {
                _assetGroups.Add(groupName, new AssetGroup(groupName, assetsAlias[groupName]));
            }
            foreach (AssetGroup group in _assetGroups.Values)
                await group.InitAsync();
        }

        public static void SetDefaultGroup(string defaultGroupName)
        {
            _defaultAssetGroup = GetGroup(defaultGroupName);
        }

        public static AssetGroup GetGroup(string groupName)
        {
            _assetGroups.TryGetValue(groupName, out AssetGroup group);
            return group;
        }

        public static T LoadAssetSync<T>(string id) where T : Object
        {
            CheckDefaultGroup();
            return _defaultAssetGroup.LoadAssetSync<T>(id);
        }

        private static void CheckDefaultGroup()
        {
            if (_defaultAssetGroup == null)
                throw new MissingReferenceException("Please Call AssetMgr.SetDefaultGroup");
        }
    }
}
