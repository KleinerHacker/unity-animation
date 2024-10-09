using System.Collections.Generic;

namespace UnityAnimation.Runtime.Projects.unity_animation.Scripts.Runtime.Types
{
    public sealed class AnimationData
    {
        private readonly IDictionary<object, object> _data = new Dictionary<object, object>();

        public object Get(object key) => _data.ContainsKey(key) ? _data[key] : null;
        public T Get<T>(object key) => (T) Get(key);

        public void Set(object key, object value)
        {
            if (_data.ContainsKey(key))
                _data[key] = value;
            else
                _data.Add(key, value);
        }
    }
}