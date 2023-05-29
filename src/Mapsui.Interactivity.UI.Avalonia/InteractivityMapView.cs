﻿using Avalonia;
using Avalonia.Input;
using Mapsui.Extensions;
using Mapsui.Interactivity.UI.Input.Core;
using Mapsui.UI.Avalonia;
using Mapsui.UI.Avalonia.Extensions;

namespace Mapsui.Interactivity.UI.Avalonia;

public class InteractivityMapView : MapControl, IView
{
    private IController? _controller;

    public static readonly StyledProperty<Map?> MapSourceProperty =
        AvaloniaProperty.Register<InteractivityMapView, Map?>(nameof(MapSource));

    public static readonly StyledProperty<IInteractive> InteractiveProperty =
        AvaloniaProperty.Register<InteractivityMapView, IInteractive>(nameof(Interactive));

    public static readonly StyledProperty<string> StateProperty =
        AvaloniaProperty.Register<InteractivityMapView, string>(nameof(State), States.Default);

    public Map? MapSource
    {
        get { return GetValue(MapSourceProperty); }
        set { SetValue(MapSourceProperty, value); }
    }

    public IInteractive Interactive
    {
        get { return GetValue(InteractiveProperty); }
        set { SetValue(InteractiveProperty, value); }
    }

    public string State
    {
        get { return GetValue(StateProperty); }
        set { SetValue(StateProperty, value); }
    }

    protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == MapSourceProperty)
        {
            if (change.NewValue.GetValueOrDefault() is Map map)
            {
                Map = map;
            }
        }
        else if (change.Property == StateProperty)
        {
            if (change.NewValue.GetValueOrDefault() is string state && string.IsNullOrEmpty(state) == false)
            {
                _controller = InteractiveControllerFactory.GetController(state);

                // HACK: after tools check, hover manipulator not active, it call this          
                _controller.HandleMouseEnter(this, new MouseEventArgs());
            }
        }
    }

    public void SetCursor(CursorType cursorType)
    {
        Cursor = new Cursor(cursorType.ToStandartCursor());
    }

    public MPoint WorldToScreen(MPoint worldPosition)
    {
        return Map.Navigator.Viewport.WorldToScreen(worldPosition);
    }

    protected override void OnPointerEnter(PointerEventArgs e)
    {
        base.OnPointerEnter(e);

        if (e.Handled)
        {
            return;
        }

        var position = e.GetPosition(this).ToMapsui();

        var mapInfo = GetMapInfo(position);

        _controller?.HandleMouseEnter(this, new MouseEventArgs { MapInfo = mapInfo });
    }

    protected override void OnPointerLeave(PointerEventArgs e)
    {
        base.OnPointerLeave(e);

        if (e.Handled)
        {
            return;
        }

        var position = e.GetPosition(this).ToMapsui();

        var mapInfo = GetMapInfo(position);

        _controller?.HandleMouseLeave(this, new MouseEventArgs { MapInfo = mapInfo });
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);

        if (e.Handled)
        {
            return;
        }

        var args = new MouseWheelEventArgs
        {
            Delta = (int)(e.Delta.Y + e.Delta.X) * 120
        };

        _controller?.HandleMouseWheel(this, args);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (e.Handled)
        {
            return;
        }

        this.Focus();

        e.Pointer.Capture(this);

        var position = e.GetPosition(this).ToMapsui();

        var mapInfo = GetMapInfo(position);

        var args = new MouseDownEventArgs
        {
#pragma warning disable CS0618 // Тип или член устарел
            ChangedButton = e.GetPointerPoint(null).Properties.PointerUpdateKind.Convert(),
#pragma warning restore CS0618 // Тип или член устарел
            ClickCount = e.ClickCount,
            MapInfo = mapInfo
        };

        _controller?.HandleMouseDown(this, args);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (e.Handled == true)
        {
            return;
        }

        var position = e.GetPosition(this).ToMapsui();

        var mapInfo = GetMapInfo(position);

        var args = new MouseEventArgs
        {
            MapInfo = mapInfo,
        };

        _controller?.HandleMouseMove(this, args);

        // TODO: ?
        e.Handled = args.Handled;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (e.Handled)
        {
            return;
        }

        e.Pointer.Capture(null);

        var position = e.GetPosition(this).ToMapsui();

        var mapInfo = GetMapInfo(position);

        _controller?.HandleMouseUp(this, new MouseEventArgs { MapInfo = mapInfo });
    }
}
