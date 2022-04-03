using UnityEngine;

namespace Thisaislan.PersistenceEasyToDeleteInEditor.Constants
{
    internal static class Consts
    {
        internal const string PedeKeyFormat = "{0}~{1}";
        
        internal const string PedeFilePethFormat = "{0}/{1}";
        
        internal static readonly string PedeFileRootFolderName = $"{Application.persistentDataPath}/Pede";
    }
}