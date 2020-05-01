using System;
using System.Collections.Generic;
using System.Diagnostics;
using PortedSolver.Components;
using PortedSolver.Results;
using PortedSolver.Utils;

namespace PortedSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
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

            //WriteCSV.writeList(items);

            // furgonato
            Container c = new Container(Configuration.CONTAINER_WIDTH, Configuration.CONTAINER_HEIGHT, Configuration.CONTAINER_DEPTH, Configuration.CONTAINER_MAX_WEIGHT, Configuration.CONTAINER_UNLOADABLE_FROM_SIDE);


            List<Item> items1;

            Solution[] solution = Solver.multiRunSolution(items, c, potentialpointsSx, potentialpointsDx);


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

            sw.Stop();

            /*        System.err.println("Company bound: " + companyBound);
                    System.err.println("My_bound: " + myResult);
                    System.err.println("---------------------------------");*/
            Console.WriteLine("Performance : " + Functions.round((myResult/companyBound*(100)), 2) + " %");


            //Console.WriteLine("Read: " + StopwatchTimer.GetElapsedMillisecondsAndReset("read"));
            //Console.WriteLine("Write: " + StopwatchTimer.GetElapsedMillisecondsAndReset("write"));
            ////Console.WriteLine("Sort: " + StopwatchTimer.GetElapsedMillisecondsAndReset("sort"));
            //Console.WriteLine("Add: " + StopwatchTimer.GetElapsedMillisecondsAndReset("add"));
            ////Console.WriteLine("Pack: " + StopwatchTimer.GetElapsedMillisecondsAndReset("pack"));
            ////Console.WriteLine("Splice: " + StopwatchTimer.GetElapsedMillisecondsAndReset("splice"));
            ////Console.WriteLine("Overlap: " + StopwatchTimer.GetElapsedMillisecondsAndReset("overlap"));
            ////Console.WriteLine("Corner: " + StopwatchTimer.GetElapsedMillisecondsAndReset("corner"));
            ////Console.WriteLine("Dimension: " + StopwatchTimer.GetElapsedMillisecondsAndReset("dimension"));
            ////Console.WriteLine("Corners Checked: " + StopwatchTimer.CornerChecks);
            //Console.WriteLine("Corners Checked: " + Container.OverlapChecks);
            //Console.WriteLine("Elapsed: " + sw.ElapsedMilliseconds);

        }
    }
}
