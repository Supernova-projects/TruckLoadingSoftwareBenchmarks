using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using PortedSolver.Components;
using PortedSolver.DTO;

namespace PortedSolver.Utils
{
    public class ReadCSV
    {
        private static double companyBound = 0;
        private static double companyTotVol = 0;
        private static double companyTotWeight = 0;
        private static int companyItemsPacked = 0;

        public static List<Item> readFromCSV()
        {
            StopwatchTimer.Start("read");
            List<Item> ItemList;
            try
            {
                using (var stream = new StreamReader(Configuration.INPUT_FILE))
                using (var csv = new CsvReader(stream, CultureInfo.InvariantCulture))
                {
                    ItemList = new List<Item>();
                    csv.Configuration.HeaderValidated = (isValid, header, arg3, arg4) => Console.Write($"");
                    csv.Configuration.MissingFieldFound = (strings, i, arg3) => Console.Write("");
                    var records = csv.GetRecords<ParsedItem>();

                    foreach (ParsedItem parsedItem in records)
                    {
                        // Accessing Values by Column Index
                        int id, customer, priority, num_items, stackable_v, unloadingOrder, shipmentCode;
                        double width, height, depth, weight, taxability, vol;
                        bool stackable;
                        num_items = parsedItem.NumberOfItems;

                        for (int i = 0; i < num_items; i++)
                        {
                            depth = parsedItem.Length;
                            width = parsedItem.Width;
                            height = parsedItem.Height;
                            vol = parsedItem.Volume * Math.Pow(10, 6);

                            //                  TODO: Change this --> Done to avoid null items, I give them by default 50x50x50cm dimensions
                            if (depth == 0 && width == 0 && height == 0)
                            {
                                depth += 50;
                                width += 50;
                                height += 50;
                            }

                            weight = parsedItem.Weight / num_items;
                            if (weight == 0)
                            {
                                weight += 100;
                            }

                            stackable_v = parsedItem.Stackable;

                            if (stackable_v == 1)
                            {
                                stackable = true;
                            }
                            else
                            {
                                stackable = false;
                            }

                            unloadingOrder = parsedItem.UnloadingOrder;
                            priority = parsedItem.Priority;
                            taxability = parsedItem.Taxability / num_items;
                            shipmentCode = parsedItem.Shipment;

                            Item item = new Item(ItemList.Count, width, height, depth, weight, stackable, priority,
                                unloadingOrder, taxability, shipmentCode);
                            if (parsedItem.Loaded == 1)
                            {
                                companyItemsPacked++;
                                companyBound += taxability;
                                companyTotVol += vol;
                                companyTotWeight += weight;
                            }

                            //                    Randomization rotation
                            if (StaticRandom.NextDouble() >= 0.5)
                            {
                                item.rotate90();
                                //                        System.err.println("ROTATED");
                                ItemList.Add(item);
                            }
                            else
                            {
                                ItemList.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            StopwatchTimer.Stop("read");
            return ItemList;
        }

        public static List<Item> readFromCSVNoMultipleItems()
        {
            StopwatchTimer.Start("read");
            List<Item> ItemList;
            try
            {

                ItemList = new List<Item>();
                using (var reader = new StreamReader(Configuration.INPUT_FILE))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<ParsedItem>();

                    foreach (ParsedItem parsedItem in records)
                    {
                        // Accessing Values by Column Index
                        int id, customer, priority, num_items, stackable_v, unloadingOrder, shipmentCode;
                        double width, height, depth, weight, taxability;
                        bool stackable;
                        num_items = parsedItem.NumberOfItems;

                        if (num_items > 1)
                        {

                            if (num_items % 2 == 0)
                            {
                                if (num_items % 4 == 0)
                                {
                                    depth = parsedItem.Length * (num_items / 4);
                                    width = parsedItem.Weight * 4;
                                    height = parsedItem.Height;
                                }
                                else
                                {
                                    depth = parsedItem.Length * (num_items / 2);
                                    width = parsedItem.Width * 2;
                                    height = parsedItem.Height;
                                }

                            }
                            else
                            {
                                depth = parsedItem.Length * num_items;
                                width = parsedItem.Width;
                                height = parsedItem.Height;
                            }


                            if (depth == 0 && width == 0 && height == 0)
                            {
                                depth += 50;
                                width += 50;
                                height += 50;
                            }

                            weight = parsedItem.Weight;
                            if (weight == 0)
                            {
                                weight += 100;
                            }

                            stackable_v = parsedItem.Stackable;

                            if (stackable_v == 1)
                            {
                                stackable = true;
                            }
                            else
                            {
                                stackable = false;
                            }

                            unloadingOrder = parsedItem.UnloadingOrder;
                            priority = parsedItem.Priority;
                            taxability = parsedItem.Taxability;
                            shipmentCode = parsedItem.Shipment;
                            //                    To calculate company bound and then compare it with my bound
                            if (parsedItem.Loaded == 1)
                            {
                                companyBound += taxability;
                            }

                            Item item = new Item(ItemList.Count, width, height, depth, weight, stackable, priority,
                                unloadingOrder, taxability, shipmentCode);
                            //                    Randomization rotation
                            if (StaticRandom.NextDouble() >= 0.5)
                            {
                                item.rotate90();
                                ItemList.Add(item);
                            }
                            else
                            {
                                ItemList.Add(item);
                            }
                        }
                        else
                        {
                            depth = parsedItem.Length;
                            width = parsedItem.Width;
                            height = parsedItem.Height;

                            //                  TODO: Change this --> Done to avoid null items, I give them by default 50x50x50 dimensions
                            if (depth == 0 && width == 0 && height == 0)
                            {
                                depth += 50;
                                width += 50;
                                height += 50;
                            }

                            weight = parsedItem.Weight;
                            if (weight == 0)
                            {
                                weight += 100;
                            }

                            stackable_v = parsedItem.Stackable;

                            if (stackable_v == 1)
                            {
                                stackable = true;
                            }
                            else
                            {
                                stackable = false;
                            }

                            unloadingOrder = parsedItem.UnloadingOrder;
                            priority = parsedItem.Priority;
                            taxability = parsedItem.Taxability;
                            shipmentCode = parsedItem.Shipment;
                            //                    To calculate company bound and then compare it with my bound
                            if (parsedItem.Loaded == 1)
                            {
                                companyBound += taxability;
                            }

                            Item item = new Item(ItemList.Count, width, height, depth, weight, stackable, priority,
                                unloadingOrder, taxability, shipmentCode);
                            //                    Randomization rotation
                            if (StaticRandom.NextDouble() >= 0.5)
                            {
                                item.rotate90();
                                ItemList.Add(item);
                            }
                            else
                            {
                                ItemList.Add(item);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            StopwatchTimer.Stop("read");
            return ItemList;
        }


        public static double getCompanyBound()
        {
            /*TODO: prova del +1*/
            return companyBound / (Configuration.TIMES_MULTIRUN + 1);
        }

        public static double getCompanyTotVol()
        {
            return companyTotVol / (Configuration.TIMES_MULTIRUN + 1);
        }

        public static double getCompanyTotWeight()
        {
            return companyTotWeight / (Configuration.TIMES_MULTIRUN + 1);
        }

        public static int getCompanyItemsPacked()
        {
            return companyItemsPacked / (Configuration.TIMES_MULTIRUN + 1);
        }

    }
}
