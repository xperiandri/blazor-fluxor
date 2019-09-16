using System;

namespace Blazor.Fluxor
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class EffectMethodAttribute : Attribute
	{
	}
}
