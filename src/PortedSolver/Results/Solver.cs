using System;
using System.Collections.Generic;
using System.Linq;
using PortedSolver.Components;
using PortedSolver.Utils;

namespace PortedSolver.Results
{
    public class Solver
    {

        private static Solution bestTaxability, lessObstacles, maxWeight, maxVolume, res;
        public static int packingMethod = Configuration.PACKING_METHOD;


        /**
         *
         * @param items
         * @param container
         * @param potentialPointsSx
         * @param potentialPointsDx
         * @return
         * @throws IOException
         * @throws ParseException
         */
        public static Solution[] multiRunSolution(List<Item> items, Container container, List<PotentialPoint> potentialPointsSx, List<PotentialPoint> potentialPointsDx)
        {
            List<Solution> solutions = new List<Solution>();
            Solution[] results = new Solution[4];
            int times = Configuration.TIMES_MULTIRUN;
            int reading_method = Configuration.READING_MULTIITEM_METHOD;

            for (int i = 0; i < times; i++)
            {
                if (reading_method == 0)
                {
                    items = ReadCSV.readFromCSV();
                }
                else
                {
                    items = ReadCSV.readFromCSVNoMultipleItems();
                }

                potentialPointsSx = new List<PotentialPoint>();
                potentialPointsDx = new List<PotentialPoint>();
                container = new Container(Configuration.CONTAINER_WIDTH, Configuration.CONTAINER_HEIGHT, Configuration.CONTAINER_DEPTH, Configuration.CONTAINER_MAX_WEIGHT, Configuration.CONTAINER_UNLOADABLE_FROM_SIDE);
                if (packingMethod == 1)
                {
                    solutions.Add(divideEtPack(items, container, potentialPointsSx));
                }
                else if (packingMethod == 2)
                {
                    solutions.Add(divideEtPackAlternate(items, container, potentialPointsSx, potentialPointsDx));
                }
                else if (packingMethod == 3)
                {
                    solutions.Add(newPackAlternate(items, container, potentialPointsSx, potentialPointsDx));
                }

            }

            WriteCSV.WriteAllSol(solutions);

            solutions.Sort((x, y) =>
            {
                if (y.getTotalTaxability() > x.getTotalTaxability())
                {
                    return 1;
                }
                else if (y.getTotalTaxability() == x.getTotalTaxability())
                {
                    return 0;
                }

                return -1;
            });
            List<Solution> paretSolTax = Functions.paretoFrontTaxabilityObstacles(solutions);

            //        WriteCSV.writePareto(paretSolTax);

            bestTaxability = solutions[0];
            results[0] = bestTaxability;
            foreach (Solution s in paretSolTax)
            {
                container.unloadEverything();
                container.loadedItemsInZone(s.getItemsPacked());
                //            System.err.println(s.getItemsPacked().size());
                if ((s.getTotalTaxability() >= bestTaxability.getTotalTaxability()) && s.zoneWeightFeasibility(container))
                {
                    bestTaxability = new Solution(s.getItemsPacked(), container, s.getPotentialPoints());
                    results[0] = bestTaxability;
                }
            }
            res = solutions[0];

            lessObstacles = solutions[0];


            return results;
        }

