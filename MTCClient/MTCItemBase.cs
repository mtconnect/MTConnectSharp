namespace MTConnectSharp
{
   /// <summary>
   /// The base MTConnect Item Class
   /// </summary>
   public abstract class MTCItemBase
	{
		/// <summary>
		/// Value of the id attribute
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Value of the name attribute
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Name and id of the item as a formatted string
		/// </summary>
		public string LongName
		{
			get
			{
				return $"{Name}({Id})";
			}
		}

		/// <summary>
		/// Formatted string describing the item
		/// </summary>
		public override string ToString()
		{
			try
			{
				return $"{Name}({Id})";
			}
			catch
			{
				return base.ToString();
			}
		}
	}
}
