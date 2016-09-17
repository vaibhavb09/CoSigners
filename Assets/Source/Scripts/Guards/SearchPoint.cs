//#define _DEBUG_SPEW
/**
 * @Description : This class is meant to be a central repository for all guard Search points
 *  The "m_searchPoints" list contains all search points that are currently in existence on this
 * 		level and the "getMostRelevantSearchPoint" method returns the most relevant search
 * 		point from any given position.
 * 
 * */

using UnityEngine;
using System.Collections.Generic;
using System;

public class SearchPoint
{
	// stores all the search points
	private static List<SearchPoint> m_searchPoints = new List<SearchPoint>();
	private static int mNumberOfSearchPoints = 0;
	
	// The id for this search pt
	int mSearchPointId;
	
	// The actual position of this search point
	private Vector3 m_Position;
	public Vector3 Position
	{
		get
		{
			return m_Position;
		}
	}
	
	// Indicates the actual position of the search Point
	private float m_PositionX,m_PositionZ;
	
	// Indicates the angle information
	private float m_AngleStart,m_AngleEnd;
	public float startingAngle
	{
		get
		{
			return m_AngleStart;
		}
	}
	
	public float angleEnd
	{
		get
		{
			return m_AngleEnd;
		}		
	}
	
	private Vector2 mVectStart,mVectEnd;
	
	// cap value
	// This indicates the number of search points that a guard will string together in one search operation
	private static int s_CapnewSearchPoints = 3;
	
	// The threshold for the radial checks 
	public static float m_RadialCheckThreshold = 200;
	
	/**
	 * @Description : Creates a new search point and adds it to the search point List
	 * @Param : 1. The search point info as read from the file
	 * */
	public static bool createSerarchPoint(SearchPointData i_searchPoint)
	{
		// Check if the search point already exists 
		foreach(SearchPoint point in m_searchPoints)
		{
			// If it does 
			if((point.m_PositionX == i_searchPoint.xPos) &&
				(point.m_PositionZ == i_searchPoint.zPos))
				// just return false
				return false;
		}
		
		mNumberOfSearchPoints++;
		// If one does not exist, then create a new one
		// 60 to 240 degrees for now
		// Add the search point to the list
		m_searchPoints.Add(new SearchPoint(i_searchPoint.xPos,
			i_searchPoint.zPos,60,240,mNumberOfSearchPoints));
		
		return true;
	}
	
	public static void initializeSearchPointList(ref SearchPointData[] i_searchPtData)
	{
		//Debug.Log("Number of search points in the XML : " + i_searchPtData.Length);
		
		foreach(SearchPointData searchPt in i_searchPtData)
		{
			mNumberOfSearchPoints++;
//			Debug.Log("Number of Search points : " + mNumberOfSearchPoints);
			// Create a new Search point
			// 60 to 240 degrees for now
			// Add the search point to the list
			m_searchPoints.Add(new SearchPoint(searchPt.xPos,searchPt.zPos,60,240,mNumberOfSearchPoints));
			
//			Debug.Log("Created new search point at : " + searchPt.xPos 
//				+"  " + searchPt.zPos + "  Angle : 60,240" );
		}
	}
	
	/**
	 * @Description : Creates a new Search Point
	 * */
	private SearchPoint(float i_PositionX,float i_PositionZ, float i_AngleStart,float i_AngleEnd,int iD)
	{
		m_PositionX = i_PositionX;
		m_PositionZ = i_PositionZ;
		
		m_AngleStart = i_AngleStart;
		m_AngleEnd = i_AngleEnd;
		
		float _AngleStart = i_AngleStart * Mathf.Deg2Rad;
		float _AngleEnd = i_AngleEnd * Mathf.Deg2Rad;
		
		mVectStart = new Vector2(Mathf.Sin(_AngleStart),-1.0f * Mathf.Cos(_AngleStart));
		mVectEnd = new Vector2(Mathf.Sin(_AngleEnd),-1.0f * Mathf.Cos(_AngleEnd));
		
		m_Position = new Vector3(m_PositionX,0.0f,m_PositionZ);
		
		mSearchPointId = iD;
	}
	
	/**
	 * @Description : Calculates and returns the facing rotation angle
	 * @Return : Angle that the facing vector needs to rotate by in order to align properly
	 * 				Positive angle means a Counter clockwise rotation 
 	 *				Negative angle means a Clockwise rotation
	 * */
	public float calculateRotationAngle(Vector3 facingVector,out bool o_AlphaOrOmega)
	{
		Vector2 facingVectin2D = new Vector2(facingVector.x,facingVector.z);
		
		// Calculating angles between the facing vector and the start and end vectors
		float alpha = Mathf.Acos(Vector2.Dot(facingVectin2D,mVectStart)) / (facingVectin2D.magnitude * mVectStart.magnitude);
		float omega = Mathf.Acos(Vector2.Dot(facingVectin2D,mVectEnd)) / (facingVectin2D.magnitude * mVectEnd.magnitude);
		
		// Calculate the final angle of rotation with sign
		float Angle = 0.0f;
		
//		 = alpha >= omega ? (alpha * (facingVectin2D.x * mVectStart.y - mVectStart.x * facingVectin2D.y)) 
//			: (omega * (facingVectin2D.x * mVectEnd.y - mVectEnd.x * facingVectin2D.y));
		
		if(alpha >= omega)
		{
			Angle = alpha * (facingVectin2D.x * mVectStart.y - mVectStart.x * facingVectin2D.y)>=0 ? 1 : -1;
			o_AlphaOrOmega = true;
		}else
		{
			Angle = (omega * (facingVectin2D.x * mVectEnd.y - mVectEnd.x * facingVectin2D.y) >=0 ? 1 : -1 );
			o_AlphaOrOmega = false;
		}
		
		// return it
		return Angle;			
	}
	
