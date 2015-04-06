using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using KMLib;
using KMLib.Feature;
using KMLib.Geometry;
using Core.Geometry;

namespace SkylinesGIS
{
    public class GpsMap
    {
        public KmlPoint topLeftCorner;
        public KmlPoint topRightCorner;
        public KmlPoint bottomLeftCorner;
        public KmlPoint bottomRightCorner;
        public KmlPoint middle;
        float bottomLatitude;
        float topLatitude;
        float rightLongitude;
        float leftLongitude;

        /* 
         * Takes the middle point of the map and defines the corner points. 
         */
        public GpsMap(KmlPoint middle)
        {
            double radianLat = Math.PI * middle.Latitude / 180;
               
            bottomLatitude = middle.Latitude - (float)(5 / 111.111);
            topLatitude = middle.Latitude + (float)(5 / 111.111);
            rightLongitude = middle.Longitude + (float)(5 / 111.111) / (float)Math.Cos(radianLat);
            leftLongitude = middle.Longitude - (float)(5 / 111.111) / (float)Math.Cos(radianLat);

            this.middle = middle;
            this.topLeftCorner = new KmlPoint(leftLongitude, topLatitude);
            this.bottomLeftCorner = new KmlPoint(leftLongitude, bottomLatitude);
            this.bottomRightCorner = new KmlPoint(rightLongitude, bottomLatitude);
            this.topRightCorner = new KmlPoint(rightLongitude, topLatitude);
        }
        public bool CheckPointInMap(KmlPoint pointToCheck)
        {
            bool result = false;
            if (pointToCheck.Latitude <= topLeftCorner.Latitude && pointToCheck.Latitude >= bottomLeftCorner.Latitude
                && pointToCheck.Longitude >= topLeftCorner.Longitude && pointToCheck.Longitude <= topRightCorner.Longitude)
            {
                result = true;
            }
            return result;
        }
        public float getZFromPoint(Point3D point)
        {
            float z = 0;
            z = (float)GeoUtils.HaversineInM((double)this.middle.Latitude, 0, point.Y, 0);
            return z;
        }
        public float getXFromPoint(Point3D point)
        {
            float x = 0;
            x = (float) GeoUtils.HaversineInM(0, (double)this.middle.Longitude, 0, point.X);
            return x;
        }
        public Vector3 getVectorFromPoint(Point3D point)
        {
            Vector3 convertedPoint;

            convertedPoint = new Vector3(getXFromPoint(point), 0, getZFromPoint(point));

            return convertedPoint;
        }
    }

    public static class GeoUtils
    {
        static int mapWidthKilometers = 10;
        static int mapLengthKilometers = 10;
        static int tileWidthVector = 2000;
        static int mapWidthVector = 10000;
        static int maximumX = 5000;
        static int minimumX = -5000;
        static int minimumY = -5000;
        static int maximumY = 5000;
        static double _eQuatorialEarthRadius = 6378.1370D;
        static double _d2r = (Math.PI / 180D);

        public static int HaversineInM(double lat1, double long1, double lat2, double long2)
        {
            return (int)(1000D * HaversineInKM(lat1, long1, lat2, long2));
        }

        public static double HaversineInKM(double lat1, double long1, double lat2, double long2)
        {
            double dlong = (long2 - long1) * _d2r;
            double dlat = (lat2 - lat1) * _d2r;
            double a = Math.Pow(Math.Sin(dlat / 2D), 2D) + Math.Cos(lat1 * _d2r) * Math.Cos(lat2 * _d2r) * Math.Pow(Math.Sin(dlong / 2D), 2D);
            double c = 2D * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1D - a));
            double d = _eQuatorialEarthRadius * c;

            return d;
        }
        
    }
}
