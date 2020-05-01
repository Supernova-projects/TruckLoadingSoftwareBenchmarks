using System;
using System.Collections.Generic;
using PortedSolver.Components;
using PortedSolver.Results;

namespace PortedSolver.Utils
{
    public class Functions
    {
        public static void swapItems(Item[] items, int i, int j)
        {
            Item tmp = items[i];
            items[i] = items[j];
            items[j] = tmp;
        }

        /*Functions for weight deviation calculation*/
        public static double totalWeightOfItems(List<Item> items)
        {
            double result = 0;
            foreach (Item i in items)
            {
                result += i.getWeight();
            }
            return result;
        }

        public static double dev_x(List<Item> items, Container container)
        {
            double num = 0;
            double den = 0;
            foreach (Item i in items)
            {
                num += ((i.getCentre().getX() - container.getCentre().getX()) * i.getWeight());
                den += i.getWeight();
            }
            return num / den;
        }

        public static double dev_y(List<Item> items, Container container)
        {
            double num = 0;
            double den = 0;
            foreach (Item i in items)
            {
                num += (i.getCentre().getX() * i.getWeight());
                den += i.getWeight();
            }
            return num / den;
        }

        public static double dev_z(List<Item> items, Container container)
        {
            double num = 0;
            double den = 0;
            foreach (Item i in items)
            {
                num += ((i.getCentre().getZ() - container.getCentre().getZ()) * i.getWeight());
                den += i.getWeight();
            }
            return num / den;
        }

        public static double objFunction(List<Item> items)
        {
            double res, tmp;
            res = 0;
            foreach (Item item in items)
            {
                tmp = item.getTaxability();
                //            System.err.println("tmp: " + tmp + "--> taxability: " + item.getTaxability() + "; volume: " + item.getVolume());
                res += tmp;
                //            System.err.println("res: " + res);
            }
            return res;
        }

        public static List<Item> getUnpackedItems(List<Item> fullList, List<Item> packedItems)
        {
            List<Item> unpackedItems = new List<Item>();
            foreach (Item item in fullList)
            {
                if (!(packedItems.Contains(item)))
                {
                    unpackedItems.Add(item);
                }
            }
            return unpackedItems;
        }

        public static int countItemsWithPriority(int priority, List<Item> list)
        {
            int res = 0;
            foreach (Item item in list)
            {
                if (item.getPriority() == priority)
                {
                    res += 1;
                }
            }
            return res;
        }
        public static int itemsPriorityOneUnpacked(List<Item> unpackedItem)
        {
            int res = 0;
            foreach (Item item in unpackedItem)
            {
                if ((item.getPriority() == 1))
                {
                    res += 1;
                }
            }
            return res;
        }
        public static int itemsPriorityZeroUnpacked(List<Item> unpackedItem)
        {
            int res = 0;
            foreach (Item item in unpackedItem)
            {
                if ((item.getPriority() == 0))
                {
                    res += 1;
                }
            }
            return res;
        }

        public static bool feasibilityCheck(Container container, double totWeight, double zone1Weight, double zone2Weight, double zone3Weight, double zone4Weight, int p1Left)
        {
            if (totWeight <= container.getMaxWeight() && zone1Weight <= container.maxZone1Weight
                    && zone2Weight <= container.maxZone2Weight && zone3Weight <= container.maxZone3Weight
                    && zone4Weight <= container.maxZone4Weight && (zone1Weight + zone2Weight) <= container.maxMixed12Zones
                    && (zone3Weight + zone4Weight) <= container.maxMixed34Zones /*&& (p1Left == 0)*/)
            {
                return true;
            }
            return false;
        }