	/**
	 * @Description : Creates a new Search Point
	 * */
	private SearchPoint()
	{
		m_PositionX = 0;
		m_PositionZ = 0;
		
		m_AngleStart = 0;
		m_AngleEnd = 0;
		
		mVectEnd = mVectStart = new Vector2(0,-1);
		
		m_Position = new Vector3();
	}
	
	private static SearchPoint getSearchPointById(int iSearchPointID)
	{
//		try
//		{
			return m_searchPoints[iSearchPointID-1];
//		}catch (Exception e)
//		{
//			Debug.Log(e.ToString());
//			
//			Debug.Log("The search point id is : " + iSearchPointID);
//			
//			for (int i =0 ; i < m_searchPoints.Count ; i++)
//			{
//				Debug.Log(m_searchPoints[i].Position);
//			}
//			
//			return m_searchPoints[0];
//			
//		}
	}
		

	
	/**
	 * @Description : Gets the closest search point to indicated reference position
	 * @Param : 1. The position from where the most relevant search point is to be located
	 * */
	public static SearchPoint getMostRelevantSearchPoint(Vector3 i_referencePosition,SearchPoint iCurrentSearchPt,ref List<int> i_PreviousSearchPoints)
	{
		SearchPoint mostRelevantSearchPoint=null;
		
		if(i_PreviousSearchPoints.Count < s_CapnewSearchPoints)
		{			
			if(iCurrentSearchPt == null)
			{
				//Debug.Log("#1");
				
				float minimumDistance = float.MaxValue;
				
				Vector2 refPositionin2D = new Vector2(i_referencePosition.x,i_referencePosition.z);
				Vector2 searchPt = new Vector2();
				
				foreach(SearchPoint point in m_searchPoints)
				{
					searchPt.Set(point.m_PositionX,point.m_PositionZ);
					
					//Debug.Log("Looking at point : " + searchPt);
					
					float dist = Vector2.Distance(refPositionin2D,searchPt); 
					
					if (dist < minimumDistance)
					{
						minimumDistance = dist;
						
						if(minimumDistance < m_RadialCheckThreshold)
							mostRelevantSearchPoint = point;
						
						// store the search point in the previous search points list
						i_PreviousSearchPoints.Add(mostRelevantSearchPoint.mSearchPointId);
					}
				}
			}
			else
			{
				//Debug.Log("#2");
				
				float minimumDistance = float.MaxValue;
				
				Vector2 refPositionin2D = new Vector2(iCurrentSearchPt.m_PositionX,iCurrentSearchPt.m_PositionZ);
				Vector2 searchPt = new Vector2();
				
				foreach(SearchPoint point in m_searchPoints)
				{
					bool checkInPrevPointsList = false;
					
					if(point.mSearchPointId != iCurrentSearchPt.mSearchPointId)
					{
						foreach (int searchPtId in i_PreviousSearchPoints)
						{
							if(point.mSearchPointId == searchPtId)
								checkInPrevPointsList = true;
							break;
						}
						
						if(checkInPrevPointsList)
							continue;
						
						searchPt.Set(point.m_PositionX,point.m_PositionZ);
						
						// Debug.Log("Looking at point : " + searchPt);
						
						float dist = Vector2.Distance(refPositionin2D,searchPt); 
						
						if (dist < minimumDistance)
						{
							minimumDistance = dist;
							
							if(minimumDistance < m_RadialCheckThreshold)
								mostRelevantSearchPoint = point;
							
							// store the search point in the previous search points list
							i_PreviousSearchPoints.Add(mostRelevantSearchPoint.mSearchPointId);
						}
					}
				}
			}
		}
		else
		{
			
			//Debug.Log ("#3");
			
			int currentSearchPtId = iCurrentSearchPt.mSearchPointId;
			int nextSearchPtIdLocinList = 0;
			
			// Hacky fix for IGF
			bool _isCurrentInList = false;
			
			for (int i=0 ; i<s_CapnewSearchPoints;i++)
			{				
				if(currentSearchPtId == i_PreviousSearchPoints[i])
				{
					_isCurrentInList = true;
					
					nextSearchPtIdLocinList = i_PreviousSearchPoints[(i+1)%s_CapnewSearchPoints];
				}
			}	
			
			mostRelevantSearchPoint = getSearchPointById( _isCurrentInList ? nextSearchPtIdLocinList : (s_CapnewSearchPoints-1));
			
		}

		#if _DEBUG_SPEW
			Debug.Log("Most Relevant search Point is : " + mostRelevantSearchPoint.mSearchPointId);

			Debug.Log("Most Relevant search Point position : " + mostRelevantSearchPoint.Position);
		#endif
		
		return mostRelevantSearchPoint;
	}
}

