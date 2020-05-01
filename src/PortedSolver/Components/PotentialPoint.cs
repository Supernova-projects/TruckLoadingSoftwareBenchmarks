using System;
using System.Collections.Generic;
using System.Text;
using PortedSolver.Utils;

namespace PortedSolver.Components
{
    public class PotentialPoint : Point
    {
        private static readonly double EXCEEDING_PARAM = Configuration.ITEM_EXCEEDING_PARAMETER;
        private double maxXAllowed, maxYAllowed, maxZAllowed, usableSpace;
        public static List<PotentialPoint> usedPotentialPoints = new List<PotentialPoint>();
        public PotentialPoint(double x, double y, double z)
            : base(x, y, z)
        {
        }

        // In order to set maxX etc. we need to know also the position and dimensions of chosen corner point where to position item
        public PotentialPoint(double x, double y, double z, double maxXAllowed, double maxYAllowed, double maxZAllowed)
            : base(x, y, z)
        {
            this.maxXAllowed = maxXAllowed;
            this.maxYAllowed = maxYAllowed;
            this.maxZAllowed = maxZAllowed;
            this.usableSpace = maxXAllowed * maxYAllowed * maxZAllowed;
        }

        public static void setUsedPotentialPoints(List<PotentialPoint> usedPotentialPoints)
        {
            PotentialPoint.usedPotentialPoints = usedPotentialPoints;
        }

        public double getUsableSpace()
        {
            return usableSpace;
        }

        public double getUsableSpaceItem(Item i)
        {
            return i.getVolume() / this.usableSpace;
        }

        public void setUsableSpace(double maxXAllowed, double maxYAllowed, double maxZAllowed)
        {
            this.usableSpace = maxXAllowed * maxYAllowed * maxZAllowed;
        }

        /** New approach for 3D corners: Add extreme points only when you Add an item so that it's easier to define them. In particular you define
         * 3 corner points for each item, one foreach axe: x y z
         * */


        public static PotentialPoint createXAxisPotentialPoint(Item i, int j, List<PotentialPoint> potentialPoints)
        {
            PotentialPoint cPointXAxis = new PotentialPoint(i.getBottomRightFront().getX(), i.getBottomLeftFront().getY(), i.getBottomLeftFront().getZ(),
                    potentialPoints[j].getMaxXAllowed() - i.getWidth(),
                    potentialPoints[j].getMaxYAllowed(),
                    potentialPoints[j].getMaxZAllowed() - i.getDepth());
            return cPointXAxis;
        }

        public static PotentialPoint createXAxisPotentialPointRight(Item i, int j, List<PotentialPoint> potentialPoints)
        {
            PotentialPoint cPointXAxis = new PotentialPoint(i.getBottomRightFront().getX() - i.getWidth(), i.getBottomLeftFront().getY(), i.getBottomLeftFront().getZ(),
                    potentialPoints[j].getMaxXAllowed() - i.getWidth(),
                    potentialPoints[j].getMaxYAllowed(),
                    potentialPoints[j].getMaxZAllowed() - i.getDepth());
            return cPointXAxis;
        }


        public static PotentialPoint createPointProjectionZ(Item i, int j, List<PotentialPoint> potentialPoints)
        {
            PotentialPoint cPointXAxis = new PotentialPoint(0, i.getBottomLeftFront().getY(), i.getBottomRightFront().getZ(),
                    potentialPoints[j].getMaxXAllowed() - i.getWidth(),
                    potentialPoints[j].getMaxYAllowed(),
                    potentialPoints[j].getMaxZAllowed() - i.getDepth());
            return cPointXAxis;
        }

        public static PotentialPoint createYAxisPotentialPoint(Item i, int j, List<PotentialPoint> potentialPoints)
        {
            PotentialPoint cPointYAxis = new PotentialPoint(i.getBottomLeftFront().getX(), i.getBottomLeftFront().getY() + i.getHeight(), i.getBottomLeftFront().getZ(),
                    i.getWidth(),
                    potentialPoints[j].getMaxYAllowed() - i.getHeight(),
                    i.getDepth());

            return cPointYAxis;
        }

        public static PotentialPoint createYAxisPotentialPointRight(Item i, int j, List<PotentialPoint> potentialPoints)
        {
            PotentialPoint cPointYAxis = new PotentialPoint(i.getBottomRightFront().getX(), i.getBottomRightFront().getY() + i.getHeight(), i.getBottomLeftFront().getZ(),
                    i.getWidth(),
                    potentialPoints[j].getMaxYAllowed() - i.getHeight(),
                    i.getDepth());

            return cPointYAxis;
        }