        /**
         *
         * @param items
         * @param container
         * @param potentialPointsSx
         * @param potentialPointsDx
         * @return
         * @throws IOException
         */
        public static Solution divideEtPackAlternate(List<Item> items, Container container, List<PotentialPoint> potentialPointsSx, List<PotentialPoint> potentialPointsDx)
        {

            List<Item> res = new List<Item>();
            List<Item> retry = new List<Item>();
            PotentialPoint cp = new PotentialPoint(0, 0, 0, container.getWidth(), container.getHeight(), container.getDepth());
            PotentialPoint.addPointToPotentialPoints(cp, potentialPointsSx);

            PotentialPoint cp2 = new PotentialPoint(container.getWidth(), 0, 0, container.getWidth(), container.getHeight(), container.getDepth());
            PotentialPoint.addPointToPotentialPoints(cp2, potentialPointsDx);

            double minHeight = Functions.getSmallestHeight(items);

            /*Add this method somewhere else, or make GroupItems work properly so you don't need this calculation here*/
            List<Item> itemPriorityZero = new List<Item>();
            foreach (Item i in items)
            {
                if (i.getPriority() == 0)
                {
                    itemPriorityZero.Add(i);
                }
            }

            int sizePriorityZero = itemPriorityZero.Count;
            int count = 0;


            itemPriorityZero.Sort((x, y) =>
            {
                if (x.getVolume() > y.getVolume())
                {
                    return 1;
                }
                else if (x.getVolume() > y.getVolume())
                {
                    return 0;
                }

                return -1;
            });

            foreach (Item i in itemPriorityZero)
            {
                double r = StaticRandom.NextDouble();
                if (r < 0.5)
                {
                    if (container.checkAddItemRevisited(i, potentialPointsSx, minHeight))
                    {
                        container.addItem(i);
                        res.Add(i);
                        count += 1;
                    }
                    else
                    {
                        retry.Add(i);
                    }
                }
                else
                {
                    if (container.checkAddItemRevisitedRight(i, potentialPointsDx, minHeight))
                    {
                        container.addItem(i);
                        res.Add(i);
                        count += 1;
                    }
                    else
                    {
                        retry.Add(i);
                    }
                }

            }

            WriteCSV.WriteList(items);

            double rnd = StaticRandom.NextDouble();
            //double rnd = 0.6;
            if (rnd > 0.5)
            {
                items = items.OrderByDescending(i => i.getTaxability())
                    .ThenBy(i => i.getPriority())
                    .ThenBy(i => i.getId())
                    .ToList();
            }
            else
            {
                items = items
                    .OrderByDescending(i => i.getVolume())
                    .ToList();
            }

            double nRnd = StaticRandom.NextDouble();
            //double nRnd = 0.5;
            if (nRnd < 0.3)
            {
                items = Functions.taxabilityRandomization(items);
            }
            else if (nRnd >= 0.3 && nRnd < 0.6)
            {
                //items = Functions.sweetRandomization(items);
                items = Functions.probWeightRandomization(items);
            }
            else if (nRnd >= 6)
            {
                items = Functions.similarityVolumeRandomization(items);
            }

            foreach (Item i in items)
            {

                double r = StaticRandom.NextDouble();
                //double r = 0.4;
                if (r < 0.5)
                {
                    if (!(res.Contains(i)) && !(i.checkSlimItem()) && (container.checkAddItemRevisited(i, potentialPointsSx, minHeight)))
                    {
                        container.addItem(i);
                        res.Add(i);
                    }
                    else
                    {
                        retry.Add(i);
                    }
                }
                else
                {
                    if (!(res.Contains(i)) && !(i.checkSlimItem()) && (container.checkAddItemRevisitedRight(i, potentialPointsDx, minHeight)))
                    {
                        container.addItem(i);
                        res.Add(i);
                    }
                    else
                    {
                        retry.Add(i);
                    }
                }


            }

            foreach (Item i in retry)
            {

                if (!(res.Contains(i)) && (container.checkAddItemRevisited(i, potentialPointsSx, minHeight)))
                {
                    container.addItem(i);
                    res.Add(i);
                }
            }

            List<PotentialPoint> potentialPoints = potentialPointsSx;
            foreach (PotentialPoint s in potentialPointsDx)
            {
                potentialPoints.Add(s);
            }
            Solution sol = new Solution(res, container, potentialPoints);


            /*Print for test best solution*/
            double myResult, upperBound;
            myResult = Functions.objFunction(res);
            upperBound = Functions.objFunction(items);
            /*
                    System.err.println("ALT: Result: " + (int)((myResult/upperBound)*100) + "%, " + "" + sol.zoneWeightFeasibility(container) +
                            ", obstacles: " +  sol.checkUnloadingOrderFeasibilityWithCounter() +
                            ", Weight: " + sol.getLoadedWeight() + ", Vol: " + sol.getVolumeOccupied() +
                            ", x deviation: " + Utils.Functions.dev_x(res, container));
            */

            if (count < sizePriorityZero)
            {
                //System.err.println("Not all p0 packed!");
                List<Item> i = new List<Item>();
                Solution s = new Solution(i, container, potentialPoints);
                return s;
            }
            else
            {
                return sol;
            }

        }

