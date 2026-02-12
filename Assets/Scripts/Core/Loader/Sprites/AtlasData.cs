using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace Core.Loader.Sprites
{
    /// <summary>
    /// 图集数据
    /// </summary>
    public readonly struct AtlasData
    {
        private readonly Dictionary<string, (Sprite sprite, int refCount)> _sprites;
    
        public SpriteAtlas Atlas { get; }
    
        public AtlasData(SpriteAtlas atlas)
        {
            Atlas = atlas;
            _sprites = new Dictionary<string, (Sprite, int)>();
        }

        /// <summary>
        /// 缓存Sprite
        /// </summary>
        /// <param name="spritename"></param>
        /// <param name="sprite"></param>
        public void Add(string spritename, Sprite sprite)
        {
            _sprites.Add(spritename, (sprite, 1));
        }
    
        public bool TryGetSprite(string SpritName, out Sprite sprite)
        {
            if (_sprites.TryGetValue(SpritName, out var cache))
            {
                sprite = cache.sprite;
                ++cache.refCount;
                return true;
            }

            sprite = null;
            return false;
        }
    
        /// <summary>
        /// 卸载
        /// </summary>
        public void Unload(string SpritName)
        {
            if (!_sprites.TryGetValue(SpritName, out var cache))
            {
                return;
            }
        
            if (cache.refCount > 0)
            {
                --cache.refCount;
            }

            if (cache.refCount == 0)
            {
                _sprites.Remove(SpritName);
            }
        }

        public int GetRefCount()
        {
            var totalRefCount = 0;
            foreach (var spritesValue in _sprites.Values)
            {
                totalRefCount += spritesValue.refCount;
            }
            return totalRefCount;
        }
    }
}
