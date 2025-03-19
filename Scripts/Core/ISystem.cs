using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    /// <summary>
    /// System interface
    /// </summary>
    public interface ISystem
    {
        void Initialize();
        void Process(List<Entity> entities);
    }
}
