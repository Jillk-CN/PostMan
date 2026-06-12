#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

public class BUImporter : AssetPostprocessor
{
    #region Constants
    private const string MATERIALS_FOLDER = "Assets/Materials";
    private const string TEXTURES_FOLDER = "Assets/Textures";
    private const string COLLIDER_KEYWORD = "collider";
    private const string LOD_SEPARATOR = "_LOD";
    #endregion

    #region Private Fields
    private List<Material> extractedMaterials = new List<Material>();
    private string shaderType;
    private Dictionary<string, MaterialData> materialInfo;
    private static readonly HashSet<string> _configuringAssets = new HashSet<string>();
    #endregion

    #region Material Data Class
    [System.Serializable]
    public class MaterialData
    {
        public List<string> textures;
        public string surface_type = "Opaque";
    }
    #endregion

    #region Shader Type Constants
    private static class ShaderTypes
    {
        public const string STANDARD = "STANDARD";
        public const string SPECULAR = "SPECULAR";
        public const string AUTODESK = "AUTODESK";
        public const string URP_AUTODESK = "URP_AUTODESK";
        public const string URP_LIT = "URP_LIT";
        public const string HDRP_AUTODESK = "HDRP_AUTODESK";
        public const string HDRP_LIT = "HDRP_LIT";
    }
    #endregion

    #region Texture Keywords
    private static class TextureKeywords
    {
        public static readonly List<string> Albedo = new List<string> { "albedo", "color", "basecolor", "diffuse", "base" };
        public static readonly List<string> Metallic = new List<string> { "metallic", "metalness", "metal" };
        public static readonly List<string> Glossiness = new List<string> { "glossiness", "smoothness" };
        public static readonly List<string> Specular = new List<string> { "specular", "specularcolor", "specularmap" };
        public static readonly List<string> Roughness = new List<string> { "roughness" };
        public static readonly List<string> Normal = new List<string> { "normal", "normalgl", "tangent" };
        public static readonly List<string> Height = new List<string> { "height", "displacement", "parallax", "bump" };
        public static readonly List<string> Occlusion = new List<string> { "occlusion", "ambientocclusion", "ao", "ambient" };
        public static readonly List<string> Emission = new List<string> { "emission", "emissive", "glow" };
        public static readonly List<string> Mask = new List<string> { "mask" };
        public static readonly List<string> BentNormal = new List<string> { "bent_normal", "bent_normalgl", "bent_tangent" };
        public static readonly List<string> Coat = new List<string> { "coat" };
        public static readonly List<string> Detail = new List<string> { "detail" };
    }
    #endregion

    #region Asset Post Processing
    void OnPostprocessGameObjectWithUserProperties(GameObject obj, string[] propNames, object[] values)
    {
        ProcessUserProperties(propNames, values);
    }

    void OnPostprocessModel(GameObject obj)
    {
        string objName = GetCleanObjectName(obj.name);
        
        ApplyCollider(obj.transform);
        ConfigureModelImporter();
        CreateDirectories();
        ExtractTextures(assetPath, TEXTURES_FOLDER);
        
        // Delay material processing until after import
        EditorApplication.delayCall += () =>
        {
            ExtractMaterials(assetPath, MATERIALS_FOLDER);
            AssetDatabase.Refresh();
            AssignTexturesToMaterials();
        };
    }
    #endregion

    #region User Properties Processing
    private void ProcessUserProperties(string[] propNames, object[] values)
    {
        for (int i = 0; i < propNames.Length; i++)
        {
            switch (propNames[i])
            {
                case "shader_type":
                    shaderType = values[i].ToString();
                    break;
                case "material_info":
                    DeserializeMaterialInfo(values[i].ToString());
                    break;
            }
        }
    }

    private void DeserializeMaterialInfo(string materialInfoJson)
    {
        try
        {
            materialInfo = JsonConvert.DeserializeObject<Dictionary<string, MaterialData>>(materialInfoJson);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to deserialize material info: {ex.Message}");
        }
    }
    #endregion

