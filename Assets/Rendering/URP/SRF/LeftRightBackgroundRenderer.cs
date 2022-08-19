using System;
using System.Collections;
using System.Collections.Generic;
using Rhinox.Rendering.Universal;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LeftRightBackgroundRenderer : ScriptableRendererFeature
{
    [Serializable]
    public struct LeftRightRendererSettings
    {
        public RenderPassEvent Event;
        public Texture LeftTexture;
        public Texture RightTexture;
    }
    
    // Note: must be lower capital to be registered in the editor
    public LeftRightRendererSettings settings = new LeftRightRendererSettings {
        Event = RenderPassEvent.AfterRenderingSkybox
    };

    private Material _material;
    
    public static LeftRightBackgroundRenderer Instance { get; private set; }
    
    
    public override void Create()
    {
        Instance = this;
        
        var shader = Shader.Find(ShaderConstants.Name);
        _material = new Material(shader);
    }
    
    protected override void Dispose(bool disposing)
    {
        Instance = null;
        base.Dispose(disposing);
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
