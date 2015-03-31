using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using KMLib;
using KMLib.Feature;
using KMLib.Geometry;

namespace SkylinesGIS
{
    public class GeoUtils
    {
        int mapWidthKilometers = 10;
        int mapLengthKilometers = 10;
        int tileWidthVector = 2000;
        int mapWidthVector = 10000;
        int maximumX = 5000;
        int minimumX = -5000;
        int minimumY = -5000;
        int maximumY = 5000;
        double _eQuatorialEarthRadius = 6378.1370D;
        double _d2r = (Math.PI / 180D);

        public int HaversineInM(double lat1, double long1, double lat2, double long2)
        {
            return (int)(1000D * HaversineInKM(lat1, long1, lat2, long2));
        }

        public double HaversineInKM(double lat1, double long1, double lat2, double long2)
        {
            double dlong = (long2 - long1) * _d2r;
            double dlat = (lat2 - lat1) * _d2r;
            double a = Math.Pow(Math.Sin(dlat / 2D), 2D) + Math.Cos(lat1 * _d2r) * Math.Cos(lat2 * _d2r) * Math.Pow(Math.Sin(dlong / 2D), 2D);
            double c = 2D * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1D - a));
            double d = _eQuatorialEarthRadius * c;

            return d;
        }
        public class GpsMap
        {
            public KmlPoint topLeftCorner;
            public KmlPoint topRightCorner;
            public KmlPoint bottomLeftCorner;
            public KmlPoint bottomRightCorner;

            public GpsMap(KmlPoint corner)
            {
                    double radianLat= Math.PI * corner.Latitude/180;
                    this.topLeftCorner = corner;
                    float bottomLatitude = corner.Latitude - (float)(10 / 111.111);
                    float rightLongitude = corner.Longitude + (float)(10 / 111.111) / (float)Math.Cos(radianLat);
                    this.bottomLeftCorner = new KmlPoint(corner.Longitude, bottomLatitude);
                    this.bottomRightCorner = new KmlPoint(rightLongitude, bottomLatitude);
                    this.topRightCorner = new KmlPoint(rightLongitude, corner.Latitude);
            }

        }
        public bool CheckPointInMap(GpsMap map, KmlPoint pointToCheck)
        {
            bool result = false;
            //if (pointToCheck.Latitude)
            return result;
        }
        public Vector2 CartesianToPolar(Vector3 point)
        {
            Vector2 polar;
            //calc longitude 
            polar.y = Mathf.Atan2(point.x, point.z);

            //this is easier to write and read than sqrt(pow(x,2), pow(y,2))!
            float xzLen = new Vector2(point.x, point.z).magnitude;
            //do the atan thing to get our latitude
            polar.x = Mathf.Atan2(-point.y, xzLen);

            //convert to degrees
            polar *= Mathf.Rad2Deg;

            return polar;
        }

        public Vector3 PolarToCartesian(Vector2 polar)
        {
            //an origin vector, representing lat,lon of 0,0. 
            Vector3 origin = new Vector3(0, 0, 1);
            //generate a rotation quat based on polar's angle values
            Quaternion rotation = Quaternion.Euler(polar.x, polar.y, 0);
            //rotate origin by rotation
            Vector3 point = rotation * origin;

            return point;
        }
    }
}