        public static PotentialPoint createZAxisPotentialPoint(Item i, int j, List<PotentialPoint> potentialPoints)
        {
            PotentialPoint cPointZAxis = new PotentialPoint(i.getBottomLeftFront().getX(), i.getBottomLeftFront().getY(), i.getBottomLeftFront().getZ() + i.getDepth(),
                    potentialPoints[j].getMaxXAllowed(),
                    potentialPoints[j].getMaxYAllowed(),
                    potentialPoints[j].getMaxZAllowed() - i.getDepth());
            return cPointZAxis;
        }

        public static PotentialPoint createZAxisPotentialPointRight(Item i, int j, List<PotentialPoint> potentialPoints)
        {
            PotentialPoint cPointZAxis = new PotentialPoint(i.getBottomRightFront().getX(), i.getBottomRightFront().getY(), i.getBottomRightFront().getZ() + i.getDepth(),
                    potentialPoints[j].getMaxXAllowed(),
                    potentialPoints[j].getMaxYAllowed(),
                    potentialPoints[j].getMaxZAllowed() - i.getDepth());
            return cPointZAxis;
        }

        public static PotentialPoint createXAxisPotentialPointProjectionMin(Item i, int j, List<PotentialPoint> potentialPoints)
        {
            PotentialPoint cPointXAxisProjection = new PotentialPoint(0, i.getBottomLeftRear().getY(), i.getBottomLeftRear().getZ(), potentialPoints[j].getMaxXAllowed(),
                    potentialPoints[j].getMaxYAllowed(),
                    potentialPoints[j].getMaxZAllowed() - i.getDepth());
            return cPointXAxisProjection;
        }
        public static PotentialPoint createXAxisPotentialPointProjectionMax(Item i, int j, List<PotentialPoint> potentialPoints)
        {
            PotentialPoint cPointXAxisProjection = new PotentialPoint(i.getBottomRightRear().getX(), 0, i.getBottomLeftRear().getZ(), potentialPoints[j].getMaxXAllowed(),
                    potentialPoints[j].getMaxYAllowed(),
                    potentialPoints[j].getMaxZAllowed() - i.getDepth());
            return cPointXAxisProjection;
        }

        public static void addPotentialPoints(Item i, List<PotentialPoint> potentialPoints)
        {

            PotentialPoint xAxis = new PotentialPoint(i.getBottomLeftFront().x + i.getWidth(), i.getBottomLeftFront().y, i.getBottomLeftFront().z);
            PotentialPoint yAxis = new PotentialPoint(i.getBottomLeftFront().x, i.getBottomLeftFront().y + i.getHeight(), i.getBottomLeftFront().z);
            PotentialPoint zAxis = new PotentialPoint(i.getBottomLeftFront().x, i.getBottomLeftFront().y, i.getBottomLeftFront().z + i.getDepth());

            if (!potentialPoints.Contains(xAxis))
            {
                potentialPoints.Add(xAxis);
            }
            if (!potentialPoints.Contains(yAxis) && i.isStackable())
            {
                potentialPoints.Add(yAxis);
            }
            if (!potentialPoints.Contains(zAxis))
            {
                potentialPoints.Add(zAxis);
            }
            for (int j = 0; j < potentialPoints.Count; j++)
            {
                if (potentialPoints[j].equals(i.getBottomLeftFront()))
                {
                    potentialPoints.RemoveAt(j);
                }
            }
        }

        public double getMaxXAllowed()
        {
            return maxXAllowed;
        }

        public void setMaxXAllowed(double maxXAllowed)
        {
            this.maxXAllowed = maxXAllowed;
        }

        public double getMaxYAllowed()
        {
            return maxYAllowed;
        }

        public void setMaxYAllowed(double maxYAllowed)
        {
            this.maxYAllowed = maxYAllowed;
        }

        public double getMaxZAllowed()
        {
            return maxZAllowed;
        }

        public void setMaxZAllowed(double maxZAllowed)
        {
            this.maxZAllowed = maxZAllowed;
        }

        /**
         *
         * @param p
         * @param potentialPoints
         */
        public static void addPointToPotentialPoints(PotentialPoint p, List<PotentialPoint> potentialPoints)
        {
            potentialPoints.Add(p);
        }

