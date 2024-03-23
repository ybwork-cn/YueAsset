using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace ybwork.Assets
{
    public class AssetGroup
    {
        public readonly string GroupName;
        private readonly List<AssetAlias> _assets;
        private readonly Dictionary<string, AssetBundle> _assetBundles = new();
        private readonly Dictionary<string, AssetBundle> _assetPaths = new();
        private bool _inited = false;

        internal AssetGroup(string groupName, List<AssetAlias> assets)
        {
            GroupName = groupName;
            _assets = assets;
        }

        public void InitSync()
        {
            if (_inited)
                return;

            var bundleNames = _assets.GroupBy(asset => asset.BundleName).Select(group => group.Key);
            foreach (var bundleName in bundleNames)
            {
                string path = AssetMgr.Path + GroupName + "/" + bundleName;
                _assetBundles[bundleName] = AssetBundle.LoadFromFile(path + ".ab");
            }

            foreach (var asset in _assets)
            {
                _assetPaths[asset.Name] = _assetBundles[asset.BundleName];
            }

            _inited = true;
        }

        public async Task InitAsync()
        {
            if (_inited)
                return;

            var bundleNames = _assets.GroupBy(asset => asset.BundleName).Select(group => group.Key);
            foreach (var bundleName in bundleNames)
            {
                string path = AssetMgr.Path + GroupName + "/" + bundleName;
                var assetBundleCreateRequest = await AssetBundle.LoadFromFileAsync(path + ".ab");
                _assetBundles[bundleName] = assetBundleCreateRequest.assetBundle;
            }

            foreach (var asset in _assets)
            {
                _assetPaths[asset.Name] = _assetBundles[asset.BundleName];
            }

            _inited = true;
        }

        public T LoadAssetSync<T>(string id) where T : Object
        {
            if (!_inited)
                throw new System.Exception("Not Inited");
            return _assetPaths[id].LoadAsset<T>(id);
        }
    }
}