        /**
         *
         * @param items
         * @param container
         * @param potentialPoints
         * @return
         * @throws IOException
         */
        public static Solution divideEtPack(List<Item> items, Container container, List<PotentialPoint> potentialPoints)
        {

            List<Item> res = new List<Item>();
            List<Item> retry = new List<Item>();
            PotentialPoint cp = new PotentialPoint(0, 0, 0, container.getWidth(), container.getHeight(), container.getDepth());
            PotentialPoint.addPointToPotentialPoints(cp, potentialPoints);

            PotentialPoint cp2 = new PotentialPoint(container.getWidth(), 0, 0, container.getWidth(), container.getHeight(), container.getDepth());
            PotentialPoint.addPointToPotentialPoints(cp2, potentialPoints);

            double minHeight = Functions.getSmallestHeight(items);

            /*Add this method somewhere else, or make GroupItems work properly so you don't need this calculation here*/
            List<Item> itemPriorityZero = new List<Item>();
            foreach (Item i in items)
            {
                if (i.getPriority() == 0)
                {
                    itemPriorityZero.Add(i);
                }
            }

            int sizePriorityZero = itemPriorityZero.Count;
            int count = 0;


            itemPriorityZero.Sort((x, y) =>
            {
                if (x.getVolume() > y.getVolume())
                {
                    return 1;
                }
                else if (x.getVolume() == y.getVolume())
                {
                    return 0;
                }

                return -1;
            });

            foreach (Item i in itemPriorityZero)
            {
                if (container.checkAddItemRevisited(i, potentialPoints, minHeight))
                {
                    container.addItem(i);
                    res.Add(i);
                    count += 1;
                }
                else
                {
                    retry.Add(i);
                }
            }

            WriteCSV.WriteList(items);

            double rnd = StaticRandom.NextDouble();
            if (rnd > 0.5)
            {
                items.Sort((x, y) =>
                {
                    if (x.getTaxability() > y.getTaxability())
                    {
                        return -1;
                    }
                    else if (x.getTaxability() == y.getTaxability())
                    {
                        return 0;
                    }

                    return 1;
                });
            }
            else
            {
                items.Sort((x, y) =>
                {
                    if (x.getVolume() > y.getVolume())
                    {
                        return -1;
                    }
                    else if (x.getVolume() == y.getVolume())
                    {
                        return 0;
                    }

                    return 1;
                });
            }

            double nRnd = StaticRandom.NextDouble();
            if (nRnd < 0.3)
            {
                items = Functions.taxabilityRandomization(items);
            }
            else if (nRnd >= 0.3 && nRnd < 0.6)
            {
                //items = Functions.sweetRandomization(items);
                items = Functions.probWeightRandomization(items);
            }
            else if (nRnd >= 6)
            {
                items = Functions.similarityVolumeRandomization(items);
            }
            items = Functions.sweetRandomization(items);
            //        items = Functions.taxabilityRandomization(items);
            items = Functions.probWeightRandomization(items);
            //        items = Functions.similarityVolumeRandomization(items);

            foreach (Item i in items)
            {

                if (!(res.Contains(i)) && !(i.checkSlimItem()) && (container.checkAddItemRevisited(i, potentialPoints, minHeight)))
                {
                    container.addItem(i);
                    res.Add(i);
                }
                else
                {
                    retry.Add(i);
                }

            }



            foreach (Item i in retry)
            {
                double mRnd = StaticRandom.NextDouble();
                if (mRnd < 0.3)
                {
                    potentialPoints.Sort((x, y) =>
                    {
                        if (x.getX() > y.getX())
                        {
                            return -1;
                        }
                        else if (x.getX() == y.getX())
                        {
                            return 0;
                        }

                        return 1;
                    });
                }
                else if (mRnd >= 0.3 && nRnd < 0.6)
                {
                    potentialPoints.Sort((x, y) =>
                    {
                        if (x.getZ() > y.getZ())
                        {
                            return 1;
                        }
                        else if (x.getZ() == y.getZ())
                        {
                            return 0;
                        }

                        return -1;
                    });
                }
                else if (mRnd >= 6)
                {
                    //                potentialPoints.sort(Comparator.comparing(PotentialPoint::getY));
                    potentialPoints.Sort((x, y) =>
                    {
                        if (x.getUsableSpace() > y.getUsableSpace())
                        {
                            return 1;
                        }
                        else if (x.getUsableSpace() == y.getUsableSpace())
                        {
                            return 0;
                        }

                        return -1;
                    });
                }

                if (!(res.Contains(i)) && (container.checkAddItemRevisited(i, potentialPoints, minHeight)))
                {
                    container.addItem(i);
                    res.Add(i);
                }
            }


            // IMPORTANT for feasbility check
            //        container.loadedItemsInZone(res);
            Solution sol = new Solution(res, container, potentialPoints);


            /*Print for test best solution*/
            double myResult, upperBound;
            myResult = Functions.objFunction(res);
            upperBound = Functions.objFunction(items);
            Console.WriteLine("N: Result: " + (int)((myResult / upperBound) * 100) + "%, " + "" + sol.zoneWeightFeasibility(container) +
                    ", items obstacling: " + sol.checkUnloadingOrderFeasibilityWithCounter() + ", Weight: " +
                    sol.getLoadedWeight() + ", Vol: " + sol.getVolumeOccupied());

            return sol;
        }


