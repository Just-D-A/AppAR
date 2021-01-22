namespace OpenCvSharp.Demo
{
	using UnityEngine;
	using System;
	using System.Collections;
	using OpenCvSharp;
	using UnityEngine.UI;
	using System.Collections.Generic;
	using System.Linq;


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

		protected override void Awake()
		{
			base.Awake();
			this.forceFrontalCamera = true;
		}

		// Our sketch generation function
		protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
		{
			Mat img = Unity.TextureToMat(input, TextureParameters);

			//Convert image to grayscale
			Mat imgGray = new Mat ();
			Cv2.CvtColor (img, imgGray, ColorConversionCodes.BGR2GRAY);
			
			// Clean up image using Gaussian Blur
			Mat imgGrayBlur = new Mat ();
			Cv2.GaussianBlur (imgGray, imgGrayBlur, new Size (5, 5), 0);

			//Extract edges
			Mat cannyEdges = new Mat ();
			Cv2.Canny (imgGrayBlur, cannyEdges, 10.0, 70.0);

			//Do an invert binarize the image
			Mat mask = new Mat ();
			Cv2.Threshold (cannyEdges, mask, 70.0, 255.0, ThresholdTypes.BinaryInv);

			// result, passing output texture as parameter allows to re-use it's buffer
			// should output texture be null a new texture will be created
			output = Unity.MatToTexture(mask, output);

			List<Texture2D> textures = new List<Texture2D>();
			textures.Add(output);
			textures.Add(marker_1);
			textures.Add(marker_2);
			textures.Add(marker_3);
			textures.Add(marker_4);
			textures.Add(marker_5);

			List<List<Point>> allContours = new List<List<Point>>();

			foreach (Texture2D texture in textures)
			{

				Mat image = Unity.TextureToMat(texture);

				Mat grayMat = new Mat();
				Cv2.CvtColor(image, grayMat, ColorConversionCodes.BGR2GRAY);
				Point[][] contours;

				Mat thresh = new Mat();
				Cv2.Threshold(grayMat, thresh, 127, 255, ThresholdTypes.BinaryInv);

				HierarchyIndex[] hierarchy;
				Cv2.FindContours(thresh, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);


				foreach (Point[] contour in contours)
				{
					Scalar color = new Scalar(0, 0, 0);//rng.Uniform(0, 256)
					Cv2.DrawContours(image, new Point[][] { contour }, 0, color, 2, LineTypes.Link8, hierarchy, 0);

					//convert to list
					List<Point> convertContour = new List<Point>();
					convertContour.AddRange(contour);
					allContours.Add(convertContour);
				}

			}
			Debug.Log("CONTOURS COUNT: " + allContours.Count);

			if (allContours.Count == MAX_CONTURES_COUNT)
			{
				List<Point> mainTexture = allContours[0];
				for (int i = 1; i < MAX_CONTURES_COUNT; i++)
				{
					if (checkContures(mainTexture, allContours[i]))
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
				Debug.Log(ERROR_MESSAGE);
			}
			return true;
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