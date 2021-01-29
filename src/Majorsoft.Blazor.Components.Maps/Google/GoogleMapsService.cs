﻿using Microsoft.JSInterop;

using System;
using System.Dynamic;
using System.Threading.Tasks;

namespace Majorsoft.Blazor.Components.Maps.Google
{
	/// <summary>
	/// Default implementation of <see cref="IGoogleMapsService"/>
	/// </summary>
	public sealed class GoogleMapsService : IGoogleMapsService
	{
		private readonly IJSRuntime _jsRuntime;
		private IJSObjectReference _mapsJs;
		private DotNetObjectReference<GoogleMapsEventInfo> _dotNetObjectReference;

		public string MapContainerId { get; private set; }

		public GoogleMapsService(IJSRuntime jsRuntime)
		{
			_jsRuntime = jsRuntime;
		}

		public async Task InitMap(string apiKey,
			string mapContainerId,
			string backgroundColor,
			int controlSize,
			Func<string, Task> mapInitializedCallback = null,
			Func<GeolocationCoordinate, Task> mapClickedCallback = null,
			Func<GeolocationCoordinate, Task> mapDoubleClickedCallback = null,
			Func<GeolocationCoordinate, Task> mapContextMenuCallback = null,
			Func<GeolocationCoordinate, Task> mapMouseUpCallback = null,
			Func<GeolocationCoordinate, Task> mapMouseDownCallback = null,
			Func<GeolocationCoordinate, Task> mapMouseMoveCallback = null,
			Func<Task> mapMouseOverCallback = null,
			Func<Task> mapMouseOutCallback = null,
			Func<GeolocationCoordinate, Task> mapCenterChangedCallback = null,
			Func<byte, Task> mapZoomChangedCallback = null,
			Func<GoogleMapTypes, Task> mapTypeChangedCallback = null,
			Func<int, Task> mapHeadingChangedCallback = null,
			Func<byte, Task> mapTiltChangedCallback = null,
			Func<Task> mapBoundsChangedCallback = null,
			Func<Task> mapProjectionChangedCallback = null,
			Func<Task> mapDraggableChangedCallback = null,
			Func<Task> mapStreetviewChangedCallback = null,
			Func<GeolocationCoordinate, Task> mapDragCallback = null,
			Func<GeolocationCoordinate, Task> mapDragEndCallback = null,
			Func<GeolocationCoordinate, Task> mapDragStartCallback = null,
			Func<Rect, Task> mapResizedCallback = null,
			Func<Task> mapTilesLoadedCallback = null,
			Func<Task> mapIdleCallback = null)
		{
			if(MapContainerId == mapContainerId)
			{
				return; //Prevent double init...
			}

			MapContainerId = mapContainerId;
			await CheckJsObjectAsync();

			var info = new GoogleMapsEventInfo(mapContainerId, 
				mapInitializedCallback: mapInitializedCallback,
				mapClickedCallback: mapClickedCallback,
				mapDoubleClickedCallback: mapDoubleClickedCallback,
				mapContextMenuCallback: mapContextMenuCallback,
				mapMouseUpCallback: mapMouseUpCallback,
				mapMouseDownCallback: mapMouseDownCallback,
				mapMouseMoveCallback: mapMouseMoveCallback,
				mapMouseOverCallback: mapMouseOverCallback,
				mapMouseOutCallback: mapMouseOutCallback,
				mapCenterChangedCallback: mapCenterChangedCallback,
				mapZoomChangedCallback: mapZoomChangedCallback,
				mapTypeChangedCallback: mapTypeChangedCallback,
				mapHeadingChangedCallback: mapHeadingChangedCallback,
				mapTiltChangedCallback: mapTiltChangedCallback,
				mapBoundsChangedCallback: mapBoundsChangedCallback,
				mapProjectionChangedCallback: mapProjectionChangedCallback,
				mapDraggableChangedCallback: mapDraggableChangedCallback,
				mapStreetviewChangedCallback: mapStreetviewChangedCallback,
				mapDragCallback: mapDragCallback,
				mapDragEndCallback: mapDragEndCallback,
				mapDragStartCallback: mapDragStartCallback,
				mapResizedCallback: mapResizedCallback,
				mapTilesLoadedCallback: mapTilesLoadedCallback,
				mapIdleCallback: mapIdleCallback);

			_dotNetObjectReference = DotNetObjectReference.Create<GoogleMapsEventInfo>(info);

			await _mapsJs.InvokeVoidAsync("init", apiKey, mapContainerId, _dotNetObjectReference, backgroundColor, controlSize);
		}

		public async Task SetCenter(double latitude, double longitude)
		{
			await CheckJsObjectAsync();
			await _mapsJs.InvokeVoidAsync("setCenterCoords", MapContainerId, latitude, longitude);
		}

		public async Task SetCenter(string address)
		{
			await CheckJsObjectAsync();
			await _mapsJs.InvokeVoidAsync("setCenterAddress", MapContainerId, address);
		}

		public async Task SetZoom(byte zoom)
		{
			await CheckJsObjectAsync();
			await _mapsJs.InvokeVoidAsync("setZoom", MapContainerId, zoom);
		}

		public async Task PanTo(double latitude, double longitude)
		{
			await CheckJsObjectAsync();
			await _mapsJs.InvokeVoidAsync("panToCoords", MapContainerId, latitude, longitude);
		}

		public async Task PanTo(string address)
		{
			await CheckJsObjectAsync();
			await _mapsJs.InvokeVoidAsync("panToAddress", MapContainerId, address);
		}

		public async Task SetMapType(GoogleMapTypes googleMapType)
		{
			await CheckJsObjectAsync();
			await _mapsJs.InvokeVoidAsync("setMapType", MapContainerId, googleMapType.ToString().ToLower());
		}

		public async Task SetHeading(int heading)
		{
			await CheckJsObjectAsync();
			await _mapsJs.InvokeVoidAsync("setHeading", MapContainerId, heading);
		}

		public async Task SetTilt(byte tilt)
		{
			await CheckJsObjectAsync();
			await _mapsJs.InvokeVoidAsync("setTilt", MapContainerId, tilt);
		}

		public async Task ResizeMap()
		{
			await CheckJsObjectAsync();
			await _mapsJs.InvokeVoidAsync("resizeMap", MapContainerId);
		}

		public async Task SetClickableIcons(bool isClickable)
		{
			await CheckJsObjectAsync();
			await _mapsJs.InvokeVoidAsync("setClickableIcons", MapContainerId, isClickable);
		}

		public async Task SetOptions(ExpandoObject options)
		{
			await CheckJsObjectAsync();
			await _mapsJs.InvokeVoidAsync("setOptions", MapContainerId, options);
		}

		private async Task CheckJsObjectAsync()
		{
			if (_mapsJs is null)
			{
#if DEBUG
				_mapsJs = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Majorsoft.Blazor.Components.Maps/googleMaps.js");
#else
				_mapsJs = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Majorsoft.Blazor.Components.Maps/googleMaps.min.js");
#endif
			}
		}

		public async ValueTask DisposeAsync()
		{
			if (_mapsJs is not null)
			{
				await _mapsJs.InvokeVoidAsync("dispose", MapContainerId);

				await _mapsJs.DisposeAsync();
			}

			_dotNetObjectReference?.Dispose();
		}
	}
}