        public static bool itemsOverlapping(Item i, List<Item> itemsLoaded)
        {
            double minX = i.bottomLeftFront.getX();
            double minY = i.bottomLeftFront.getY();
            double minZ = i.bottomLeftFront.getZ();
            double maxX = i.topRightRear.getX();
            double maxY = i.topRightRear.getY();
            double maxZ = i.topRightRear.getZ();

            //TRY
            itemsLoaded.Remove(i);

            foreach (Item item in itemsLoaded)
            {
                if ((maxX > item.bottomLeftFront.getX()) && (item.topRightRear.getX() > minX) && (maxY > item.bottomLeftFront.getY()) &&
                        (item.topRightRear.getY() > minY) && (maxZ > item.bottomLeftFront.getZ()) && (item.topRightRear.getZ() > minZ))
                {
                    return true;
                }
            }
            return false;
        }

        public static double getSmallestHeight(List<Item> items)
        {
            items.Sort((x, y) =>
            {
                if (x.getHeight() > y.getHeight())
                {
                    return 1;
                }
                else if (x.getHeight() == y.getHeight())
                {
                    return 0;
                }

                return -1;
            });

            double res = items[0].getHeight();
            return res;
        }

        public static List<Item> sweetRandomization(List<Item> items)
        {
            double rnd;
            for (int i = 0; i < items.Count; i++)
            {
                rnd = StaticRandom.NextDouble();
                if (rnd <= 0.3)
                {
                    items.SwapIndexes(i, StaticRandom.Next(i, items.Count));
                }
            }
            return items;
        }

        public static List<Item> taxabilityRandomization(List<Item> items)
        {
            int dim = items.Count;

            double totTaxability = 0;
            double mediumTaxability;
            foreach (Item item in items)
            {
                totTaxability += item.getTaxability();
            }
            mediumTaxability = totTaxability / dim;

            double rnd;
            int i = -1;

            for (int j = 0; j < items.Count; j++)
            {
                Item item = items[j];
                i += 1;
                rnd = StaticRandom.NextDouble();
                if ((item.getTaxability() < mediumTaxability) && (rnd > 0.2))
                {
                    items.SwapIndexes(i, StaticRandom.Next(i, items.Count));
                }
            }

            return items;
        }

        public static List<Item> probWeightRandomization(List<Item> items)
        {

            int dim = items.Count;
            double rnd = 0.6;
            double similarity = 0.7;
            for (int i = 0; i < dim - 1; i++)
            {
                if ((Math.Min(items[i].getWeight(), items[i + 1].getWeight())) / (Math.Max(items[i].getWeight(), items[i + 1].getWeight())) > similarity)
                {
                    //rnd = Math.random();
                    if (rnd > 0.5)
                    {
                        items.SwapIndexes(i, i + 1);
                    }
                }
            }
            return items;

        }

        public static List<Item> similarityVolumeRandomization(List<Item> items)
        {

            int dim = items.Count;
            double rnd;
            for (int i = 0; i < dim - 1; i++)
            {
                if ((Math.Min(items[i].getVolume(), items[i + 1].getVolume())) / (Math.Max(items[i].getVolume(), items[i + 1].getVolume())) > 0.7)
                {
                    rnd = StaticRandom.NextDouble();
                    if (rnd > 0.5)
                    {
                        items.SwapIndexes(i, i + 1);
                    }
                }
            }
            return items;
        }

        public static double round(double value, int places)
        {
            if (places < 0) throw new ArgumentException();

            long factor = (long)Math.Pow(10, places);
            value = value * factor;
            double tmp = Math.Round(value);
            return tmp / factor;
        }

