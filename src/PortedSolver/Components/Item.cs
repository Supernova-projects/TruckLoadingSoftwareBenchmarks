using System;
using System.Collections.Generic;
using System.Text;

namespace PortedSolver.Components
{
    public class Item
    {

        private int COVERSION_RATE = 1; // items are already presented in cm
        private int id, customer, priority, unloadingOrder, shipmentCode;
        private double width, height, depth, weight, volume, taxability, baseSimilarity, taxabilityOverVolume, taxabilityOverWeight;
        private bool stackable;
        public Point topLeftRear, topRightRear, topLeftFront, topRightFront, bottomLeftRear, bottomRightRear, bottomLeftFront, bottomRightFront;


        public Item(int id, double width, double height, double depth, double weight, bool stackable, int priority, int unloadingOrder, double taxability, int shipmentCode)
        {
            this.id = id;
            this.width = width;
            this.height = height;
            this.depth = depth;
            this.weight = weight;
            this.volume = (width * depth * height); //in cm^3
            this.stackable = stackable;
            this.baseSimilarity = Math.Abs(this.width - this.depth);
            this.priority = priority;
            this.unloadingOrder = unloadingOrder;
            this.taxability = taxability;
            this.taxabilityOverVolume = (this.taxability) / (this.volume);
            this.taxabilityOverWeight = (this.taxability) / (this.weight);
            this.shipmentCode = shipmentCode;
        }


        public int getShipmentCode()
        {
            return shipmentCode;
        }

        public void setShipmentCode(int shipmentCode)
        {
            this.shipmentCode = shipmentCode;
        }

        public void setTaxabilityOverVolume(double taxabilityOverVolume)
        {
            this.taxabilityOverVolume = taxabilityOverVolume;
        }

        public void setTaxabilityOverWeight(double taxabilityOverWeight)
        {
            this.taxabilityOverWeight = taxabilityOverWeight;
        }

