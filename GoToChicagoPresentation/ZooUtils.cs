// -----------------------------------------------------------------------
//  <copyright file="ZooUtils.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Raven.Abstractions.Data;
using Raven.Abstractions.Replication;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace GoToChicagoPresentation
{
	public class ZooUtils
	{
		public static void SetupZooDatabases()
		{
			CreateReplicatedDatabasesPair("Zoo");
		}

		public static void SetupShardedZooDatabases()
		{
			CreateReplicatedDatabasesPair("DogsZoo");
			CreateReplicatedDatabasesPair("CatsZoo");			
		}

		private static void CreateReplicatedDatabasesPair(string databaseName)
		{
			using (var store1 = new DocumentStore()
			{
				Url = "http://localhost:8080"
			}.Initialize())
			using (var store2 = new DocumentStore()
			{
				Url = "http://localhost:8081"
			}.Initialize())
			{
				DeleteDatabase(databaseName, store1);
				DeleteDatabase(databaseName, store2);

				CreateDatabaseWithReplication(databaseName, store1);
				CreateDatabaseWithReplication(databaseName, store2);
				SetUpReplication(databaseName, store1, store2.Url);
				SetUpReplication(databaseName, store2, store1.Url);

			}
		}

		private static void DeleteDatabase(string databaseName, IDocumentStore store)
		{
			if (store.DatabaseCommands.GlobalAdmin.GetDatabaseNames(128, 0).Contains(databaseName))
				store.DatabaseCommands.GlobalAdmin.DeleteDatabase(databaseName, true);
		}

		private static void CreateDatabaseWithReplication(string databaseName, IDocumentStore store)
		{
			store.DatabaseCommands.GlobalAdmin.CreateDatabase(new DatabaseDocument()
			{
				Id = databaseName,
				Settings = new Dictionary<string, string>()
				{
					{
						"Raven/ActiveBundles", "Replication"
					},
					{
						"Raven/DataDir", "~/" + databaseName
					}
				}
			});
			using (var newStore = new DocumentStore
			{
				Url = store.Url,
				DefaultDatabase = databaseName
			}.Initialize())
			{
				newStore.ExecuteIndex(new RavenDocumentsByEntityName());
			}
		}

		private static void SetUpReplication(string databaseName, IDocumentStore store2, string replicaUrl)
		{
			using (var session = store2.OpenSession(databaseName))
			{
				session.Store(new ReplicationDocument()
				{
					ClientConfiguration = new ReplicationClientConfiguration()
					{
						FailoverBehavior = FailoverBehavior.AllowReadsFromSecondariesAndWritesToSecondaries
					},
					Destinations = new List<ReplicationDestination>()
					{
						new ReplicationDestination()
						{
							Url = replicaUrl,
							Database = databaseName
						}
					}
				});
				session.SaveChanges();
			}
		} 
	}
}