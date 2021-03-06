﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEM.Net.Lib.Services
{
	//
	// Code from https://gist.github.com/oliverheilig/7777382
	//
	public static class DouglasPeucker
	{
		/// <summary>
		/// Uses the Douglas Peucker algorithim to reduce the number of points.
		/// </summary>
		/// <param name="Points">The points.</param>
		/// <param name="Tolerance">The tolerance.</param>
		/// <returns></returns>
		public static List<GeoPoint> DouglasPeuckerReduction(List<GeoPoint> Points, double Tolerance)
		{
			if (Points == null || Points.Count < 3)
				return Points;

			int firstPoint = 0;
			int lastPoint = Points.Count - 1;
			var pointIndexsToKeep = new List<int>();

			//Add the first and last index to the keepers
			pointIndexsToKeep.Add(firstPoint);
			pointIndexsToKeep.Add(lastPoint);

			//The first and the last point can not be the same
			while (Points[firstPoint].Equals(Points[lastPoint]))
			{
				lastPoint--;
			}

			DouglasPeuckerReduction(Points, firstPoint, lastPoint, Tolerance, ref pointIndexsToKeep);

			var returnPoints = new List<GeoPoint>();
			pointIndexsToKeep.Sort();
			foreach (int index in pointIndexsToKeep)
			{
				returnPoints.Add(Points[index]);
			}

			return returnPoints;
		}

		/// <summary>
		/// Douglases the peucker reduction.
		/// </summary>
		/// <param name="points">The points.</param>
		/// <param name="firstPoint">The first point.</param>
		/// <param name="lastPoint">The last point.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <param name="pointIndexsToKeep">The point indexs to keep.</param>
		private static void DouglasPeuckerReduction(List<GeoPoint> points, int firstPoint, int lastPoint, double tolerance, ref List<int> pointIndexsToKeep)
		{
			double maxDistance = 0;
			int indexFarthest = 0;

			for (int index = firstPoint; index < lastPoint; index++)
			{
				double distance = PerpendicularDistance(points[firstPoint], points[lastPoint], points[index]);
				if (distance > maxDistance)
				{
					maxDistance = distance;
					indexFarthest = index;
				}
			}

			if (maxDistance > tolerance && indexFarthest != 0)
			{
				//Add the largest point that exceeds the tolerance
				pointIndexsToKeep.Add(indexFarthest);

				DouglasPeuckerReduction(points, firstPoint, indexFarthest, tolerance, ref pointIndexsToKeep);
				DouglasPeuckerReduction(points, indexFarthest, lastPoint, tolerance, ref pointIndexsToKeep);
			}
		}

		/// <summary>
		/// The distance of a point from a line made from point1 and point2.
		/// </summary>
		/// <param name="pt1">The PT1.</param>
		/// <param name="pt2">The PT2.</param>
		/// <param name="p">The p.</param>
		/// <returns></returns>
		public static double PerpendicularDistance(GeoPoint Point1, GeoPoint Point2, GeoPoint Point)
		{
			//Area = |(1/2)(x1y2 + x2y3 + x3y1 - x2y1 - x3y2 - x1y3)|   *Area of triangle
			//Base = √((x1-x2)²+(x1-x2)²)                               *Base of Triangle*
			//Area = .5*Base*H                                          *Solve for height
			//Height = Area/.5/Base

			double area = Math.Abs(.5 * (Point1.DistanceFromOriginMeters * Point2.Elevation.GetValueOrDefault(0) + Point2.DistanceFromOriginMeters * Point.Elevation.GetValueOrDefault(0) + Point.DistanceFromOriginMeters * Point1.Elevation.GetValueOrDefault(0) - Point2.DistanceFromOriginMeters * Point1.Elevation.GetValueOrDefault(0) - Point.DistanceFromOriginMeters * Point2.Elevation.GetValueOrDefault(0) - Point1.DistanceFromOriginMeters * Point.Elevation.GetValueOrDefault(0)));
			double bottom = Math.Sqrt(Math.Pow(Point1.DistanceFromOriginMeters - Point2.DistanceFromOriginMeters, 2) + Math.Pow(Point1.Elevation.GetValueOrDefault(0) - Point2.Elevation.GetValueOrDefault(0), 2));
			double height = area / bottom * 2;

			return height;
		}
	}
}