        public int getId()
        {
            return id;
        }
        public void setId(int id)
        {
            this.id = id;
        }
        public double getWeight()
        {
            return weight;
        }
        public void setWeight(double weight)
        {
            this.weight = weight;
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
        public bool isStackable()
        {
            return stackable;
        }
        public void setStackable(bool stackable)
        {
            this.stackable = stackable;
        }
        public int getCustomer()
        {
            return customer;
        }
        public void setCustomer(int customer)
        {
            this.customer = customer;
        }
        public int getPriority()
        {
            return priority;
        }
        public void setPriority(int priority)
        {
            this.priority = priority;
        }
        public double getVolume()
        {
            return volume;
        }
        public void setVolume(double volume)
        {
            this.volume = volume;
        }

        public double getBaseSimilarity()
        {
            return baseSimilarity;
        }

        public void setBaseSimilarity(double baseSimilarity)
        {
            this.baseSimilarity = baseSimilarity;
        }

        /* ##SETTERS## : set all points of cuboid based on only one point in input. */

        public void setBottomLeftFront(Point bottomLeftFront)
        {
            this.bottomLeftFront = bottomLeftFront;
            this.bottomRightFront = new Point(this.bottomLeftFront.x + width, this.bottomLeftFront.y, this.bottomLeftFront.z);
            this.topLeftFront = new Point(this.bottomLeftFront.x, this.bottomLeftFront.y + height, this.bottomLeftFront.z);
            this.topRightFront = new Point(this.bottomLeftFront.x + width, this.bottomLeftFront.y + height, this.bottomLeftFront.z);
            this.bottomLeftRear = new Point(this.bottomLeftFront.x, this.bottomLeftFront.y, this.bottomLeftFront.z + depth);
            this.bottomRightRear = new Point(this.bottomLeftFront.x + width, this.bottomLeftFront.y, this.bottomLeftFront.z + depth);
            this.topLeftRear = new Point(this.bottomLeftFront.x, this.bottomLeftFront.y + height, this.bottomLeftFront.z + depth);
            this.topRightRear = new Point(this.bottomLeftFront.x + width, this.bottomLeftFront.y + height, this.bottomLeftFront.z + depth);
        }
        public void setBottomRightFront(Point bottomRightFront)
        {
            this.bottomRightFront = bottomRightFront;
            this.bottomLeftFront = new Point(this.bottomRightFront.x - width, this.bottomRightFront.y, this.bottomRightFront.z);
            this.topRightFront = new Point(this.bottomRightFront.x, this.bottomRightFront.y + height, this.bottomRightFront.z);
            this.topLeftFront = new Point(this.bottomRightFront.x - width, this.bottomRightFront.y + height, this.bottomRightFront.z);
            this.bottomRightRear = new Point(this.bottomRightFront.x, this.bottomRightFront.y, this.bottomRightFront.z + depth);
            this.bottomLeftRear = new Point(this.bottomRightFront.x - width, this.bottomRightFront.y, this.bottomRightFront.z + depth);
            this.topRightRear = new Point(this.bottomRightFront.x, this.bottomRightFront.y + height, this.bottomRightFront.z + depth);
            this.topLeftRear = new Point(this.bottomRightFront.x - width, this.bottomRightFront.y + height, this.bottomRightFront.z + depth);
        }

        public Point getTopLeftRear()
        {
            return topLeftRear;
        }
        public Point getTopRightRear()
        {
            return topRightRear;
        }
        public Point getTopLeftFront()
        {
            return topLeftFront;
        }
        public Point getTopRightFront()
        {
            return topRightFront;
        }
        public Point getBottomLeftRear()
        {
            return bottomLeftRear;
        }
        public double getXPosition()
        {
            Point pos = this.getBottomLeftFront();
            return pos.getX();
        }
        public double getYPosition()
        {
            Point pos = this.getBottomLeftFront();
            return pos.getY();
        }
        public double getZPosition()
        {
            Point pos = this.getBottomLeftFront();
            return pos.getZ();
        }
        public Point getBottomRightRear()
        {
            return bottomRightRear;
        }
        public Point getBottomLeftFront()
        {
            return bottomLeftFront;
        }
        public Point getBottomRightFront()
        {
            return bottomRightFront;
        }

        public int getUnloadingOrder()
        {
            return unloadingOrder;
        }

        public void setUnloadingOrder(int unloadingOrder)
        {
            this.unloadingOrder = unloadingOrder;
        }

        public double getTaxability()
        {
            return taxability;
        }

        public void setTaxability(double taxability)
        {
            this.taxability = taxability;
        }

        /* Get 6 extreme points of cuboid in order to use them to check intersections between items */
        public double getLeftPoint()
        {
            return this.topLeftRear.x;
        }
        public double getRightPoint()
        {
            return this.topRightRear.x;
        }
        public double getTopPoint()
        {
            return this.topLeftRear.y;
        }
        public double getBottomPoint()
        {
            return this.bottomLeftRear.y;
        }
        public double getRearPoint()
        {
            return this.bottomLeftFront.z;
        }
        public double getFrontPoint()
        {
            return this.bottomLeftRear.z;
        }


        public void rotate90()
        {
            double tmp = this.getWidth();
            this.setWidth(this.getDepth());
            this.setDepth(tmp);
        }

        public override string ToString()
        {
            return "\nItem{" +
                    "id=" + id; //+
            /*                ", customer=" + customer +
                            ", priority=" + priority +
                            ", width=" + width +
                            ", height=" + height +
                            ", depth=" + depth +
                            ", weight=" + weight +
                            ", volume=" + volume +
                            ", stackable=" + stackable +
                            ", \ntopLeftFront=" + topLeftFront +
                            ", \ntopRightFront=" + topRightFront +
                            ", \ntopLeftRear=" + topLeftRear +
                            ", \ntopRightRear=" + topRightRear +
                            ", \nbottomLeftFront=" + bottomLeftFront +
                            ", \nbottomRightFront=" + bottomRightFront +
                            ", \nbottomLeftRear=" + bottomLeftRear +
                            ", \nbottomRightRear=" + bottomRightRear +
                            '}';*/
        }

        public String toCSV()
        {
            String str = this.id + "," + this.width + "," + this.height + "," + this.depth + "," + this.topLeftFront.x + "," + this.topLeftFront.y + "," + this.topLeftFront.z + "," +
                    this.topRightFront.x + "," + this.topRightFront.y + "," + this.topRightFront.z + "," +
                    this.bottomRightFront.x + "," + this.bottomRightFront.y + "," + this.bottomRightFront.z + "," +
                    this.bottomLeftFront.x + "," + this.bottomLeftFront.y + "," + this.bottomLeftFront.z + "," +
                    this.bottomRightRear.x + "," + this.bottomRightRear.y + "," + this.bottomRightRear.z + "," +
                    this.bottomLeftRear.x + "," + this.bottomLeftRear.y + "," + this.bottomLeftRear.z + "," +
                    this.topLeftRear.x + "," + this.topLeftRear.y + "," + this.topLeftRear.z + "," +
                    this.topRightRear.x + "," + this.topRightRear.y + "," + this.topRightRear.z + "\n";
            return str;
        }

        public String toCSVOutputDescription()
        {
            String str = this.id + "," + this.priority + "," + this.unloadingOrder + "," + this.taxability + "," + this.bottomLeftFront.x + ","
                    + this.bottomLeftFront.y + "," + this.bottomLeftFront.z + "," + this.depth + "," + this.width + "," + this.height + "\n";
            return str;
        }

        public String toCSVFullListWithID()
        {
            String str = this.id + "," + this.shipmentCode + "," + this.priority + "," + this.unloadingOrder + "," + this.taxability + "," + this.depth + "," + this.width + "," + this.height + "\n";
            return str;
        }

        public Point getCentre()
        {
            double x, y, z;
            x = (this.getBottomLeftFront().getX() + this.getWidth()) / 2;
            y = (this.getBottomLeftFront().getY() + this.getHeight()) / 2;
            z = (this.getBottomLeftFront().getZ() + this.getDepth()) / 2;
            Point p = new Point(x, y, z);
            return p;
        }

        public String toCSVOrderedList()
        {
            String str = this.id + "," + this.shipmentCode + "," + this.priority + "," + this.unloadingOrder + "," + this.taxability + "\n";
            return str;
        }

        public bool checkSlimItem()
        {
            if ((this.height / this.depth) > 5 || (this.height / this.width) > 5)
            {
                return true;
            }
            return false;
        }

        public double getTaxabilityOverVolume()
        {
            return this.getTaxability() / this.getVolume();
        }

        public double getTaxabilityOverWeight()
        {
            return this.getTaxability() / this.getWeight();
        }

    }
}
