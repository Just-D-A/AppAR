namespace OpenCvSharp.Demo
{
	using UnityEngine;
	using System;
	using System.Collections;
	using OpenCvSharp;
	using UnityEngine.UI;
	using System.Collections.Generic;
	using System.Linq;

	public class ContoursByShapeScript : MonoBehaviour {


		public Texture2D exampleImage;

		public Texture2D marker_1;
		public Texture2D marker_2;
		public Texture2D marker_3;
		public Texture2D marker_4;
		public Texture2D marker_5;

		private const String ERROR_MESSAGE = "ERROR: Find more than one contour";
		private const int MARKERS_COUNT = 5;
		private const int MAX_CONTURES_COUNT = 6;

		// Use this for initialization
		void Start () {
			List<Texture2D> textures = new List<Texture2D>();
			textures.Add(exampleImage);
			textures.Add(marker_1);
			textures.Add(marker_2);
			textures.Add(marker_3);
			textures.Add(marker_4);
			textures.Add(marker_5);


			//Point[][] allContours = new Point[][] { new Point[] { new Point() } }; //it works!!!
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

			if(allContours.Count == MAX_CONTURES_COUNT)
            {
				List<Point> mainTexture = allContours[0];
				for (int i = 1; i < MAX_CONTURES_COUNT; i++)
                {
					if(mainTexture.SequenceEqual(allContours[i]))
                    {
						Debug.Log("FIRST LIST AND " + i + " LIST IS EQUAL" );
					} else
                    {
						Debug.Log("FIRST LIST AND " + i + " LIST IS NOT EQUAL");
					}
				}
			} else
            {
				Debug.Log(ERROR_MESSAGE);
			}

		}

		// Update is called once per frame
		void Update () {

		}


	}
}