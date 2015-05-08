// -----------------------------------------------------------------------
//  <copyright file="SimpleReplicationDemo.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Raven.Abstractions.Replication;
using Raven.Client;
using Raven.Client.Document;

namespace GoToChicagoPresentation
{
	public class SimpleReplicationDemo : IDisposable
	{
		private IDocumentStore store;

		public SimpleReplicationDemo()
		{
			store = new DocumentStore()
			{
				Url = "http://localhost:8080",
				DefaultDatabase = "Zoo",
				Conventions = new DocumentConvention()
				{
					FailoverBehavior = FailoverBehavior.AllowReadsFromSecondariesAndWritesToSecondaries
				}
			}.Initialize();
		}

		public void RunDemo()
		{
			var rand = new Random();
			var counter = 0;
			Console.WriteLine("Press any key to generate a new animal, press Esc to quit");

			ConsoleKey curKey = Console.ReadKey().Key;
			while (curKey != ConsoleKey.Escape)
			{
				try
				{
					var localSP = Stopwatch.StartNew();
					var species = Animal.SpeciesNames[rand.Next(0, Animal.SpeciesNames.Length)];

					using (var session = store.OpenSession())
					{
						session.Store(new Animal()
						{
							Name = string.Format("{0} the {1}th", species, counter),
							Species = species
						});
						session.SaveChanges();
					}


					Console.WriteLine("New animal created in {0}ms", localSP.ElapsedMilliseconds);
					curKey = Console.ReadKey().Key;
					counter++;
				}
				catch (Exception e)
				{
					Console.WriteLine("ConnectionError... {0}", e.Message);
				}
			}
		}

		public void Dispose()
		{
			store.Dispose();
		}
	}
}