        public static Solution newPackAlternate(List<Item> items, Container container, List<PotentialPoint> potentialPointsSx, List<PotentialPoint> potentialPointsDx)
        {

            List<Item> res = new List<Item>();
            List<Item> retry = new List<Item>();
            PotentialPoint cp = new PotentialPoint(0, 0, 0, container.getWidth(), container.getHeight(), container.getDepth());
            PotentialPoint.addPointToPotentialPoints(cp, potentialPointsSx);

            PotentialPoint cp2 = new PotentialPoint(container.getWidth(), 0, 0, container.getWidth(), container.getHeight(), container.getDepth());
            PotentialPoint.addPointToPotentialPoints(cp2, potentialPointsDx);

            double minHeight = Functions.getSmallestHeight(items);

            /*Add this method somewhere else, or make GroupItems work properly so you don't need this calculation here*/
            List<Item> zeroPriorityList = new List<Item>();
            List<Item> onePriorityList = new List<Item>();
            List<Item> otherPriorityList = new List<Item>();
            int countZeroPriority = 0, countOnePriority = 0, countOtherPriority = 0, loadedCount = 0;
            foreach (Item i in items)
            {
                if (i.getPriority() == 0)
                {
                    zeroPriorityList.Add(i);
                    countZeroPriority++;
                }
                else if (i.getPriority() == 1)
                {
                    onePriorityList.Add(i);
                    countOnePriority++;
                }
                else
                {
                    otherPriorityList.Add(i);
                    countOtherPriority++;
                }
            }

            zeroPriorityList = zeroPriorityList
                .OrderBy(i => i.getCustomer())
                .ThenByDescending(i => i.getTaxability())
                .ToList();

            onePriorityList = onePriorityList
                .OrderBy(i => i.getCustomer())
                .ThenByDescending(i => i.getTaxability())
                .ToList();

            foreach (Item i in zeroPriorityList)
            {

                double r = StaticRandom.NextDouble();
                if (r < 0.5)
                {
                    if (container.checkAddItemRevisited(i, potentialPointsSx, minHeight))
                    {
                        container.addItem(i);
                        res.Add(i);
                        loadedCount += 1;
                    }
                    else
                    {
                        retry.Add(i);
                    }
                }
                else
                {
                    if (container.checkAddItemRevisitedRight(i, potentialPointsDx, minHeight))
                    {
                        container.addItem(i);
                        res.Add(i);
                        loadedCount += 1;
                    }
                    else
                    {
                        retry.Add(i);
                    }
                }

            }

            WriteCSV.WriteList(items);

            foreach (Item i in onePriorityList)
            {

                double r = StaticRandom.NextDouble();
                if (r < 0.5)
                {
                    if (!(res.Contains(i)) && !(i.checkSlimItem()) && (container.checkAddItemRevisited(i, potentialPointsSx, minHeight)))
                    {
                        container.addItem(i);
                        res.Add(i);
                    }
                    else
                    {
                        retry.Add(i);
                    }
                }
                else
                {
                    if (!(res.Contains(i)) && !(i.checkSlimItem()) && (container.checkAddItemRevisitedRight(i, potentialPointsDx, minHeight)))
                    {
                        container.addItem(i);
                        res.Add(i);
                    }
                    else
                    {
                        retry.Add(i);
                    }
                }
            }

            foreach (Item i in retry)
            {

                if (!(res.Contains(i)) && (container.checkAddItemRevisited(i, potentialPointsSx, minHeight)))
                {
                    container.addItem(i);
                    res.Add(i);
                }
            }



            foreach (Item i in otherPriorityList)
            {

                double r = StaticRandom.NextDouble();
                if (r < 0.5)
                {
                    if (!(res.Contains(i)) && !(i.checkSlimItem()) && (container.checkAddItemRevisited(i, potentialPointsSx, minHeight)))
                    {
                        container.addItem(i);
                        res.Add(i);
                    }
                    else
                    {
                        retry.Add(i);
                    }
                }
                else
                {
                    if (!(res.Contains(i)) && !(i.checkSlimItem()) && (container.checkAddItemRevisitedRight(i, potentialPointsDx, minHeight)))
                    {
                        container.addItem(i);
                        res.Add(i);
                    }
                    else
                    {
                        retry.Add(i);
                    }
                }
            }

            // IMPORTANT for feasbility check
            //        container.loadedItemsInZone(res);
            List<PotentialPoint> potentialPoints = potentialPointsSx;
            foreach (PotentialPoint s in potentialPointsDx)
            {
                potentialPoints.Add(s);
            }
            Solution sol = new Solution(res, container, potentialPoints);


            /*Print for test best solution*/
            double myResult, upperBound;
            myResult = Functions.objFunction(res);
            upperBound = Functions.objFunction(items);
            /*        System.err.println("ALT: Result: " + (int)((myResult/upperBound)*100) + "%, " + "" + sol.zoneWeightFeasibility(container) +
                            ", obstacles: " +  sol.checkUnloadingOrderFeasibilityWithCounter() +
                            ", taxability: " + sol.getTotalTaxability() +
                            ", Weight: " + sol.getLoadedWeight() + ", Vol: " + sol.getVolumeOccupied() +
                            ", x deviation: " + Utils.Functions.dev_x(res, container));*/

            return sol;
        }
    }
}
