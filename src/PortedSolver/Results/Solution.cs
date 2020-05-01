using System;
using System.Collections.Generic;
using System.Linq;
using PortedSolver.Components;
using PortedSolver.Utils;

namespace PortedSolver.Results
{
    public class Solution
    {

        private List<Item> itemsPacked, itemsNotPacked;
        private double bound;
        private double percentageBound;
        private double volumeOccupied;
        private double weightLoaded;
        private double taxability;
        private double zone1Weight, zone2Weight, zone3Weight, zone4Weight;
        private bool zoneWeightConstraint, unloadingConstraint;
        private bool unloadFromTopFeasibility = true;
        private List<Item> zone1Items = new List<Item>();
        private List<Item> zone2Items = new List<Item>();
        private List<Item> zone3Items = new List<Item>();
        private List<Item> zone4Items = new List<Item>();

        private List<PotentialPoint> potentialPoints;


        public Solution(List<Item> itemsPacked, Container c, List<PotentialPoint> potentialPoints)
        {
            this.itemsPacked = itemsPacked;

            zone1Items = c.getZone1Items();
            zone2Items = c.getZone2Items();
            zone3Items = c.getZone3Items();
            zone4Items = c.getZone4Items();

            zone1Weight = c.getZone1Weight();
            zone2Weight = c.getZone2Weight();
            zone3Weight = c.getZone3Weight();
            zone4Weight = c.getZone4Weight();

            this.potentialPoints = potentialPoints;
        }


        public bool isUnloadingConstraint()
        {
            return unloadingConstraint;
        }

        public void setUnloadingConstraint(bool unloadingConstraint)
        {
            this.unloadingConstraint = unloadingConstraint;
        }

        public bool isUnloadFromTopFeasibility()
        {
            return unloadFromTopFeasibility;
        }

        public void setUnloadFromTopFeasibility(bool unloadFromTopFeasibility)
        {
            this.unloadFromTopFeasibility = unloadFromTopFeasibility;
        }

        public double getTotalTaxability()
        {
            double res = 0;
            foreach (Item i in this.getItemsPacked())
            {
                res += i.getTaxability();
            }
            return res;
        }

        public bool zoneWeightFeasibility(Container c)
        {
            if ((c.zone1Weight > 5000 + (Configuration.WEIGHT_SURPLUS_PARAMETER * 5000)) ||
                    (c.zone2Weight > 6000 + (Configuration.WEIGHT_SURPLUS_PARAMETER * 6000)) ||
                    (c.zone3Weight > 10000 + (Configuration.WEIGHT_SURPLUS_PARAMETER * 10000)) ||
                    (c.zone4Weight > 5000 + (Configuration.WEIGHT_SURPLUS_PARAMETER * 5000))
                /*|| c.zone1Weight+c.zone2Weight > 10000 || c.zone3Weight+c.zone4Weight > 14000*/)
            {
                return false;
            }
            return true;
        }


        public void wipe()
        {
            this.setZone1Weight(0);
            this.setZone2Weight(0);
            this.setZone3Weight(0);
            this.setZone4Weight(0);
            this.setZone1Items(new List<Item>());
            this.setZone2Items(new List<Item>());
            this.setZone3Items(new List<Item>());
            this.setZone4Items(new List<Item>());

        }

        public List<PotentialPoint> getPotentialPoints()
        {
            return potentialPoints;
        }

        public void setPotentialPoints(List<PotentialPoint> potentialPoints)
        {
            this.potentialPoints = potentialPoints;
        }

        public List<Item> getZone1Items()
        {
            return zone1Items;
        }

        public void setZone1Items(List<Item> zone1Items)
        {
            this.zone1Items = zone1Items;
        }

        public List<Item> getZone2Items()
        {
            return zone2Items;
        }

        public void setZone2Items(List<Item> zone2Items)
        {
            this.zone2Items = zone2Items;
        }

        public List<Item> getZone3Items()
        {
            return zone3Items;
        }

        public void setZone3Items(List<Item> zone3Items)
        {
            this.zone3Items = zone3Items;
        }

        public List<Item> getZone4Items()
        {
            return zone4Items;
        }

        public void setZone4Items(List<Item> zone4Items)
        {
            this.zone4Items = zone4Items;
        }

        public double getZone1Weight()
        {
            return zone1Weight;
        }

