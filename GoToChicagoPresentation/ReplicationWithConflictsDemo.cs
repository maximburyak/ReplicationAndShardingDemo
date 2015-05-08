// -----------------------------------------------------------------------
//  <copyright file="ReplicationWithConflictsDemo.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using Raven.Abstractions.Replication;
using Raven.Client;
using Raven.Client.Document;

namespace GoToChicagoPresentation
{
	public class ReplicationWithConflictsDemo : IDisposable
	{
		private IDocumentStore store1;
		private IDocumentStore store2;

		public ReplicationWithConflictsDemo()
		{
			store1 = new DocumentStore()
			{
				Url = "http://localhost:8080",
				DefaultDatabase = "Zoo",
				Conventions = new DocumentConvention()
				{
					FailoverBehavior = FailoverBehavior.AllowReadsFromSecondariesAndWritesToSecondaries
				}
			}.Initialize();

			store2 = new DocumentStore()
			{
				Url = "http://localhost:8081",
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
			Console.WriteLine("Press any key to generate a new animal, press Esc to quit");
			var counter = 0;
			ConsoleKey curKey = Console.ReadKey().Key;
			while (curKey != ConsoleKey.Escape)
			{
				var species = Animal.SpeciesNames[rand.Next(0, Animal.SpeciesNames.Length)];
				try
				{
					using (var session1 = store1.OpenSession())
					using (var session2 = store2.OpenSession())
					{
						session1.Store(new Animal()
						{
							Name = string.Format("{0} the {1}th - A", species, counter),
							Species = species
						}, "Animals/" + counter);
						session2.Store(new Animal()
						{
							Name = string.Format("{0} the {1}th - B", species, counter),
							Species = species
						}, "Animals/" + counter);
						session1.SaveChanges();
						session2.SaveChanges();

					}
					Console.WriteLine("New pair of conflicting animals created");
					
				}
				catch (Exception e)
				{
					Console.WriteLine("ConnectionError... {0}", e.Message);
				}
				curKey = Console.ReadKey().Key;
				counter++;
			}
		}

		public void Dispose()
		{
			store1.Dispose();
			store2.Dispose();
		}
	}
}