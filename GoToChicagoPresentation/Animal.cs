// -----------------------------------------------------------------------
//  <copyright file="Animal.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
namespace GoToChicagoPresentation
{
	public class Animal
	{
		public static readonly string[] SpeciesNames = {
			"Dog",
			"Cat"			
		};

		public string Name { get; set; }
		public int Weight { get; set; }
		public string Species { get; set; }
	}
}