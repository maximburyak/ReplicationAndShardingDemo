// -----------------------------------------------------------------------
//  <copyright file="Replication.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Raven.Client.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Xml.Serialization;
using Raven.Abstractions.Data;
using Raven.Abstractions.Replication;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Raven.Client.Shard;
using Raven.Json.Linq;

namespace GoToChicagoPresentation
{
	public class DemonstrationEngine
	{
		public  void Run()
		{
			int input = 0;
			while (input != 5)
			{
				try
				{
					Console.WriteLine("Choose Your Destiny:");
					Console.WriteLine("1)Replicaiton Demonstration");
					Console.WriteLine("2)Replicaiton Demonstration With Conflicts");
					Console.WriteLine("3)Sharding Demonstration");
					Console.WriteLine("4)Quit");
					input = Int32.Parse(Console.ReadLine());
					switch (input)
					{
						case 1:
							ZooUtils.SetupZooDatabases();
							using (var domo = new SimpleReplicationDemo())
							{
								domo.RunDemo();
							}
							break;
						case 2:
							ZooUtils.SetupZooDatabases();
							using (var demo = new ReplicationWithConflictsDemo())
							{
								demo.RunDemo();
							}
							break;
						case 3:
							ZooUtils.SetupShardedZooDatabases();
							using (var demo = new ShardingWithReplicationDemo())
							{
								demo.RunDemo();
							}
							break;
						case 5:
							return;
					}
				}
				catch (Exception exception)
				{
					Console.WriteLine(string.Format("Failed to process input: {0}", exception.Message));
				}
			}
		}
	}
}