using System;
using SFramework.ECS.Runtime;
using TMPro;

namespace _Client_.Scripts.Runtime.Components
{
    [SFGenerateComponent]
    [Serializable]
    public struct TextRef : ISFComponent
    {
        public TMP_Text reference;
    }
}