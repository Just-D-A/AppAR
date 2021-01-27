namespace OpenCvSharp.Demo
{
	using UnityEngine;
	using System;
	using OpenCvSharp;
	using System.Collections.Generic;

	public class LiveSketchScript : WebCamera
	{
		public Texture2D marker_1;
		public Texture2D marker_2;
		public Texture2D marker_3;
		public Texture2D marker_4;
		public Texture2D marker_5;

		private const String ERROR_MESSAGE = "ERROR: Find more than one contour";
		private const int MARKERS_COUNT = 5;
		private const int MAX_CONTURES_COUNT = 6;
		private const int DELTA = 10; 
		private List<Texture2D> textures = new List<Texture2D>();
		private List<List<Point>> templateContours = new List<List<Point>>();

		protected override void Awake()
		{
			base.Awake();
			this.forceFrontalCamera = true;

			textures.Add(marker_1);
			textures.Add(marker_2);
			textures.Add(marker_3);
			textures.Add(marker_4);
			textures.Add(marker_5);

			foreach (var texture in textures)
            {
				templateContours.AddRange(GetContours(texture));
				Debug.Log(templateContours.Count);
            }
		}

		// Our sketch generation function
		protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
		{
			Mat image = Unity.TextureToMat(input, TextureParameters);
			HierarchyIndex[] hierarchy = new HierarchyIndex[0];

			List<List<Point>> allContours = GetContours(image, ref hierarchy);

			foreach (var contour in allContours)
			{
				Scalar color = new Scalar(0, 0, 0);
				Cv2.DrawContours(image, new Point[][] { contour.ToArray() }, 0, color, 2, LineTypes.Link8, hierarchy, 0);
			}

			output = Unity.MatToTexture(image, output);
			//Debug.Log("CONTOURS COUNT: " + allContours.Count);

			if (allContours.Count == MAX_CONTURES_COUNT)
			{
				List<Point> mainTexture = allContours[0];
				Debug.Log(mainTexture);
				for (int i = 0; i < templateContours.Count; ++i)
				{
					if (checkContures(mainTexture, templateContours[i]))
					{
						Debug.Log("FIRST LIST AND " + i + " LIST IS EQUAL");
					}
					else
					{
						Debug.Log("FIRST LIST AND " + i + " LIST IS NOT EQUAL");
					}
				}
			}
			else
			{
				//Debug.Log(ERROR_MESSAGE);
			}
			return true;
		}

		private List<List<Point>> GetContours(Texture2D texture)
        {
			Mat image = Unity.TextureToMat(texture);
			HierarchyIndex[] hierarchy = new HierarchyIndex[0];
			return GetContours(image, ref hierarchy);
		}

		private List<List<Point>> GetContours(Mat image, ref HierarchyIndex[] hierarchy)
		{
			Mat grayMat = new Mat();
			Cv2.CvtColor(image, grayMat, ColorConversionCodes.BGR2GRAY);
			Point[][] contours;
			List<List<Point>> allContours = new List<List<Point>>();

			Mat thresh = new Mat();
			Cv2.Threshold(grayMat, thresh, 127, 255, ThresholdTypes.BinaryInv);

			Cv2.FindContours(thresh, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

			foreach (Point[] contour in contours)
			{
				//convert to list
				List<Point> convertContour = new List<Point>();
				convertContour.AddRange(contour);
				allContours.Add(convertContour);
			}

			return allContours;
		}

		private bool checkContures(List<Point> mainTexture, List<Point> currentTexture)
        {
			int minCount = 0;
			if(mainTexture.Count > currentTexture.Count)
            {
				minCount = currentTexture.Count;
			}
			else
            {
				minCount = mainTexture.Count;

			}
			for (int i = 0; i < minCount; i++)
            {
				Point mainPoint = mainTexture[i];
				Point currentPoint = currentTexture[i];
				
				if(((mainPoint.X - currentPoint.X) > DELTA) || ((mainPoint.X - currentPoint.X) < -DELTA)
					||
					((mainPoint.Y - currentPoint.Y) > DELTA) || ((mainPoint.Y - currentPoint.Y) < -DELTA))
                {
					return false;
                }
			}
			
			return true;
        }
	}
}