using System;
using System.Collections.Generic;
using System.IO;
using PortedSolver.Components;
using PortedSolver.Results;

namespace PortedSolver.Utils
{
    public class WriteCSV
    {

        public static void WriteItemPositionForVisualization(List<Item> items)
        {
            StopwatchTimer.Start("write");
            using (var writer = new StreamWriter(Configuration.OUTPUT_FILE))
            {
                string header = "PresentId, Width (x),height (y),Depth (z),tlr,,,trr,,,brr,,,blr,,,brf,,,blf,,,tlf,,,trf,," + "\n";
                writer.Write(header);
                foreach (Item i in items)
                {

                    string collect = i.toCSV();
                    //            System.out.println(collect);

                    writer.Write(collect);
                }
            }

            StopwatchTimer.Stop("write");
        }

        public static void WriteList(List<Item> items)
        {
            StopwatchTimer.Start("write");
            using (var writer = new StreamWriter("./orderedList.csv"))
            {
                String header = "ID, shipmentCode, priority, unloading priority, taxability, maxHeightPosition" + "\n";
                writer.Write(header);

                foreach (Item i in items)
                {

                    String collect = i.toCSVOrderedList();
                    //            System.out.println(collect);

                    writer.Write(collect);
                }
            }
            StopwatchTimer.Stop("write");
        }

        public static void WriteOutpoutDescription(List<Item> items)
        {
            StopwatchTimer.Start("write");
            using (var writer = new StreamWriter("./outputDescr.csv"))
            {
                string header = "ID, priority, unloading priority, taxability, positionX, positionY, positionZ, length, width, height" + "\n";
                writer.Write(header);

                foreach (Item i in items)
                {

                    String collect = i.toCSVOutputDescription();
                    //            System.out.println(collect);

                    writer.Write(collect);
                }
            }
            StopwatchTimer.Stop("write");
        }

        public static void WriteFullItemListWithID(List<Item> items)
        {
            StopwatchTimer.Start("write");
            using (var writer = new StreamWriter("./fullListId.csv"))
            {
                String header = "ID, shipmentCode, priority, unloading priority, taxability, length, width, height" + "\n";
                writer.Write(header);
                foreach (Item i in items)
                {

                    String collect = i.toCSVFullListWithID();
                    //            System.out.println(collect);

                    writer.Write(collect);
                }
            }
            StopwatchTimer.Stop("write");
        }

        public static void WritePareto(List<Solution> solutions)
        {
            StopwatchTimer.Start("write");
            using (var writer = new StreamWriter("./paretoSol.csv"))
            {
                String header = "Taxability, Obstacles" + "\n";
                writer.Write(header);

                foreach (Solution s in solutions)
                {
                    String collect = s.toCSVParetoSol(s);
                    //            System.out.println(collect);

                    writer.Write(collect);
                }
            }
            StopwatchTimer.Stop("write");
        }

        public static void WriteAllSol(List<Solution> solutions)
        {
            StopwatchTimer.Start("write");
            using (var writer = new StreamWriter("./allSol.csv"))
            {
                string header = "Taxability, Obstacles" + "\n";
                writer.Write(header);

                foreach (Solution s in solutions)
                {

                    String collect = s.toCSVParetoSol(s);
                    //            System.out.println(collect);

                    writer.Write(collect);
                }
            }
            StopwatchTimer.Stop("write");
        }


    }
}
