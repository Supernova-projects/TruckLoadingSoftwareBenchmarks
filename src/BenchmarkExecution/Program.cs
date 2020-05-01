using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using PortedSolver.Components;
using PortedSolver.Results;
using PortedSolver.Utils;

namespace BenchmarkJarExecution
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<BenchmarkSolver>();
        }
    }
    
    [SimpleJob(RuntimeMoniker.NetCoreApp31, baseline: true)]
    public class BenchmarkSolver
    {
        [Benchmark]
        public void Jar()
        {
            var process = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    FileName = "cmd.exe",
                    Arguments = ($"/c java -jar \"clp_itml.jar\" \".\\input\\15.csv\" \".\\output\" \".\\config.properties\"")
                }
            };

            process.Start();
            process.WaitForExit();

            // Needed so that the java VM can cleanup resources, otherwise rapid sequential executions fail to evaluate
            Thread.Sleep(200);
        }

        [Benchmark]
        public void Ported()
        {
            int reading_method = Configuration.READING_MULTIITEM_METHOD;
            List<Item> items;
            if (reading_method == 0)
            {
                items = ReadCSV.readFromCSV();
            }
            else
            {
                items = ReadCSV.readFromCSVNoMultipleItems();
            }


            List<Item> comparison;
            List<PotentialPoint> potentialpointsSx = new List<PotentialPoint>();
            List<PotentialPoint> potentialpointsDx = new List<PotentialPoint>();

            WriteCSV.WriteFullItemListWithID(items);

            /*TO CREATE NEW INSTANCES*/
            //        InstanceCreator.writeInstance();

            double upper_bound;
            upper_bound = Functions.objFunction(items);
            //Console.WriteLine(upper_bound);

            //WriteCSV.writeList(items);

            // furgonato
            Container c = new Container(Configuration.CONTAINER_WIDTH, Configuration.CONTAINER_HEIGHT, Configuration.CONTAINER_DEPTH, Configuration.CONTAINER_MAX_WEIGHT, Configuration.CONTAINER_UNLOADABLE_FROM_SIDE);


            List<Item> items1;

            Solution[] solution = Solver.multiRunSolution(items, c, potentialpointsSx, potentialpointsDx);
            //Console.WriteLine(solution.Length);


            Solution sol = solution[0];

            items1 = sol.getItemsPacked();

            WriteCSV.WriteItemPositionForVisualization(items1);
            WriteCSV.WriteOutpoutDescription(items1);


            double myResult;
            myResult = Functions.objFunction(items1);

            double x, y, z;
            x = Functions.dev_x(items1, c);
            y = Functions.dev_y(items1, c);
            //        z = Util.Functions.dev_z(items1, c);


            c.loadedItemsInZone(items1);

            double weightMax;
            weightMax = Functions.totalWeightOfItems(items);

            List<Item> unpackedItems = Functions.getUnpackedItems(items, items1);
            int priority1leftUnpacked = Functions.itemsPriorityOneUnpacked(unpackedItems);
            int priority0leftUnpacked = Functions.itemsPriorityZeroUnpacked(unpackedItems);

            bool feasible = sol.zoneWeightFeasibility(c);
            double companyBound = ReadCSV.getCompanyBound();
            double companyTotVol = ReadCSV.getCompanyTotVol();
            double companyTotWeight = ReadCSV.getCompanyTotWeight();
            int companyItemsPacked = ReadCSV.getCompanyItemsPacked();

            /*        System.err.println("Company bound: " + companyBound);
                    System.err.println("My_bound: " + myResult);
                    System.err.println("---------------------------------");*/
            Console.WriteLine("Performance : " + Functions.round((myResult / companyBound * (100)), 2) + " %");

            //Console.WriteLine("Read: " + StopwatchTimer.GetElapsedMillisecondsAndReset("read"));
            //Console.WriteLine("Write: " + StopwatchTimer.GetElapsedMillisecondsAndReset("write"));
            //Console.WriteLine("Sort: " + StopwatchTimer.GetElapsedMillisecondsAndReset("sort"));
            //Console.WriteLine("Add: " + StopwatchTimer.GetElapsedMillisecondsAndReset("add"));
            //Console.WriteLine("Pack: " + StopwatchTimer.GetElapsedMillisecondsAndReset("pack"));
        }
    }
}
