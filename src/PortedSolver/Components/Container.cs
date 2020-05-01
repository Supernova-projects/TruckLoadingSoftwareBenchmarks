using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PortedSolver.Utils;

namespace PortedSolver.Components
{
    public class Container
    {
        public static int OverlapChecks = 0;

        /*FIXED container values for check feasibility*/
        public int maxZone1Weight = Configuration.MAX_ZONE1_WEIGHT;
        public int maxZone2Weight = Configuration.MAX_ZONE2_WEIGHT;
        public int maxZone3Weight = Configuration.MAX_ZONE3_WEIGHT;
        public int maxZone4Weight = Configuration.MAX_ZONE4_WEIGHT;
        public int maxMixed12Zones = Configuration.MAX_MIXED_ZONES12_WEIGHT;
        public int maxMixed34Zones = Configuration.MAX_MIXED_ZONES34_WEIGHT;
        public double weightSurplusPerameter = Configuration.WEIGHT_SURPLUS_PARAMETER;

        private double width, height, depth, maxWeight;
        private bool unloadableFromSide;
        private double loadedWeight = 0;
        private List<Item> itemsLoaded = new List<Item>();
        private int numItemsLoaded = 0;
        private double remainingVolume, remainingWeight;
        public List<Item> zone1Items = new List<Item>();
        public List<Item> zone2Items = new List<Item>();
        public List<Item> zone3Items = new List<Item>();
        public List<Item> zone4Items = new List<Item>();
        public double zone1Weight = 0;
        public double zone2Weight = 0;
        public double zone3Weight = 0;
        public double zone4Weight = 0;

        /**
         *
         * @param width
         * @param height
         * @param depth
         * @param maxWeight
         * @param unloadableFromSide
         *
         */
        public Container(double width, double height, double depth, double maxWeight, bool unloadableFromSide)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;
            this.maxWeight = maxWeight;
            this.unloadableFromSide = unloadableFromSide;
        }

