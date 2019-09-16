namespace FullStackSample.Client.Store
{
	public class PropertyUpdate<T>
	{
		public readonly bool Updated;
		private readonly T Value;

		public static implicit operator PropertyUpdate<T>(T value) => new PropertyUpdate<T>(value);
		public T GetValueOrDefault() => Updated ? Value : default(T);
		public T UpdatedValue(T originalValue) => Updated ? Value : originalValue;

		public PropertyUpdate(T value)
		{
			Updated = true;
			Value = value;
		}

		public override string ToString() =>
			Updated ? null : Value?.ToString();
	}
}