        public static double upperBoundFullContainer(List<Item> items, Container container)
        {
            items.Sort((x, y) =>
            {
                if (x.getTaxabilityOverVolume() > y.getTaxabilityOverVolume())
                {
                    return -1;
                }
                else if (x.getTaxabilityOverVolume() == y.getTaxabilityOverVolume())
                {
                    return 0;
                }

                return -1;
            });

            double res, res_2;
            double volumeToOccupy = container.getVolume();
            res = 0;
            res_2 = 0;
            foreach (Item i in items)
            {
                if (i.getVolume() <= volumeToOccupy)
                {
                    res += i.getTaxability();
                    volumeToOccupy -= i.getVolume();
                }
                else
                {
                    res += i.getTaxability() * (volumeToOccupy / i.getVolume());
                }
            }

            items.Sort((x, y) =>
            {
                if (x.getTaxabilityOverWeight() > y.getTaxabilityOverVolume())
                {
                    return -1;
                }
                else if (x.getTaxabilityOverWeight() == y.getTaxabilityOverVolume())
                {
                    return 0;
                }

                return -1;
            });

            volumeToOccupy = container.getVolume();
            foreach (Item i in items)
            {
                if (i.getVolume() <= volumeToOccupy)
                {
                    res_2 += i.getTaxability();
                    volumeToOccupy -= i.getVolume();
                }
                else
                {
                    res_2 += i.getTaxability() * (volumeToOccupy / i.getVolume());
                }
            }

            if (res < res_2)
            {
                return res;
            }
            else
            {

                return res_2;
            }

        }

        /*TODO: Sistema paretofront con scrittura su file perchè duplica risultati e non va bene*/

        // this function compares all the solutions and saves only those who are not dominated by taxability result and number of obstacles (lower is better)
        public static List<Solution> paretoFrontTaxabilityObstacles(List<Solution> solutions)
        {
            List<Solution> sol = new List<Solution>();
            solutions.Sort((x, y) =>
            {
                if (x.getTotalTaxability() > y.getTotalTaxability())
                {
                    return -1;
                }
                else if (x.getTotalTaxability() == y.getTotalTaxability())
                {
                    return 0;
                }

                return 1;
            });

            Solution dom = solutions[0];
            sol.Add(dom);
            double bestTax = dom.getTaxability();
            double lessObs = dom.checkUnloadingOrderFeasibilityWithCounter();

            for (int i = 1; i < solutions.Count; i++)
            {
                if (solutions[i].checkUnloadingOrderFeasibilityWithCounter() < lessObs)
                {
                    lessObs = solutions[i].checkUnloadingOrderFeasibilityWithCounter();
                    sol.Add(solutions[i]);
                }

            }

            return sol;
        }

        //public static List<Solution> paretoFrontVolumeObstacles(List<Solution> solutions)
        //{
        //    List<Solution> sol = new List<Solution>();
        //    bool cont;
        //    for (int i = 0; i < solutions.size(); i++)
        //    {
        //        cont = false;
        //        for (int j = i + 1; j < solutions.size(); j++)
        //        {
        //            if ((solutions.get(j).getVolumeOccupied() >= solutions[i].getVolumeOccupied()) &&
        //                    (solutions.get(j).checkUnloadingOrderFeasibilityWithCounter() <= solutions[i].checkUnloadingOrderFeasibilityWithCounter()))
        //            {
        //                cont = true;
        //            }
        //        }
        //        if (cont)
        //        {
        //            sol.add(solutions[i]);
        //        }
        //    }
        //    return sol;
        //}

        //public static List<Solution> paretoFrontWeighObstacles(List<Solution> solutions)
        //{
        //    List<Solution> sol = new List<>();
        //    boolean cont;
        //    for (int i = 0; i < solutions.size(); i++)
        //    {
        //        cont = false;
        //        for (int j = i + 1; j < solutions.size(); j++)
        //        {
        //            if ((solutions.get(j).getWeightLoaded() >= solutions[i].getWeightLoaded()) &&
        //                    (solutions.get(j).checkUnloadingOrderFeasibilityWithCounter() <= solutions[i].checkUnloadingOrderFeasibilityWithCounter()))
        //            {
        //                cont = true;
        //            }
        //        }
        //        if (cont)
        //        {
        //            sol.add(solutions[i]);
        //        }
        //    }
        //    return sol;
        //}


        //public static List<Item> groupOrderItems(List<Item> items)
        //{
        //    items.sort(Comparator.comparing(Item::getUnloadingOrder).reversed().thenComparing(Item::getPriority));
        //    return items;
        //}

    }
}