        public void setZone1Weight(double zone1Weight)
        {
            this.zone1Weight = zone1Weight;
        }

        public double getZone2Weight()
        {
            return zone2Weight;
        }

        public void setZone2Weight(double zone2Weight)
        {
            this.zone2Weight = zone2Weight;
        }

        public double getZone3Weight()
        {
            return zone3Weight;
        }

        public void setZone3Weight(double zone3Weight)
        {
            this.zone3Weight = zone3Weight;
        }

        public double getZone4Weight()
        {
            return zone4Weight;
        }

        public void setZone4Weight(double zone4Weight)
        {
            this.zone4Weight = zone4Weight;
        }

        public double getLoadedWeight()
        {
            double res = 0;
            foreach (Item i in this.getItemsPacked())
            {
                res += i.getWeight();
            }
            return res;
        }
        public List<Item> getItemsPacked()
        {
            return itemsPacked;
        }

        public void setItemsPacked(List<Item> itemsPacked)
        {
            this.itemsPacked = itemsPacked;
        }

        public List<Item> getItemsNotPacked()
        {
            return itemsNotPacked;
        }

        public void setItemsNotPacked(List<Item> itemsNotPacked)
        {
            this.itemsNotPacked = itemsNotPacked;
        }

        public double getBound()
        {
            return bound;
        }

        public void setBound(double bound)
        {
            this.bound = bound;
        }

        public double getPercentageBound(List<Item> allItems)
        {
            double upper_bound = Functions.objFunction(allItems);
            double myResult = Functions.objFunction(itemsPacked);
            this.percentageBound = (myResult / upper_bound) * 100;
            return percentageBound;
        }

        public void setPercentageBound(double percentageBound)
        {
            this.percentageBound = percentageBound;
        }

        public double getVolumeOccupied()
        {
            double res = 0;
            foreach (Item i in this.getItemsPacked())
            {
                res += i.getVolume();
            }

            return res;
        }

        public void setVolumeOccupied(double volumeOccupied)
        {
            this.volumeOccupied = volumeOccupied;
        }

        public double getWeightLoaded()
        {
            return weightLoaded;
        }

        public void setWeightLoaded(double weightLoaded)
        {
            this.weightLoaded = weightLoaded;
        }

        public double getPercentageBound()
        {
            return percentageBound;
        }

        public double getTaxability()
        {
            return taxability;
        }

        public void setTaxability(double taxability)
        {
            this.taxability = taxability;
        }

        public bool isZoneWeightConstraint()
        {
            return zoneWeightConstraint;
        }

        public void setZoneWeightConstraint(bool zoneWeightConstraint)
        {
            this.zoneWeightConstraint = zoneWeightConstraint;
        }


        public bool checkUnloadingOrderFeasibility()
        {
            List<Item> items = this.getItemsPacked();
            items.Sort((x, y) =>
            {
                if (x.getZPosition() > y.getZPosition())
                {
                    return 1;
                }
                else if (x.getZPosition() == y.getZPosition())
                {
                    return 0;
                }

                return -1;
            });

            int dim = items.Count;
            int firstUnloadingOrder = items[0].getUnloadingOrder();
            /*
                    int lastUnloadingOrder = items.get(dim).getUnloadingOrder();
            */
            bool onTop = false;
            bool obstacle = false;

            //        Control only first order priority unloading, not considering case of unloading more than 2 times.
            for (int i = 0; i < dim - 1; i++)
            {
                if (items[i].getUnloadingOrder() == firstUnloadingOrder)
                {
                    for (int j = i + 1; j < dim; j++)
                    {
                        if (items[j].getUnloadingOrder() > firstUnloadingOrder)
                        {
                            if ((items[j].getYPosition() >= items[i].getYPosition() + items[j].getHeight())
                                    && (items[i].getXPosition() <= items[j].getXPosition()) && (items[j].getXPosition() <= items[i].getXPosition() + items[i].getWidth())
                                    && (items[i].getZPosition() <= items[j].getZPosition()) && (items[j].getZPosition() <= items[i].getZPosition() + items[i].getDepth()))
                            {
                                onTop = true;
                            }
                            if ((items[j].getZPosition() >= items[i].getZPosition() + items[i].getDepth())
                                    && (items[i].getXPosition() <= items[j].getXPosition()) && (items[j].getXPosition() <= items[i].getXPosition() + items[i].getWidth())
                                    && (items[i].getYPosition() <= items[j].getYPosition()) && (items[j].getYPosition() <= items[i].getYPosition() + items[i].getHeight()))
                            {
                                obstacle = true;
                            }
                        }

                    }

                    if (onTop || obstacle)
                    {
                        return false;
                    }

                }

            }

            return true;
        }

