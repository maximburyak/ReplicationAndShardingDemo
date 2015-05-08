// -----------------------------------------------------------------------
//  <copyright file="ShardingWithReplicationDemo.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Raven.Abstractions.Replication;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Shard;

namespace GoToChicagoPresentation
{
	public class ShardingWithReplicationDemo : IDisposable
	{
		private Dictionary<string, IDocumentStore> shards;
		private ShardedDocumentStore shardedStore;

		public ShardingWithReplicationDemo()
		{
			shards = new Dictionary<string, IDocumentStore>()
			{

				{
					"Dog", new DocumentStore()
					{
						Url = "http://localhost:8080",
						DefaultDatabase = "DogsZoo"
					}
				},
				{
					"Cat", new DocumentStore()
					{
						Url = "http://localhost:8080",
						DefaultDatabase = "CatsZoo"
					}
				},
				
			};

			shardedStore = new ShardedDocumentStore(
				new ShardStrategy(shards).ShardingOn<Animal>(x => x.Species));

			shardedStore.Conventions = new DocumentConvention()
			{
				FailoverBehavior = FailoverBehavior.AllowReadsFromSecondariesAndWritesToSecondaries
			};

			shardedStore.Initialize();
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

					using (var session = shardedStore.OpenSession())
					{
						session.Store(new Animal()
						{
							Name = string.Format("{0} #{1}", species, counter),
							Species = species
						});
						session.SaveChanges();
					}

					Console.WriteLine("Created new animal in {0}ms", localSP.ElapsedMilliseconds);
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
			shardedStore.Dispose();
		}
	}
}