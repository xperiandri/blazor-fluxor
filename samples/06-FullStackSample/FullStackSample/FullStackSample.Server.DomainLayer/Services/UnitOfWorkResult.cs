namespace FullStackSample.Server.DomainLayer.Services
{
	public class UnitOfWorkResult
	{
		public string Error { get; }
		public bool HasError => !string.IsNullOrEmpty(Error);
		public bool Successful => !HasError;

		public static readonly UnitOfWorkResult Success;

		static UnitOfWorkResult()
		{
			Success = new UnitOfWorkResult();
		}

		private UnitOfWorkResult() { }

		public UnitOfWorkResult(string error) : this()
		{
			Error = error;
		}
	}
}