    #region Utility Methods
    private string GetCleanObjectName(string originalName)
    {
        if (originalName.Contains(LOD_SEPARATOR))
        {
            string[] parts = originalName.Split(new string[] { LOD_SEPARATOR }, StringSplitOptions.None);
            return parts[0];
        }
        return originalName;
    }

    private void ConfigureModelImporter()
    {
        if (_configuringAssets.Contains(assetPath))
            return;

        ModelImporter modelImporter = assetImporter as ModelImporter;
        modelImporter.SearchAndRemapMaterials(ModelImporterMaterialName.BasedOnMaterialName, ModelImporterMaterialSearch.Everywhere);

        string capturedPath = assetPath;
        _configuringAssets.Add(capturedPath);
        EditorApplication.delayCall += () =>
        {
            AssetDatabase.WriteImportSettingsIfDirty(capturedPath);
            AssetDatabase.ImportAsset(capturedPath, ImportAssetOptions.ForceUpdate);
            _configuringAssets.Remove(capturedPath);
        };
    }

    private void CreateDirectories()
    {
        CreateDirectoryIfNotExists(MATERIALS_FOLDER);
        CreateDirectoryIfNotExists(TEXTURES_FOLDER);
    }

    private void CreateDirectoryIfNotExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
    #endregion