        /**
         * @info: Functions counts how many items with worst unloading priority obstruct an item with higher unloading priority
         * @return
         */
        public int checkUnloadingOrderFeasibilityWithCounter()
        {
            List<Item> items = this.getItemsPacked();
            items.Sort((x, y) =>
            {
                if (x.getZPosition() > y.getZPosition())
                {
                    return 1;
                }
                else if (x.getZPosition() == y.getZPosition())
                {
                    return 0;
                }

                return -1;
            });
            int dim = items.Count;
            /*
                    int firstUnloadingOrder = items.get(0).getUnloadingOrder();
            */
            /*
                    int lastUnloadingOrder = items.get(dim).getUnloadingOrder();
            */

            int counter = 0;

            //Iterator iterator = items.iterator();
            List<Item> obstacles = new List<Item>();

            foreach (Item i in items)
            {
                for (int j = 0; j < dim; j++)
                {
                    /*Check on top*/
                    if ((!obstacles.Contains(i)) && (i.getUnloadingOrder() > items[j].getUnloadingOrder()) && !(i.Equals(items[j])))
                    {
                        if ((items[j].getYPosition() >= i.getYPosition() + items[j].getHeight())
                            && (i.getXPosition() <= items[j].getXPosition()) && (items[j].getXPosition() <= i.getXPosition() + i.getWidth())
                            && (i.getZPosition() <= items[j].getZPosition()) && (items[j].getZPosition() <= i.getZPosition() + i.getDepth()))
                        {
                            counter++;
                            unloadFromTopFeasibility = false;
                            obstacles.Add(i);
                        }
                        if ((!obstacles.Contains(i)) && ((items[j].getZPosition() >= i.getZPosition() + i.getDepth()))
                                                     && (i.getXPosition() <= items[j].getXPosition()) && (items[j].getXPosition() <= i.getXPosition() + i.getWidth())
                                                     && (i.getYPosition() <= items[j].getYPosition()) && (items[j].getYPosition() <= i.getYPosition() + i.getHeight()))
                        {
                            counter++;
                            obstacles.Add(i);
                        }

                    }

                }
            }

            //while (iterator.hasNext())
            //{
            //    Item i = (Item)iterator.next();
            //    for (int j = 0; j < dim; j++)
            //    {
            //        /*Check on top*/
            //        if ((!obstacles.contains(i)) && (i.getUnloadingOrder() > items[j].getUnloadingOrder()) && !(i.equals(items[j])))
            //        {
            //            if ((items[j].getYPosition() >= i.getYPosition() + items[j].getHeight())
            //                    && (i.getXPosition() <= items[j].getXPosition()) && (items[j].getXPosition() <= i.getXPosition() + i.getWidth())
            //                    && (i.getZPosition() <= items[j].getZPosition()) && (items[j].getZPosition() <= i.getZPosition() + i.getDepth()))
            //            {
            //                counter++;
            //                unloadFromTopFeasibility = false;
            //                obstacles.add(i);
            //            }
            //            if ((!obstacles.contains(i)) && ((items[j].getZPosition() >= i.getZPosition() + i.getDepth()))
            //                    && (i.getXPosition() <= items[j].getXPosition()) && (items[j].getXPosition() <= i.getXPosition() + i.getWidth())
            //                    && (i.getYPosition() <= items[j].getYPosition()) && (items[j].getYPosition() <= i.getYPosition() + i.getHeight()))
            //            {
            //                counter++;
            //                obstacles.add(i);
            //            }

            //        }

            //    }

            //}

            return counter;
        }

        public override string ToString()
        {
            return "Solution{" +
                    "itemsPacked=" + itemsPacked +
                    ", percentageBound=" + percentageBound +
                    ", taxability=" + taxability +
                    ", zoneWeightConstraint=" + zoneWeightConstraint +
                    '}';
        }

        public String toCSVParetoSol(Solution s)
        {

            String str = s.getTotalTaxability() + "," + s.checkUnloadingOrderFeasibilityWithCounter() + "\n";

            return str;
        }

    }
}
