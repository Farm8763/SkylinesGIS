﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SkylinesGIS
{
    class GeoUtils
    {
        int mapWidthKilometers = 10;
        int mapLengthKilometers = 10;
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
