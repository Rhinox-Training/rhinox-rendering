using System;
using System.Collections;
using System.Collections.Generic;
using Rhinox.Rendering.Universal;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LeftRightImageRenderer : ScriptableRendererFeature
{
    [Serializable]
    public struct LeftRightRendererSettings
    {
        public RenderPassEvent Event;
        public Texture LeftTexture;
        public Texture RightTexture;
    }
    
    [SerializeField] // Note: must be lower capital to be registered in the editor
    private LeftRightRendererSettings settings = new LeftRightRendererSettings {
        Event = RenderPassEvent.BeforeRenderingOpaques
    };

    private Material _material;
    private int _currentEyeDebug;
    
    public static LeftRightImageRenderer Instance { get; private set; }

    ~LeftRightImageRenderer()
    {
        if (Instance == this)
            Instance = null;
    }
    
    public override void Create()
    {
        Instance = this;
        
        var shader = Shader.Find(ShaderConstants.Name);
        _material = new Material(shader);
    }
    
    public void Enable(Texture left, Texture right)
    {
        settings.LeftTexture = left;
        settings.RightTexture = right;
    }
    
    public void Disable()
    {
        settings.LeftTexture = null;
        settings.RightTexture = null;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        _material.SetInt(ShaderConstants.ShowLeft,settings.LeftTexture == null ? 0 : 1);
        _material.SetInt(ShaderConstants.ShowRight,settings.RightTexture == null ? 0 : 1);

        _material.SetTexture(ShaderConstants.LeftTexture, settings.LeftTexture);
        _material.SetTexture(ShaderConstants.RightTexture, settings.RightTexture);
        
        var pass = new BlitPass(settings.Event, _material, 0, "LeftRightBackgroundRenderer");
        pass.Setup(renderer.cameraColorTarget, renderer.cameraColorTarget);
        renderer.EnqueuePass(pass);
    }

    public static class ShaderConstants
    {
        public const string Name = "Hidden/LeftRightTexture";

        public static int ShowLeft = Shader.PropertyToID("_ShowLeft");
        public static int ShowRight = Shader.PropertyToID("_ShowRight");

        public static int LeftTexture = Shader.PropertyToID("_LeftTex");
        public static int RightTexture = Shader.PropertyToID("_RightTex");
    }
}
