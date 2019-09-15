﻿using System;

namespace Blazor.Fluxor.AutoDiscovery
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class ReducerAttribute : Attribute
	{
		public readonly ReducerOptions Options;

		public ReducerAttribute(ReducerOptions options = ReducerOptions.None)
		{
			Options = options;
		}
	}
}