    #region Collider Management
    private void ApplyCollider(Transform obj)
    {
        if (obj.name.ToLower().Contains(COLLIDER_KEYWORD))
        {
            obj.gameObject.AddComponent<MeshCollider>();
        }

        foreach (Transform child in obj)
        {
            if (child.name.ToLower().Contains(COLLIDER_KEYWORD))
            {
                var meshRenderer = child.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    meshRenderer.enabled = false;
                }
            }
            ApplyCollider(child);
        }
    }
    #endregion

    #region Asset Extraction
    private void ExtractTextures(string assetPath, string textureDirectory)
    {
        ModelImporter modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
        if (modelImporter != null && !string.IsNullOrEmpty(textureDirectory))
        {
            modelImporter.ExtractTextures(textureDirectory);
        }
    }

    private void ExtractMaterials(string assetPath, string destinationPath)
    {
        HashSet<string> assetsToReimport = new HashSet<string>();
        var materials = AssetDatabase.LoadAllAssetsAtPath(assetPath)
            .Where(x => x.GetType() == typeof(Material));

        foreach (UnityEngine.Object materialAsset in materials)
        {
            if (TryExtractMaterial(materialAsset, destinationPath, assetPath, assetsToReimport))
            {
                // Material was successfully extracted or already exists
            }
        }

        ReimportAssets(assetsToReimport);
    }

    private bool TryExtractMaterial(UnityEngine.Object materialAsset, string destinationPath, string assetPath, HashSet<string> assetsToReimport)
    {
        string materialPath = Path.Combine(destinationPath, materialAsset.name) + ".mat";
        
        if (File.Exists(materialPath))
        {
            // Material already exists, load and add to extracted materials
            Material existingMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (existingMaterial != null)
            {
                extractedMaterials.Add(existingMaterial);
            }
            return true;
        }

        // Extract new material
        string uniquePath = AssetDatabase.GenerateUniqueAssetPath(materialPath);
        string extractResult = AssetDatabase.ExtractAsset(materialAsset, uniquePath);
        
        if (string.IsNullOrEmpty(extractResult))
        {
            assetsToReimport.Add(assetPath);
            Material extractedMaterial = AssetDatabase.LoadAssetAtPath<Material>(uniquePath);
            if (extractedMaterial != null)
            {
                extractedMaterials.Add(extractedMaterial);
            }
            return true;
        }
        
        return false;
    }

    private void ReimportAssets(HashSet<string> assetsToReimport)
    {
        foreach (string assetPath in assetsToReimport)
        {
            AssetDatabase.WriteImportSettingsIfDirty(assetPath);
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        }
    }
    #endregion

    #region Material and Texture Assignment
    private void AssignTexturesToMaterials()
    {
        if (materialInfo == null) return;

        foreach (var materialData in materialInfo)
        {
            string materialName = materialData.Key;
            MaterialData matData = materialData.Value;

            Material material = FindMaterial(materialName);
            if (material != null)
            {
                ConfigureMaterialShader(material, matData);
            }
        }
    }

    private Material FindMaterial(string materialName)
    {
        string[] guids = AssetDatabase.FindAssets($"{materialName} t:Material", new[] { MATERIALS_FOLDER });
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<Material>(path);
        }
        return null;
    }

    private void ConfigureMaterialShader(Material material, MaterialData matData)
    {
        string[] textureNames = matData.textures.ToArray();
        string surfaceType = matData.surface_type;

        switch (shaderType)
        {
            case ShaderTypes.STANDARD:
                ConfigureStandardShader(material, textureNames, surfaceType);
                break;
            case ShaderTypes.SPECULAR:
                ConfigureSpecularShader(material, textureNames, surfaceType);
                break;
            case ShaderTypes.AUTODESK:
                ConfigureAutodeskShader(material, textureNames, surfaceType);
                break;
            case ShaderTypes.URP_AUTODESK:
                ConfigureURPAutodeskShader(material, textureNames, surfaceType);
                break;
            case ShaderTypes.URP_LIT:
                ConfigureURPLitShader(material, textureNames, surfaceType);
                break;
            case ShaderTypes.HDRP_AUTODESK:
                ConfigureHDRPAutodeskShader(material, textureNames, surfaceType);
                break;
            case ShaderTypes.HDRP_LIT:
                ConfigureHDRPLitShader(material, textureNames, surfaceType);
                break;
            default:
                ConfigureStandardShader(material, textureNames, surfaceType);
                break;
        }
    }
    
    private bool IsTransparent(string surfaceType)
    {
        return surfaceType.Equals("Transparent", StringComparison.OrdinalIgnoreCase);
    }

    private Texture2D FindTexture(string textureName)
    {
        string[] guids = AssetDatabase.FindAssets($"{textureName} t:Texture", new[] { TEXTURES_FOLDER });
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
        return null;
    }

    private string GetTextureType(string textureName)
    {
        string[] textureParts = textureName.ToLower().Split('_');
        return textureParts.Length > 0 ? textureParts[textureParts.Length - 1] : string.Empty;
    }

    private bool ContainsKeyword(string textureType, List<string> keywords)
    {
        return keywords.Any(keyword => textureType.Contains(keyword));
    }
    #endregion

    #region Shader Configuration Methods
    private void ConfigureStandardShader(Material material, string[] textureNames, string surfaceType)
    {
        material.shader = Shader.Find("Standard");
        
        // Set rendering mode based on surface type for Built-in RP
        material.SetFloat("_Mode", IsTransparent(surfaceType) ? 3 : 0);
        
        ProcessTextures(material, textureNames, (mat, texture, type) =>
        {
            if (ContainsKeyword(type, TextureKeywords.Albedo))
                mat.SetTexture("_MainTex", texture);
            else if (ContainsKeyword(type, TextureKeywords.Metallic) || ContainsKeyword(type, TextureKeywords.Glossiness))
                mat.SetTexture("_MetallicGlossMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.Normal))
                mat.SetTexture("_BumpMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.Height))
                mat.SetTexture("_ParallaxMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.Occlusion))
                mat.SetTexture("_OcclusionMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.Emission))
            {
                mat.EnableKeyword("_EMISSION");
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
                mat.SetTexture("_EmissionMap", texture);
            }
        });
    }

    private void ConfigureSpecularShader(Material material, string[] textureNames, string surfaceType)
    {
        material.shader = Shader.Find("Standard (Specular setup)");
        
        // Set rendering mode based on surface type for Built-in RP
        material.SetFloat("_Mode", IsTransparent(surfaceType) ? 3 : 0);
        
        ProcessTextures(material, textureNames, (mat, texture, type) =>
        {
            if (ContainsKeyword(type, TextureKeywords.Albedo))
                mat.SetTexture("_MainTex", texture);
            else if (ContainsKeyword(type, TextureKeywords.Specular) || ContainsKeyword(type, TextureKeywords.Glossiness))
                mat.SetTexture("_MetallicGlossMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.Normal))
                mat.SetTexture("_BumpMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.Height))
                mat.SetTexture("_ParallaxMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.Occlusion))
                mat.SetTexture("_OcclusionMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.Emission))
            {
                mat.EnableKeyword("_EMISSION");
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
                mat.SetTexture("_EmissionMap", texture);
            }
        });
    }

    private void ConfigureAutodeskShader(Material material, string[] textureNames, string surfaceType)
    {
        material.shader = Shader.Find("Autodesk Interactive");
        
        // Set rendering mode based on surface type for Built-in RP
        material.SetFloat("_Mode", IsTransparent(surfaceType) ? 3 : 0);
        
        ProcessTextures(material, textureNames, (mat, texture, type) =>
        {
            if (ContainsKeyword(type, TextureKeywords.Albedo))
                mat.SetTexture("_MainTex", texture);
            else if (ContainsKeyword(type, TextureKeywords.Metallic))
                mat.SetTexture("_MetallicGlossMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.Roughness) || ContainsKeyword(type, TextureKeywords.Glossiness))
                mat.SetTexture("_SpecGlossMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.Normal))
                mat.SetTexture("_BumpMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.Height))
                mat.SetTexture("_ParallaxMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.Occlusion))
                mat.SetTexture("_OcclusionMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.Emission))
            {
                mat.EnableKeyword("_EMISSION");
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
                mat.SetTexture("_EmissionMap", texture);
            }
        });
    }

    private void ConfigureURPAutodeskShader(Material material, string[] textureNames, string surfaceType)
    {
        material.shader = Shader.Find("Universal Render Pipeline/Autodesk Interactive/AutodeskInteractive");
        
        // Set surface type for URP (case-insensitive)
        material.SetFloat("_Surface", IsTransparent(surfaceType) ? 1 : 0);
        
        ProcessTextures(material, textureNames, (mat, texture, type) =>
        {
            if (ContainsKeyword(type, TextureKeywords.Albedo))
            {
                mat.SetInt("_UseColorMap", 1);
                mat.SetTexture("_MainTex", texture);
            }
            else if (ContainsKeyword(type, TextureKeywords.Normal))
            {
                mat.SetInt("_UseNormalMap", 1);
                mat.SetTexture("_BumpMap", texture);
            }
            else if (ContainsKeyword(type, TextureKeywords.Metallic))
            {
                mat.SetInt("_UseMetallicMap", 1);
                mat.SetTexture("_MetallicGlossMap", texture);
            }
            else if (ContainsKeyword(type, TextureKeywords.Roughness) || ContainsKeyword(type, TextureKeywords.Glossiness))
            {
                mat.SetInt("_UseRoughnessMap", 1);
                mat.SetTexture("_SpecGlossMap", texture);
            }
            else if (ContainsKeyword(type, TextureKeywords.Emission))
            {
                mat.SetInt("_UseEmissiveMap", 1);
                mat.SetTexture("_EmissionMap", texture);
            }
            else if (ContainsKeyword(type, TextureKeywords.Occlusion))
            {
                mat.SetInt("_UseAoMap", 1);
                mat.SetTexture("_OcclusionMap", texture);
            }
        });
    }

    private void ConfigureURPLitShader(Material material, string[] textureNames, string surfaceType)
    {
        material.shader = Shader.Find("Universal Render Pipeline/Lit");
        
        // Set surface type for URP (case-insensitive)
        material.SetFloat("_Surface", IsTransparent(surfaceType) ? 1 : 0);
        
        ProcessTextures(material, textureNames, (mat, texture, type) =>
        {
            if (ContainsKeyword(type, TextureKeywords.Albedo))
                mat.SetTexture("_BaseMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.Metallic) || ContainsKeyword(type, TextureKeywords.Glossiness))
                mat.SetTexture("_MetallicGlossMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.Normal))
                mat.SetTexture("_BumpMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.Height))
                mat.SetTexture("_ParallaxMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.Occlusion))
                mat.SetTexture("_OcclusionMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.Emission))
            {
                mat.EnableKeyword("_EMISSION");
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
                mat.SetTexture("_EmissionMap", texture);
            }
        });
    }

    private void ConfigureHDRPAutodeskShader(Material material, string[] textureNames, string surfaceType)
    {
        material.shader = Shader.Find("HDRP/Autodesk Interactive/AutodeskInteractive");
        
        // Set surface type for HDRP (case-insensitive)
        material.SetFloat("_SurfaceType", IsTransparent(surfaceType) ? 1 : 0);
        
        ProcessTextures(material, textureNames, (mat, texture, type) =>
        {
            if (ContainsKeyword(type, TextureKeywords.Albedo))
            {
                mat.SetInt("_UseColorMap", 1);
                mat.SetTexture("_MainTex", texture);
            }
            else if (ContainsKeyword(type, TextureKeywords.Normal))
            {
                mat.SetInt("_UseNormalMap", 1);
                mat.SetTexture("_BumpMap", texture);
            }
            else if (ContainsKeyword(type, TextureKeywords.Metallic))
            {
                mat.SetInt("_UseMetallicMap", 1);
                mat.SetTexture("_MetallicGlossMap", texture);
            }
            else if (ContainsKeyword(type, TextureKeywords.Roughness) || ContainsKeyword(type, TextureKeywords.Glossiness))
            {
                mat.SetInt("_UseRoughnessMap", 1);
                mat.SetTexture("_SpecGlossMap", texture);
            }
            else if (ContainsKeyword(type, TextureKeywords.Emission))
            {
                mat.SetInt("_UseEmissiveMap", 1);
                mat.SetTexture("_EmissionMap", texture);
            }
            else if (ContainsKeyword(type, TextureKeywords.Occlusion))
            {
                mat.SetInt("_UseAoMap", 1);
                mat.SetTexture("_OcclusionMap", texture);
            }
        });
    }

    private void ConfigureHDRPLitShader(Material material, string[] textureNames, string surfaceType)
    {
        material.shader = Shader.Find("HDRP/Lit");
        
        // Set surface type for HDRP (case-insensitive)
        material.SetFloat("_SurfaceType", IsTransparent(surfaceType) ? 1 : 0);
        
        ProcessTextures(material, textureNames, (mat, texture, type) =>
        {
            if (ContainsKeyword(type, TextureKeywords.Albedo))
                mat.SetTexture("_BaseColorMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.Mask))
                mat.SetTexture("_MaskMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.Normal))
                mat.SetTexture("_NormalMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.BentNormal))
                mat.SetTexture("_BentNormalMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.Coat))
                mat.SetTexture("_CoatMaskMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.Detail))
                mat.SetTexture("_DetailMap", texture);
            else if (ContainsKeyword(type, TextureKeywords.Emission))
            {
                mat.EnableKeyword("_UseEmissiveIntensity");
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
                mat.SetTexture("_EmissiveColorMap", texture);
            }
        });
    }

    private void ProcessTextures(Material material, string[] textureNames, System.Action<Material, Texture2D, string> assignmentAction)
    {
        foreach (string textureName in textureNames)
        {
            Texture2D texture = FindTexture(textureName);
            if (texture != null)
            {
                string textureType = GetTextureType(textureName);
                if (!string.IsNullOrEmpty(textureType))
                {
                    assignmentAction(material, texture, textureType);
                }
            }
        }
    }
    #endregion
}
#endif