﻿using Microsoft.Xna.Framework;

namespace Disarray.Engine;

/// <summary>
/// Controls the camera. 
/// </summary>
public sealed class Camera : ISubsystem
{
    private VirtualViewport virtualViewport;
    private float _maximumZoom = float.MaxValue;
    private float _minimumZoom;
    private float _zoom;

    public Camera()
    {
        Rotation = 0;
        Zoom = 1;
        Origin = Vector2.Zero;
        Position = Vector2.Zero;
    }

    public void Initialize(Main main)
    {
        virtualViewport = main.Renderer.VirtualViewport;

        Rotation = 0;
        Zoom = 1;
        Origin = new Vector2(virtualViewport.Width / 2f, virtualViewport.Height / 2f);
        Position = Vector2.Zero;
    }

    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public Vector2 Origin { get; set; }
    public Vector2 Center => Position + Origin;

    public float Zoom
    {
        get => _zoom;
        set
        {
            if ((value < MinimumZoom) || (value > MaximumZoom))
                throw new ArgumentException("Zoom must be between MinimumZoom and MaximumZoom");

            _zoom = value;
        }
    }

    public float MinimumZoom
    {
        get => _minimumZoom;
        set
        {
            if (value < 0)
                throw new ArgumentException("MinimumZoom must be greater than zero");

            if (Zoom < value)
                Zoom = MinimumZoom;

            _minimumZoom = value;
        }
    }

    public float MaximumZoom
    {
        get => _maximumZoom;
        set
        {
            if (value < 0)
                throw new ArgumentException("MaximumZoom must be greater than zero");

            if (Zoom > value)
                Zoom = value;

            _maximumZoom = value;
        }
    }

    // TODO: Implement RectangleF
    //public RectangleF BoundingRectangle
    //{
    //    get
    //    {
    //        var frustum = GetBoundingFrustum();
    //        var corners = frustum.GetCorners();
    //        var topLeft = corners[0];
    //        var bottomRight = corners[2];
    //        var width = bottomRight.X - topLeft.X;
    //        var height = bottomRight.Y - topLeft.Y;
    //        return new RectangleF(topLeft.X, topLeft.Y, width, height);
    //    }
    //}

    public void Move(Vector2 direction)
    {
        Position += Vector2.Transform(direction, Matrix.CreateRotationZ(-Rotation));
    }

    public void Rotate(float deltaRadians)
    {
        Rotation += deltaRadians;
    }

    public void ZoomIn(float deltaZoom)
    {
        ClampZoom(Zoom + deltaZoom);
    }

    public void ZoomOut(float deltaZoom)
    {
        ClampZoom(Zoom - deltaZoom);
    }

    private void ClampZoom(float value)
    {
        if (value < MinimumZoom)
            Zoom = MinimumZoom;
        else
            Zoom = value > MaximumZoom ? MaximumZoom : value;
    }

    public void LookAt(Vector2 position)
    {
        Position = position - new Vector2(virtualViewport.Width / 2f, virtualViewport.Height / 2f);
    }

    public Vector2 WorldToScreen(float x, float y)
    {
        return WorldToScreen(new Vector2(x, y));
    }

    public Vector2 WorldToScreen(Vector2 worldPosition)
    {
        var viewport = virtualViewport.Destination;
        return Vector2.Transform(worldPosition + new Vector2(viewport.X, viewport.Y), GetViewMatrix());
    }

    public Vector2 ScreenToWorld(float x, float y)
    {
        return ScreenToWorld(new Vector2(x, y));
    }

    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        var viewport = virtualViewport.Destination;
        return Vector2.Transform(screenPosition - new Vector2(viewport.X, viewport.Y),
            Matrix.Invert(GetViewMatrix() * virtualViewport.Scale));
    }

    public Matrix GetViewMatrix(Vector2 parallaxFactor)
    {
        return GetVirtualViewMatrix(parallaxFactor);
    }

    private Matrix GetVirtualViewMatrix(Vector2 parallaxFactor)
    {
        return
            Matrix.CreateTranslation(new Vector3(-Position * parallaxFactor, 0.0f)) *
            Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
            Matrix.CreateRotationZ(Rotation) *
            Matrix.CreateScale(Zoom, Zoom, 1) *
            Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
    }

    private Matrix GetVirtualViewMatrix()
    {
        return GetVirtualViewMatrix(Vector2.One);
    }

    public Matrix GetViewMatrix()
    {
        return GetViewMatrix(Vector2.One);
    }

    public Matrix GetInverseViewMatrix()
    {
        return Matrix.Invert(GetViewMatrix());
    }

    private Matrix GetProjectionMatrix(Matrix viewMatrix)
    {
        var projection = Matrix.CreateOrthographicOffCenter(0, virtualViewport.Width, virtualViewport.Height, 0, -1, 0);
        return Matrix.Multiply(viewMatrix, projection);
    }

    public BoundingFrustum GetBoundingFrustum()
    {
        var viewMatrix = GetVirtualViewMatrix();
        var projectionMatrix = GetProjectionMatrix(viewMatrix);
        return new BoundingFrustum(projectionMatrix);
    }

    public ContainmentType Contains(Point point)
    {
        return Contains(new Vector2(point.X, point.Y));
    }

    public ContainmentType Contains(Vector2 vector2)
    {
        return GetBoundingFrustum().Contains(new Vector3(vector2.X, vector2.Y, 0));
    }

    public ContainmentType Contains(Rectangle rectangle)
    {
        var max = new Vector3(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, 0.5f);
        var min = new Vector3(rectangle.X, rectangle.Y, 0.5f);
        var boundingBox = new BoundingBox(min, max);
        return GetBoundingFrustum().Contains(boundingBox);
    }
}