        public static void addToUsedPotentialPoints(PotentialPoint sp)
        {
            usedPotentialPoints.Add(sp);
        }
        public static List<PotentialPoint> getUsedPotentialPoints()
        {
            return usedPotentialPoints;
        }

        //    public static List<PotentialPoint> getPotentialPoints() {
        //        return potentialPoints;
        //    }

        /*    public static void setPotentialPoints(List<PotentialPoint> potentialPoints) {
                PotentialPoint.potentialPoints = potentialPoints;
            }*/

        public bool cornerCheckWithLimit(Item i, Double low_limit, Double high_limit)
        {
            if ((this.z) > low_limit && (i.getDepth() + this.z) < high_limit)
            {
                return false;
            }
            return true;
        }

        /**
         * Define how much (in percentage) a package stacked on top of another can exceed it.
         */
        public bool cornerCheck(Item i, Container c, double minHeight)
        {
            //        System.err.println("width: " + i.getWidth() + "maxAllowed: " + this.maxXAllowed + "height: " + i.getHeight() + "maxAllowed: " + this.maxYAllowed + "depth: " + i.getDepth()+ "maxAllowed: " + this.maxZAllowed + "\n");
            //        Change the stupid 11 parameter to the smallest height of items in list
            if (((i.getDepth()) > (1 + EXCEEDING_PARAM) * (this.maxZAllowed)) || ((i.getWidth()) > (1 + EXCEEDING_PARAM) * (this.maxXAllowed)) ||
                    (i.getHeight() > this.maxYAllowed) /*|| (i.getTopLeftFront().getY() > (c.getHeight()-minHeight))*/)
            {
                return false;
            }
            return true;
        }

        /**
         * Just for debug reasons
         */
        //public static void showPotentialPoints(List<PotentialPoint> potentialPoints)
        //{
        //    foreach (PotentialPoint cp in potentialPoints)
        //    {
        //        System.out.println("PotentialPoint: (" + cp.x + ";" + cp.y + ";" + cp.z + ")");
        //    }

        //}

        ////Ascending order based on (X,Y, or Z)
        //public static List<PotentialPoint> orderPotentialPointsX(List<PotentialPoint> cPoints)
        //{
        //    PotentialPoint tmp;
        //    for (int i = 0; i < cPoints.size() - 1; i++)
        //    {
        //        for (int j = 0; j < cPoints.size() - i - 1; j++)
        //        {
        //            if (cPoints[j].getX() > cPoints.get(j + 1).getX())
        //            {
        //                tmp = cPoints[j];
        //                cPoints.set(j, cPoints.get(j + 1));
        //                cPoints.set(j + 1, tmp);
        //            }
        //        }
        //    }
        //    return cPoints;
        //}
        //public static List<PotentialPoint> orderPotentialPointsZ(List<PotentialPoint> cPoints)
        //{
        //    PotentialPoint tmp;
        //    for (int i = 0; i < cPoints.size() - 1; i++)
        //    {
        //        for (int j = 0; j < cPoints.size() - i - 1; j++)
        //        {
        //            if (cPoints[j].getZ() > cPoints.get(j + 1).getZ())
        //            {
        //                tmp = cPoints[j];
        //                cPoints.set(j, cPoints.get(j + 1));
        //                cPoints.set(j + 1, tmp);
        //            }
        //        }
        //    }
        //    return cPoints;
        //}

        //public static List<PotentialPoint> orderPotentialPointsY(List<PotentialPoint> cPoints)
        //{
        //    PotentialPoint tmp;
        //    for (int i = 0; i < cPoints.size() - 1; i++)
        //    {
        //        for (int j = 0; j < cPoints.size() - i - 1; j++)
        //        {
        //            if (cPoints[j].getY() > cPoints.get(j + 1).getY())
        //            {
        //                tmp = cPoints[j];
        //                cPoints.set(j, cPoints.get(j + 1));
        //                cPoints.set(j + 1, tmp);
        //            }
        //        }
        //    }
        //    return cPoints;
        //}

        public override string ToString()
        {
            return "(x: " + x + ", y: " + y + ", z: " + z + ", maxX: " + maxXAllowed + ", maxY: " + maxYAllowed + ", maxZ: " + maxZAllowed + ")\n";
        }

    }
}