        /**
         * CONSTRAINT: check if item fits inside container
         * @param i
         * @return true if dimension can fit inside the container
         */
        public bool dimensionsFit(Item i)
        {
            if (i.getWidth() > this.getWidth() || i.getHeight() > this.getHeight() || i.getDepth() > this.getDepth() ||
                    ((i.getBottomLeftFront().getX() + i.getWidth()) > this.width) || ((i.getBottomLeftFront().getY() + i.getHeight()) > this.height) ||
                    ((i.getBottomLeftFront().getZ() + i.getDepth()) > this.depth))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool dimensionsFitRight(Item i)
        {
            if (i.getWidth() > this.getWidth() || i.getHeight() > this.getHeight() || i.getDepth() > this.getDepth() ||
                    ((i.getBottomRightFront().getX() - i.getWidth()) < 0) || ((i.getBottomRightFront().getY() + i.getHeight()) > this.height) ||
                    ((i.getBottomRightFront().getZ() + i.getDepth()) > this.depth))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /**
         * CONSTRAINT: check weight of item, if added to other items in container it exceeds or not maxWeight
         * @param i
         * @return
         */
        public bool weightFits(Item i)
        {
            if ((i.getWeight() + loadedWeight) > maxWeight)
            {
                return false;
            }
            return true;
        }

        /**
         * CONSTRAINT: overlapping items
         * @param i
         * @return TRUE or FALSE
         */
        public bool itemsOverlapping(Item i)
        {
            double minX = i.bottomLeftFront.x;
            double minY = i.bottomLeftFront.y;
            double minZ = i.bottomLeftFront.z;
            double maxX = i.topRightRear.x;
            double maxY = i.topRightRear.y;
            double maxZ = i.topRightRear.z;

            //TRY
            bool removed = itemsLoaded.Remove(i);

            foreach (Item item in itemsLoaded)
            {
                OverlapChecks++;
                if ((maxX > item.bottomLeftFront.x) && (item.topRightRear.x > minX) && (maxY > item.bottomLeftFront.y) &&
                        (item.topRightRear.y > minY) && (maxZ > item.bottomLeftFront.z) && (item.topRightRear.z > minZ))
                {
                    return true;
                }
            }
            return false;
        }


        public void addToZone(Item i, int zone)
        {
            if (zone == 1)
            {
                zone1Items.Add(i);
            }
            else if (zone == 2)
            {
                zone2Items.Add(i);
            }
            else if (zone == 3)
            {
                zone3Items.Add(i);
            }
            else
            {
                zone4Items.Add(i);
            }
        }

        public bool checkAddItem(Item i, List<PotentialPoint> supportPoints, double minHeight)
        {
            //SHUFFLE
            if (supportPoints.Any() && this.weightFits(i))
            {
                //            supportPoints.sort(Comparator.comparing(PotentialPoint::getY).thenComparing(PotentialPoint::getZ));
                //            int randomNum = ThreadLocalRandom.current().nextInt(1, 6);
                for (int j = 0; j < supportPoints.Count; j++)
                {
                    PotentialPoint supportPoint = supportPoints[j];
                    i.setBottomLeftFront(supportPoint);
                    if ((!this.itemsOverlapping(i)) && (this.dimensionsFit(i)) && (supportPoint.cornerCheck(i, this, minHeight)))
                    {

                        PotentialPoint cPointXAxis = PotentialPoint.createXAxisPotentialPoint(i, j, supportPoints);
                        PotentialPoint cPointZAxis = PotentialPoint.createZAxisPotentialPoint(i, j, supportPoints);

                        PotentialPoint.addPointToPotentialPoints(cPointXAxis, supportPoints);
                        PotentialPoint.addPointToPotentialPoints(cPointZAxis, supportPoints);

                        if (i.isStackable())
                        {
                            PotentialPoint cPointYAxis = PotentialPoint.createYAxisPotentialPoint(i, j, supportPoints);
                            PotentialPoint.addPointToPotentialPoints(cPointYAxis, supportPoints);
                        }

                        PotentialPoint cPointXAxisProjectionMax = PotentialPoint.createXAxisPotentialPointProjectionMax(i, j, supportPoints);
                        PotentialPoint.addPointToPotentialPoints(cPointXAxisProjectionMax, supportPoints);

                        /*Remove chosen support point*/
                        PotentialPoint.usedPotentialPoints.Add(supportPoints[j]);
                        bool removed = supportPoints.Remove(supportPoints[j]);
                        return true;
                    }


                }

            }
            return false;
        }

        /**
         *
         * @param i
         * @param supportPoints
         * @param minHeight
         * @return
         */
        public bool checkAddItemRevisited(Item i, List<PotentialPoint> supportPoints, double minHeight)
        {
            StopwatchTimer.Start("add");
            double zone1, zone2, zone3, zone4;
            zone1 = this.getZonesDepth();
            zone2 = zone1 * 2;
            zone3 = zone1 * 3;
            zone4 = this.getDepth();
            bool weightOk = true;

            double rnd = StaticRandom.NextDouble();

            supportPoints.Sort((x, y) =>
            {
                if (x.getY() > y.getY())
                {
                    return 1;
                }
                else if (x.getY() == y.getY())
                {
                    return 0;
                }

                return -1;
            });

            if (supportPoints.Any() && this.weightFits(i))
            {

                for (int j = 0; j < supportPoints.Count; j++)
                {
                    PotentialPoint supportPoint = supportPoints[j];
                    i.setBottomLeftFront(supportPoint);


                    if (((i.bottomLeftFront.getZ() < zone1) && (this.getZone1Weight() + i.getWeight() > this.maxZone1Weight * (1 + weightSurplusPerameter)))
                            || (((i.bottomLeftFront.z >= zone1) && (i.bottomLeftFront.getZ() < zone2)) && (this.getZone2Weight() + i.getWeight() > this.maxZone2Weight * (1 + weightSurplusPerameter)))
                            || (((i.bottomLeftFront.z >= zone2) && (i.bottomLeftFront.getZ() < zone3)) && (this.getZone3Weight() + i.getWeight() > this.maxZone3Weight * (1 + weightSurplusPerameter)))
                            || (((i.bottomLeftFront.z >= zone3) && (i.bottomLeftFront.getZ() < zone4)) && (this.getZone4Weight() + i.getWeight() > this.maxZone3Weight * (1 + weightSurplusPerameter))))
                    {

                        weightOk = false;
                    }
                    else
                    {
                        weightOk = true;
                    }



                    if ((!this.itemsOverlapping(i)) && (this.dimensionsFit(i)) && (supportPoint.cornerCheck(i, this, minHeight)) && weightOk)
                    {



                        PotentialPoint cPointXAxis = PotentialPoint.createXAxisPotentialPoint(i, j, supportPoints);
                        PotentialPoint cPointZAxis = PotentialPoint.createZAxisPotentialPoint(i, j, supportPoints);
                        // TEST---------------
                        if (cPointXAxis.x != 0)
                        {
                            PotentialPoint nspZzero = new PotentialPoint(0, cPointXAxis.y, cPointXAxis.z);
                            PotentialPoint.addPointToPotentialPoints(nspZzero, supportPoints);
                        }

                        PotentialPoint.addPointToPotentialPoints(cPointXAxis, supportPoints);
                        PotentialPoint.addPointToPotentialPoints(cPointZAxis, supportPoints);

                        if (i.isStackable())
                        {

                            PotentialPoint cPointYAxis = PotentialPoint.createYAxisPotentialPoint(i, j, supportPoints);
                            PotentialPoint.addPointToPotentialPoints(cPointYAxis, supportPoints);

                            int dim = supportPoints.Count;
                            List<PotentialPoint> newList = new List<PotentialPoint>();
                            double maxSubst;

                            foreach (PotentialPoint sp in supportPoints)
                            {
                                if ((!sp.equals(cPointYAxis)) && sp.getZ() == cPointYAxis.getZ() && sp.getY() == cPointYAxis.getY())
                                {


                                    if (sp.getMaxXAllowed() > cPointYAxis.getMaxXAllowed())
                                    {
                                        maxSubst = sp.getMaxXAllowed();
                                    }
                                    else
                                    {
                                        maxSubst = cPointYAxis.getMaxXAllowed();
                                    }
                                    PotentialPoint toAdd = new PotentialPoint(sp.getX(), sp.getY(), sp.getZ(), maxSubst, sp.getMaxYAllowed(), sp.getMaxZAllowed());
                                    newList.Add(toAdd);
                                }
                            }

                            foreach (PotentialPoint sp in newList)
                            {
                                //                        System.err.println("ADDED");
                                supportPoints.Add(sp);
                            }
                        }

                        PotentialPoint cPointXAxisProjectionMax = PotentialPoint.createXAxisPotentialPointProjectionMax(i, j, supportPoints);
                        PotentialPoint.addPointToPotentialPoints(cPointXAxisProjectionMax, supportPoints);


                        /*°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°*/

                        /*Remove chosen support point*/
                        PotentialPoint.usedPotentialPoints.Add(supportPoints[j]);
                        bool removed = supportPoints.Remove(supportPoints[j]);

                        StopwatchTimer.Stop("add");
                        return true;
                    }


                }

            }

            StopwatchTimer.Stop("add");
            return false;
        }

        public void addItem(Item item)
        {
            itemsLoaded.Add(item);

            numItemsLoaded++;
            this.loadedWeight = loadedWeight + item.getWeight();
            this.remainingVolume = this.remainingVolume - item.getVolume();

            double zone1, zone2, zone3, zone4;
            zone1 = this.getZonesDepth();
            zone2 = zone1 * 2;
            zone3 = zone1 * 3;
            zone4 = this.getDepth();
            double distrWeight1, distrWeight2;


            if (item.bottomLeftFront.getZ() < zone1)
            {
                zone1Items.Add(item);
                if (item.bottomLeftRear.getZ() < zone1)
                {
                    zone1Weight += item.getWeight();
                }
                else if ((item.bottomLeftRear.getZ() >= zone1) && (item.bottomLeftRear.getZ() < zone2))
                {
                    distrWeight1 = ((zone1 - item.bottomLeftFront.getZ()) / item.getDepth()) * item.getWeight();
                    distrWeight2 = ((item.bottomLeftRear.getZ() - zone1) / item.getDepth()) * item.getWeight();
                    zone1Weight += distrWeight1;
                    zone2Weight += distrWeight2;
                }
            }
            else if ((item.bottomLeftFront.z >= zone1) && (item.bottomLeftFront.getZ() < zone2))
            {
                zone2Items.Add(item);
                if (item.bottomLeftRear.getZ() < zone2)
                {
                    zone2Weight += item.getWeight();
                }
                else if ((item.bottomLeftRear.getZ() >= zone2) && (item.bottomLeftRear.getZ() < zone3))
                {
                    distrWeight1 = ((zone2 - item.bottomLeftFront.getZ()) / item.getDepth()) * item.getWeight();
                    distrWeight2 = ((item.bottomLeftRear.getZ() - zone2) / item.getDepth()) * item.getWeight();
                    zone2Weight += distrWeight1;
                    zone3Weight += distrWeight2;
                }
            }
            else if ((item.bottomLeftFront.z >= zone2) && (item.bottomLeftFront.getZ() < zone3))
            {
                zone3Items.Add(item);
                if (item.bottomLeftRear.getZ() < zone3)
                {
                    zone3Weight += item.getWeight();
                }
                else if ((item.bottomLeftRear.getZ() >= zone3) && (item.bottomLeftRear.getZ() < zone4))
                {
                    distrWeight1 = ((zone3 - item.bottomLeftFront.getZ()) / item.getDepth()) * item.getWeight();
                    distrWeight2 = ((item.bottomLeftRear.getZ() - zone3) / item.getDepth()) * item.getWeight();
                    zone3Weight += distrWeight1;
                    zone4Weight += distrWeight2;
                }
            }
            else if ((item.bottomLeftFront.z >= zone3) && (item.bottomLeftFront.getZ() < zone4))
            {
                zone4Items.Add(item);
                zone4Weight += item.getWeight();
            }

        }

        /**
         *
         * @param i
         * @param supportPoints
         * @param minHeight
         * @return
         */
        public bool checkAddItemRevisitedRight(Item i, List<PotentialPoint> supportPoints, double minHeight)
        {
            StopwatchTimer.Start("add");
            double zone1, zone2, zone3, zone4;
            zone1 = this.getZonesDepth();
            zone2 = zone1 * 2;
            zone3 = zone1 * 3;
            zone4 = this.getDepth();
            bool weightOk = true;

            double rnd = StaticRandom.NextDouble();



            //        Need to use usableSpace in relation with item to be added
            //        supportPoints.sort(Comparator.comparing(PotentialPoint::getUsableSpace));
            if (supportPoints.Any() && this.weightFits(i))
            {

                for (int j = 0; j < supportPoints.Count; j++)
                {
                    PotentialPoint supportPoint = supportPoints[j];
                    i.setBottomRightFront(supportPoint);


                    if (((i.bottomLeftFront.getZ() < zone1) && (this.getZone1Weight() + i.getWeight() > this.maxZone1Weight * (1 + weightSurplusPerameter)))
                            || (((i.bottomLeftFront.z >= zone1) && (i.bottomLeftFront.getZ() < zone2)) && (this.getZone2Weight() + i.getWeight() > this.maxZone2Weight * (1 + weightSurplusPerameter)))
                            || (((i.bottomLeftFront.z >= zone2) && (i.bottomLeftFront.getZ() < zone3)) && (this.getZone3Weight() + i.getWeight() > this.maxZone3Weight * (1 + weightSurplusPerameter)))
                            || (((i.bottomLeftFront.z >= zone3) && (i.bottomLeftFront.getZ() < zone4)) && (this.getZone4Weight() + i.getWeight() > this.maxZone3Weight * (1 + weightSurplusPerameter))))
                    {

                        weightOk = false;
                    }
                    else
                    {
                        weightOk = true;
                    }



                    if ((!this.itemsOverlapping(i)) && (this.dimensionsFitRight(i)) && (supportPoint.cornerCheck(i, this, minHeight)) && weightOk)
                    {



                        PotentialPoint cPointXAxis = PotentialPoint.createXAxisPotentialPoint(i, j, supportPoints);
                        PotentialPoint cPointZAxis = PotentialPoint.createZAxisPotentialPointRight(i, j, supportPoints);
                        // TEST---------------
                        if (cPointXAxis.x != 0)
                        {
                            PotentialPoint nspZzero = new PotentialPoint(0, cPointXAxis.y, cPointXAxis.z);
                            PotentialPoint.addPointToPotentialPoints(nspZzero, supportPoints);
                        }

                        PotentialPoint.addPointToPotentialPoints(cPointXAxis, supportPoints);
                        PotentialPoint.addPointToPotentialPoints(cPointZAxis, supportPoints);

                        if (i.isStackable())
                        {

                            PotentialPoint cPointYAxis = PotentialPoint.createYAxisPotentialPointRight(i, j, supportPoints);
                            PotentialPoint.addPointToPotentialPoints(cPointYAxis, supportPoints);

                            int dim = supportPoints.Count;
                            List<PotentialPoint> newList = new List<PotentialPoint>();
                            double maxSubst;
                            foreach (PotentialPoint sp in supportPoints)
                            {
                                if ((!sp.equals(cPointYAxis)) && sp.getZ() == cPointYAxis.getZ() && sp.getY() == cPointYAxis.getY())
                                {


                                    if (sp.getMaxXAllowed() > cPointYAxis.getMaxXAllowed())
                                    {
                                        maxSubst = sp.getMaxXAllowed();
                                    }
                                    else
                                    {
                                        maxSubst = cPointYAxis.getMaxXAllowed();
                                    }
                                    PotentialPoint toAdd = new PotentialPoint(sp.getX(), sp.getY(), sp.getZ(), maxSubst, sp.getMaxYAllowed(), sp.getMaxZAllowed());
                                    newList.Add(toAdd);
                                }
                            }

                            foreach (PotentialPoint sp in newList)
                            {
                                //                        System.err.println("ADDED");
                                supportPoints.Add(sp);
                            }
                        }

                        /*                    PotentialPoint cPointXAxisProjectionMax = PotentialPoint.createXAxisPotentialPointProjectionMax(i, j, supportPoints);
                                            PotentialPoint.addPointToPotentialPoints(cPointXAxisProjectionMax, supportPoints);*/

                        PotentialPoint cPointProjectionZ = PotentialPoint.createPointProjectionZ(i, j, supportPoints);
                        PotentialPoint.addPointToPotentialPoints(cPointProjectionZ, supportPoints);


                        /*°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°*/

                        /*Remove chosen support point*/
                        PotentialPoint.usedPotentialPoints.Add(supportPoints[j]);
                        supportPoints.Remove(supportPoints[j]);

                        StopwatchTimer.Stop("add");
                        return true;
                    }


                }

            }

            StopwatchTimer.Stop("add");
            return false;
        }

        public void addItemRight(Item item)
        {
            itemsLoaded.Add(item);

            numItemsLoaded++;
            this.loadedWeight = loadedWeight + item.getWeight();
            this.remainingVolume = this.remainingVolume - item.getVolume();

            double zone1, zone2, zone3, zone4;
            zone1 = this.getZonesDepth();
            zone2 = zone1 * 2;
            zone3 = zone1 * 3;
            zone4 = this.getDepth();
            double distrWeight1, distrWeight2;


            if (item.bottomLeftFront.getZ() < zone1)
            {
                zone1Items.Add(item);
                if (item.bottomLeftRear.getZ() < zone1)
                {
                    zone1Weight += item.getWeight();
                }
                else if ((item.bottomLeftRear.getZ() >= zone1) && (item.bottomLeftRear.getZ() < zone2))
                {
                    distrWeight1 = ((zone1 - item.bottomLeftFront.getZ()) / item.getDepth()) * item.getWeight();
                    distrWeight2 = ((item.bottomLeftRear.getZ() - zone1) / item.getDepth()) * item.getWeight();
                    zone1Weight += distrWeight1;
                    zone2Weight += distrWeight2;
                }
            }
            else if ((item.bottomLeftFront.z >= zone1) && (item.bottomLeftFront.getZ() < zone2))
            {
                zone2Items.Add(item);
                if (item.bottomLeftRear.getZ() < zone2)
                {
                    zone2Weight += item.getWeight();
                }
                else if ((item.bottomLeftRear.getZ() >= zone2) && (item.bottomLeftRear.getZ() < zone3))
                {
                    distrWeight1 = ((zone2 - item.bottomLeftFront.getZ()) / item.getDepth()) * item.getWeight();
                    distrWeight2 = ((item.bottomLeftRear.getZ() - zone2) / item.getDepth()) * item.getWeight();
                    zone2Weight += distrWeight1;
                    zone3Weight += distrWeight2;
                }
            }
            else if ((item.bottomLeftFront.z >= zone2) && (item.bottomLeftFront.getZ() < zone3))
            {
                zone3Items.Add(item);
                if (item.bottomLeftRear.getZ() < zone3)
                {
                    zone3Weight += item.getWeight();
                }
                else if ((item.bottomLeftRear.getZ() >= zone3) && (item.bottomLeftRear.getZ() < zone4))
                {
                    distrWeight1 = ((zone3 - item.bottomLeftFront.getZ()) / item.getDepth()) * item.getWeight();
                    distrWeight2 = ((item.bottomLeftRear.getZ() - zone3) / item.getDepth()) * item.getWeight();
                    zone3Weight += distrWeight1;
                    zone4Weight += distrWeight2;
                }
            }
            else if ((item.bottomLeftFront.z >= zone3) && (item.bottomLeftFront.getZ() < zone4))
            {
                zone4Items.Add(item);
                zone4Weight += item.getWeight();
            }

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

        public double getVolume()
        {
            return (this.width / 100) * (this.height / 100) * (this.depth / 100);
        }

        public double getUsedVolume(List<Item> itemsLoaded)
        {
            double vol = 0;
            foreach (Item i in itemsLoaded)
            {
                vol += i.getVolume();
            }
            return vol;
        }


        public double getWidth()
        {
            return width;
        }

        public void setWidth(double width)
        {
            this.width = width;
        }

        public double getHeight()
        {
            return height;
        }

        public void setHeight(double height)
        {
            this.height = height;
        }

        public double getDepth()
        {
            return depth;
        }

        public void setDepth(double depth)
        {
            this.depth = depth;
        }

        public double getMaxWeight()
        {
            return maxWeight;
        }

        public void setMaxWeight(double maxWeight)
        {
            this.maxWeight = maxWeight;
        }

        public bool isUnloadableFromSide()
        {
            return unloadableFromSide;
        }

        public void setUnloadableFromSide(bool unloadableFromSide)
        {
            this.unloadableFromSide = unloadableFromSide;
        }

        public double getLoadedWeight()
        {
            return loadedWeight;
        }

        public void setLoadedWeight(double loadedWeight)
        {
            this.loadedWeight = loadedWeight;
        }

        public List<Item> getItemsLoaded()
        {
            return itemsLoaded;
        }

        public void setItemsLoaded(List<Item> itemsLoaded)
        {
            this.itemsLoaded = itemsLoaded;
        }

        public int getNumItemsLoaded()
        {
            return numItemsLoaded;
        }

        public void setNumItemsLoaded(int numItemsLoaded)
        {
            this.numItemsLoaded = numItemsLoaded;
        }

        public double getRemainingVolume()
        {
            return remainingVolume;
        }

        public void setRemainingVolume(double remainingVolume)
        {
            this.remainingVolume = remainingVolume;
        }

        public double getRemainingWeight()
        {
            return remainingWeight;
        }

        public void setRemainingWeight(double remainingWeight)
        {
            this.remainingWeight = remainingWeight;
        }

        public Container unloadEverything()
        {
            this.setZone1Weight(0);
            this.setZone2Weight(0);
            this.setZone3Weight(0);
            this.setZone4Weight(0);
            this.setZone1Items(new List<Item>());
            this.setZone2Items(new List<Item>());
            this.setZone3Items(new List<Item>());
            this.setZone4Items(new List<Item>());
            return new Container(this.width, this.height, this.depth, this.maxWeight, this.unloadableFromSide);
        }

        public void unloadContainer()
        {
            itemsLoaded = new List<Item>();
            this.setLoadedWeight(0);
        }

        public Point getCentre()
        {
            double x, y, z;
            x = this.getWidth() / 2;
            y = this.getHeight() / 2;
            z = this.getDepth() / 2;

            Point p = new Point(x, y, z);
            return p;
        }

        /** Definition of values and functions to ensure weight distribution between zones of the cargo
         *
         */

        public double zonesDepth;

        public double getZonesDepth()
        {
            zonesDepth = this.depth / 4;
            return zonesDepth;
        }



        public void loadedItemsInZone(List<Item> loadedItems)
        {
            double zone1, zone2, zone3, zone4;
            zone1 = this.getZonesDepth();
            zone2 = zone1 * 2;
            zone3 = zone1 * 3;
            zone4 = this.getDepth();
            double distrWeight1, distrWeight2;

            foreach (Item item in loadedItems)
            {
                if (item.bottomLeftFront.getZ() < zone1)
                {
                    zone1Items.Add(item);
                    if (item.bottomLeftRear.getZ() < zone1)
                    {
                        zone1Weight += item.getWeight();
                    }
                    else if ((item.bottomLeftRear.getZ() >= zone1) && (item.bottomLeftRear.getZ() < zone2))
                    {
                        distrWeight1 = ((zone1 - item.bottomLeftFront.getZ()) / item.getDepth()) * item.getWeight();
                        distrWeight2 = ((item.bottomLeftRear.getZ() - zone1) / item.getDepth()) * item.getWeight();
                        zone1Weight += distrWeight1;
                        zone2Weight += distrWeight2;
                    }
                }
                else if ((item.bottomLeftFront.z >= zone1) && (item.bottomLeftFront.getZ() < zone2))
                {
                    zone2Items.Add(item);
                    if (item.bottomLeftRear.getZ() < zone2)
                    {
                        zone2Weight += item.getWeight();
                    }
                    else if ((item.bottomLeftRear.getZ() >= zone2) && (item.bottomLeftRear.getZ() < zone3))
                    {
                        distrWeight1 = ((zone2 - item.bottomLeftFront.getZ()) / item.getDepth()) * item.getWeight();
                        distrWeight2 = ((item.bottomLeftRear.getZ() - zone2) / item.getDepth()) * item.getWeight();
                        zone2Weight += distrWeight1;
                        zone3Weight += distrWeight2;
                    }
                }
                else if ((item.bottomLeftFront.z >= zone2) && (item.bottomLeftFront.getZ() < zone3))
                {
                    zone3Items.Add(item);
                    if (item.bottomLeftRear.getZ() < zone3)
                    {
                        zone3Weight += item.getWeight();
                    }
                    else if ((item.bottomLeftRear.getZ() >= zone3) && (item.bottomLeftRear.getZ() < zone4))
                    {
                        distrWeight1 = ((zone3 - item.bottomLeftFront.getZ()) / item.getDepth()) * item.getWeight();
                        distrWeight2 = ((item.bottomLeftRear.getZ() - zone3) / item.getDepth()) * item.getWeight();
                        zone3Weight += distrWeight1;
                        zone4Weight += distrWeight2;
                    }
                }
                else if ((item.bottomLeftFront.z >= zone3) && (item.bottomLeftFront.getZ() < zone4))
                {
                    zone4Items.Add(item);
                    zone4Weight += item.getWeight();
                }
            }

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

    }
}
